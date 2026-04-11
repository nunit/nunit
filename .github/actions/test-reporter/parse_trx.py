#!/usr/bin/env python3
"""
NUnit Test Reporter - TRX Parser
Parses TRX files and generates GitHub summary with framework information
"""

import os
import re
import json
import xml.etree.ElementTree as ET
from pathlib import Path
from glob import glob
from urllib import request
from urllib.error import HTTPError

# Inputs from environment
name = os.environ.get('INPUT_NAME', 'Test Results')
path_pattern = os.environ.get('INPUT_PATH', 'TestResults/**/*.trx')
fail_on_error = os.environ.get('INPUT_FAIL_ON_ERROR', 'true').lower() == 'true'
github_token = os.environ.get('GITHUB_TOKEN', '')
commit_sha = os.environ.get('COMMIT_SHA') or os.environ.get('GITHUB_SHA', '')
repo = os.environ.get('GITHUB_REPOSITORY', '')
server_url = os.environ.get('GITHUB_SERVER_URL', 'https://github.com')
repo_url = f"{server_url}/{repo}"

print(f"Using commit SHA: {commit_sha}")

# Debug: show current directory and contents
cwd = os.getcwd()
print(f"Current working directory: {cwd}")
print(f"Looking for pattern: {path_pattern}")

# List TestResults directory if it exists
test_results_dir = Path("TestResults")
if test_results_dir.exists():
    print(f"TestResults directory exists, contents:")
    for f in test_results_dir.rglob("*"):
        print(f"  {f}")
else:
    print(f"TestResults directory does NOT exist at {test_results_dir.absolute()}")

# Find all TRX files
trx_files = glob(path_pattern, recursive=True)

if not trx_files:
    alt_pattern = "**/*.trx"
    print(f"Trying alternative pattern: {alt_pattern}")
    trx_files = glob(alt_pattern, recursive=True)

# VSTest often writes under TestResults/<guid>/; glob patterns can miss some layouts — scan the tree
if not trx_files and test_results_dir.is_dir():
    trx_files = sorted(str(p) for p in test_results_dir.rglob("*.trx"))
    if trx_files:
        print(f"Found {len(trx_files)} TRX file(s) via TestResults/**/*.trx (rglob)")

if not trx_files:
    print(f"::warning::No TRX files found matching pattern: {path_pattern}")
    exit(0)

print(f"Found {len(trx_files)} TRX file(s)")

# Regex to extract framework from paths
framework_regex = re.compile(r'[/\\](net\d+\.\d+|net\d+|netcoreapp\d+\.\d+|netstandard\d+\.\d+)[/\\]', re.IGNORECASE)

# Data structures
results = []
all_failed_tests = []
total_passed = 0
total_failed = 0
total_skipped = 0
total_tests = 0


def extract_framework(storage_path):
    """Extract framework from assembly path like bin/Release/net8.0/"""
    if not storage_path:
        return "unknown"
    match = framework_regex.search(storage_path)
    return match.group(1).lower() if match else "unknown"


def extract_project(storage_path):
    """Extract project name from DLL path"""
    if not storage_path:
        return "unknown"
    return Path(storage_path).stem


def _collapse_internal_whitespace(s):
    """Single spaces only — catches tabs vs spaces and double spaces in raw TRX lines."""
    return re.sub(r'\s+', ' ', s.strip())


def dedupe_repeated_sequence(items):
    """
    If the whole list is N>=2 copies of the same block and the block has length>=2,
    return one copy. Used when list items compare equal as strings.
    """
    n = len(items)
    if n < 4:
        return items
    for period in range(2, n // 2 + 1):
        if n % period != 0:
            continue
        block = items[:period]
        if all(items[i] == block[i % period] for i in range(n)):
            return list(block)
    return items


def dedupe_repeated_sequence_by_key(keys, items):
    """
    Same as dedupe_repeated_sequence but compares by parallel *keys* (e.g. method +
    file basename + line). Needed when duplicate stack halves produce different markdown
    (e.g. one frame resolves to a repo src/ link and the duplicate uses bin/ paths).
    """
    n = len(keys)
    if n < 4 or n != len(items):
        return items
    for period in range(2, n // 2 + 1):
        if n % period != 0:
            continue
        block = keys[:period]
        if all(keys[i] == block[i % period] for i in range(n)):
            return list(items[:period])
    return items


def parse_stack_frame_in_file(line):
    """
    Parse a CLR stack line with file info. Returns (method, file_path, line_num_str) or None.
    Supports :line N and trailing :N (Core/5+ sometimes omits the word 'line').
    """
    m = re.search(r'at (.+?) in (.+?):line (\d+)', line)
    if m:
        return m.group(1).strip(), m.group(2), m.group(3)
    m = re.search(r'at (.+?) in (.+)', line)
    if not m:
        return None
    method, rest = m.group(1).strip(), m.group(2)
    m2 = re.match(r'(.+):(?:line )?(\d+)\s*$', rest)
    if m2:
        return method, m2.group(1), m2.group(2)
    return None


def dedupe_repeated_stack_trace(stack_trace):
    """
    VSTest / adapters sometimes emit the same stack frames multiple times in a row.
    If the entire trace consists of N>=2 identical copies of a multi-line block,
    return a single copy. Skips period-1 repetition so consecutive identical frames
    from recursion are not collapsed.

    Comparison uses collapsed internal whitespace so minor spacing differences between
    duplicate halves do not prevent detection.
    """
    if not stack_trace or not stack_trace.strip():
        return stack_trace
    normalized = stack_trace.replace('\r\n', '\n').replace('\r', '\n')
    lines = [ln.strip() for ln in normalized.split('\n') if ln.strip()]
    n = len(lines)
    if n < 4:
        return stack_trace
    norm = [_collapse_internal_whitespace(ln) for ln in lines]
    for period in range(2, n // 2 + 1):
        if n % period != 0:
            continue
        block_norm = norm[:period]
        if all(norm[i] == block_norm[i % period] for i in range(n)):
            # Preserve original spacing from the first occurrence
            return '\n'.join(lines[:period])
    return stack_trace


def parse_stack_trace(stack_trace):
    """Convert stack trace to markdown list with source links"""
    if not stack_trace:
        return ""

    stack_trace = dedupe_repeated_stack_trace(stack_trace)

    lines = []
    frame_keys = []

    for line in stack_trace.strip().split('\n'):
        line = line.strip()
        if not line:
            continue

        parsed = parse_stack_frame_in_file(line)
        if parsed:
            method, file_path, line_num = parsed
            file_name = Path(file_path.replace('\\', '/')).name
            # Dedupe key: basename + line only — full paths often differ between duplicate halves on net6+.
            frame_keys.append((method, file_name, line_num))

            # Try to extract relative path from src/
            src_match = re.search(r'[/\\](src[/\\].+)$', file_path)
            if src_match:
                rel_path = src_match.group(1).replace('\\', '/')
                file_name = Path(rel_path).name
                source_link = f"{repo_url}/blob/{commit_sha}/{rel_path}#L{line_num}"
                lines.append(f"- `{method}`<br/>  :point_right: [{file_name}:{line_num}]({source_link})")
            else:
                file_name = Path(file_path.replace('\\', '/')).name
                lines.append(f"- `{method}`<br/>  :point_right: {file_name}:{line_num}")
        else:
            # Match: at Method() without file info
            match = re.search(r'at (.+)', line)
            if match:
                method = match.group(1).strip()
                frame_keys.append((method, '', ''))
                lines.append(f"- `{method}`")

    # Semantic dedupe: on net6+ duplicate halves often use different full paths (src/ vs bin/),
    # so markdown strings differ but (method, file basename, line) repeats.
    lines = dedupe_repeated_sequence_by_key(frame_keys, lines)
    lines = dedupe_repeated_sequence(lines)

    return "\n".join(lines) if lines else ""


# Parse each TRX file
for trx_file in trx_files:
    print(f"Processing: {Path(trx_file).name}")

    try:
        tree = ET.parse(trx_file)
        root = tree.getroot()

        # Handle namespace
        ns_match = re.match(r'\{(.+?)\}', root.tag)
        ns = {'t': ns_match.group(1)} if ns_match else {}

        def find(xpath):
            return root.find(xpath, ns) if ns else root.find(xpath.replace('t:', ''))

        def findall(xpath):
            return root.findall(xpath, ns) if ns else root.findall(xpath.replace('t:', ''))

        # Get counters
        counters = find('.//t:Counters')
        if counters is None:
            print(f"  No counters found, skipping")
            continue

        passed = int(counters.get('passed', 0))
        failed = int(counters.get('failed', 0))
        total = int(counters.get('total', 0))
        executed = int(counters.get('executed', 0))
        skipped = total - executed

        # Extract framework and project from first unit test
        framework = "unknown"
        project = "unknown"

        unit_test = find('.//t:UnitTest')
        if unit_test is not None:
            storage = unit_test.get('storage', '')
            framework = extract_framework(storage)
            project = extract_project(storage)

        # Collect failed tests
        for result in findall('.//t:UnitTestResult[@outcome="Failed"]'):
            test_name = result.get('testName', 'Unknown')
            test_id = result.get('testId', '')
            duration = result.get('duration', '')

            error_message = ""
            stack_trace = ""
            std_out = ""
            std_err = ""

            output = result.find('t:Output', ns) if ns else result.find('Output')
            if output is not None:
                error_info = output.find('t:ErrorInfo', ns) if ns else output.find('ErrorInfo')
                if error_info is not None:
                    msg_elem = error_info.find('t:Message', ns) if ns else error_info.find('Message')
                    stack_elem = error_info.find('t:StackTrace', ns) if ns else error_info.find('StackTrace')
                    error_message = msg_elem.text if msg_elem is not None and msg_elem.text else ""
                    stack_trace = stack_elem.text if stack_elem is not None and stack_elem.text else ""

                stdout_elem = output.find('t:StdOut', ns) if ns else output.find('StdOut')
                stderr_elem = output.find('t:StdErr', ns) if ns else output.find('StdErr')
                std_out = stdout_elem.text if stdout_elem is not None and stdout_elem.text else ""
                std_err = stderr_elem.text if stderr_elem is not None and stderr_elem.text else ""

            # Get class and method info
            class_name = ""
            method_name = ""
            if test_id:
                test_method = find(f'.//t:UnitTest[@id="{test_id}"]/t:TestMethod')
                if test_method is not None:
                    class_name = test_method.get('className', '')
                    method_name = test_method.get('name', '')

            all_failed_tests.append({
                'name': test_name,
                'framework': framework,
                'project': project,
                'error_message': error_message,
                'stack_trace': stack_trace,
                'std_out': std_out,
                'std_err': std_err,
                'duration': duration,
                'class_name': class_name,
                'method_name': method_name,
            })

        results.append({
            'project': project,
            'framework': framework,
            'passed': passed,
            'failed': failed,
            'skipped': skipped,
            'total': total,
        })

        total_passed += passed
        total_failed += failed
        total_skipped += skipped
        total_tests += total

    except Exception as e:
        print(f"::warning::Failed to parse {trx_file}: {e}")

# Group results by project and framework
grouped = {}
for r in results:
    key = f"{r['project']}|{r['framework']}"
    if key not in grouped:
        grouped[key] = {'project': r['project'], 'framework': r['framework'], 'passed': 0, 'failed': 0, 'skipped': 0, 'total': 0}
    grouped[key]['passed'] += r['passed']
    grouped[key]['failed'] += r['failed']
    grouped[key]['skipped'] += r['skipped']
    grouped[key]['total'] += r['total']

grouped_results = sorted(grouped.values(), key=lambda x: (x['project'], x['framework']))

# Determine overall status
status_icon = ":x:" if total_failed > 0 else ":white_check_mark:"
status_badge = "![Failed](https://img.shields.io/badge/tests-failed-red)" if total_failed > 0 else "![Passed](https://img.shields.io/badge/tests-passed-brightgreen)"
conclusion = "failure" if total_failed > 0 else "success"

# Build markdown summary
summary = f"""# {status_icon} {name}

{status_badge}

## Summary

| Total | Passed | Failed | Skipped |
|------:|-------:|-------:|--------:|
| {total_tests} | {total_passed} | {total_failed} | {total_skipped} |

## Results by Project and Framework

| Project | Framework | Passed | Failed | Skipped | Status |
|:--------|:----------|-------:|-------:|--------:|:------:|
"""

for r in grouped_results:
    icon = ":x:" if r['failed'] > 0 else ":white_check_mark:"
    summary += f"| {r['project']} | {r['framework']} | {r['passed']} | {r['failed']} | {r['skipped']} | {icon} |\n"

# Add failed tests section
if all_failed_tests:
    summary += f"\n## :x: Failed Tests ({len(all_failed_tests)})\n\n"

    for test in all_failed_tests:
        short_name = test['name'][:100] + "..." if len(test['name']) > 100 else test['name']

        summary += f"""
### :x: {short_name}

| Property | Value |
|:---------|:------|
| **Project** | {test['project']} |
| **Framework** | {test['framework']} |
| **Class** | `{test['class_name']}` |
| **Method** | `{test['method_name']}` |
| **Duration** | {test['duration']} |

"""

        if test['error_message']:
            summary += f"""**Error Message:**
```
{test['error_message']}
```

"""

        if test['stack_trace']:
            stack_table = parse_stack_trace(test['stack_trace'])
            summary += f"""**Stack Trace:**

{stack_table}

"""

        if test['std_out']:
            summary += f"""<details>
<summary>Standard Output</summary>

```
{test['std_out']}
```

</details>

"""

        if test['std_err']:
            summary += f"""<details>
<summary>Standard Error</summary>

```
{test['std_err']}
```

</details>

"""

        summary += "\n---\n"

# Write to GitHub Step Summary
github_step_summary = os.environ.get('GITHUB_STEP_SUMMARY', '')
if github_step_summary:
    with open(github_step_summary, 'a', encoding='utf-8') as f:
        f.write(summary)

# Set outputs
github_output = os.environ.get('GITHUB_OUTPUT', '')
if github_output:
    with open(github_output, 'a', encoding='utf-8') as f:
        f.write(f"passed={total_passed}\n")
        f.write(f"failed={total_failed}\n")
        f.write(f"skipped={total_skipped}\n")

print()
print("========================================")
print(f"  Test Results: {total_tests} total")
print(f"  Passed: {total_passed}")
print(f"  Failed: {total_failed}")
print(f"  Skipped: {total_skipped}")
print("========================================")

# Create GitHub Check Run via API
if github_token and repo and commit_sha:
    title = f"{total_tests} tests: {total_passed} passed, {total_failed} failed, {total_skipped} skipped"

    # Truncate summary if too long
    check_summary = summary
    if len(check_summary) > 65000:
        check_summary = check_summary[:65000] + "\n\n... (truncated, see Job Summary for full details)"

    api_url = f"https://api.github.com/repos/{repo}/check-runs"

    print(f"\nCreating Check Run '{name}'...")
    print(f"  API URL: {api_url}")
    print(f"  Commit SHA: {commit_sha}")
    print(f"  Conclusion: {conclusion}")
    print(f"  Summary length: {len(check_summary)} chars")

    payload = json.dumps({
        "name": name,
        "head_sha": commit_sha,
        "status": "completed",
        "conclusion": conclusion,
        "output": {
            "title": title,
            "summary": check_summary
        }
    }).encode('utf-8')

    headers = {
        "Authorization": f"Bearer {github_token}",
        "Accept": "application/vnd.github+json",
        "X-GitHub-Api-Version": "2022-11-28",
        "Content-Type": "application/json"
    }

    try:
        req = request.Request(api_url, data=payload, headers=headers, method='POST')
        with request.urlopen(req) as response:
            result = json.loads(response.read().decode('utf-8'))
            print(f"Check Run created successfully: {result.get('html_url', 'N/A')}")
    except HTTPError as e:
        error_body = e.read().decode('utf-8') if e.fp else ''
        print(f"::warning::Failed to create Check Run (HTTP {e.code})")
        print(f"::warning::Error: {e.reason}")
        if error_body:
            print(f"::warning::Response: {error_body}")
        print("::warning::This may be expected for PRs from forks (limited token permissions)")
    except Exception as e:
        print(f"::warning::Failed to create Check Run: {e}")
else:
    if not github_token:
        print("::warning::GITHUB_TOKEN not available, skipping Check Run creation")
    if not commit_sha:
        print("::warning::Commit SHA not available, skipping Check Run creation")

# Exit with error if tests failed and fail-on-error is set
if fail_on_error and total_failed > 0:
    print(f"::error::{total_failed} test(s) failed")
    exit(1)
