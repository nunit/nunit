#!/usr/bin/env bash
# Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

# Exercise Framework binaries directly under Mono. This deliberately avoids the
# VSTest host, which no longer launches Framework tests on non-Windows systems.
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
results_root="${1:-$repo_root/TestResults/Mono}"
configuration="${CONFIGURATION:-Release}"
case "$configuration" in Debug|Release) ;; *) echo 'CONFIGURATION must be Debug or Release.' >&2; exit 2 ;; esac
for tool in dotnet mono python3; do
    command -v "$tool" > /dev/null || { echo "Required tool not found: $tool" >&2; exit 2; }
done
mkdir -p "$results_root"
results_root="$(cd "$results_root" && pwd)"
for suite in framework legacy nunitlite; do
    if [[ -e "$results_root/$suite.xml" ]]; then
        echo "Existing result must be preserved: $results_root/$suite.xml" >&2
        exit 2
    fi
done
cd "$repo_root"
dotnet --info > "$results_root/dotnet-info.txt"
mono --version > "$results_root/mono-version.txt"

# Global properties also override #5465's non-Windows default exclusions. The
# normal modern-.NET jobs retain their own complete target-framework lists.
for project in nunit.framework.legacy.tests/nunit.framework.legacy.tests.csproj nunitlite.tests/nunitlite.tests.csproj; do
    dotnet build "src/NUnitFramework/$project" --configuration "$configuration" --nologo \
        -p:NUnitLibraryFrameworks=net462 -p:NUnitRuntimeFrameworks=net462 -maxcpucount:1
done

runner="$repo_root/src/NUnitFramework/nunitlite-runner/bin/$configuration/net462/nunitlite-runner.exe"
framework="$repo_root/src/NUnitFramework/tests/bin/$configuration/net462/nunit.framework.tests.dll"
legacy="$repo_root/src/NUnitFramework/nunit.framework.legacy.tests/bin/$configuration/net462/nunit.framework.legacy.tests.dll"
lite="$repo_root/src/NUnitFramework/nunitlite.tests/bin/$configuration/net462/nunitlite.tests.exe"
for binary in "$runner" "$framework" "$legacy" "$lite"; do
    [[ -f "$binary" ]] || { echo "Required Framework binary not built: $binary" >&2; exit 2; }
done

# Run every real suite, retaining all results even if an earlier suite fails.
status=0
for suite in framework legacy nunitlite; do
    # Several tests resolve fixtures and source files relative to their own binary directory.
    case "$suite" in
        framework) binary="$framework"; command=(mono "$(dirname "$framework")/nunitlite-runner.exe" "$framework") ;;
        legacy) binary="$legacy"; command=(mono "$(dirname "$legacy")/nunitlite-runner.exe" "$legacy") ;;
        nunitlite) binary="$lite"; command=(mono "$lite") ;;
    esac
    if (cd "$(dirname "$binary")" && "${command[@]}" --workers=0 "--result=$results_root/$suite.xml") \
        > "$results_root/$suite.log" 2>&1; then
        suite_exit=0
    else
        suite_exit=$?
        status=1
    fi
    printf '%s\n' "$suite_exit" > "$results_root/$suite.exit"
    tail -n 25 "$results_root/$suite.log"
done

# A successful command with no tests, missing XML, or a failed run is not coverage.
python3 - "$results_root" <<'PY'
import json
import pathlib
import sys
import xml.etree.ElementTree as ET

root = pathlib.Path(sys.argv[1])
summary = []
for suite in ('framework', 'legacy', 'nunitlite'):
    result = ET.parse(root / (suite + '.xml')).getroot()
    total = int(result.attrib['total'])
    failed = int(result.attrib['failed'])
    code = int((root / (suite + '.exit')).read_text().strip())
    summary.append(dict(suite=suite, total=total, passed=int(result.attrib['passed']),
                        failed=failed, skipped=int(result.attrib['skipped']),
                        result=result.attrib['result'], exitCode=code))
(root / 'summary.json').write_text(json.dumps(summary, indent=2) + '\n')
print(json.dumps(summary, indent=2))
for row in summary:
    # NUnit serializes an aggregate containing ignored children as Skipped/Ignored,
    # even when thousands of cases passed. Require actual passes as well as the
    # runner's success exit and zero failures; an entirely skipped suite must fail.
    result = ET.parse(root / (row['suite'] + '.xml')).getroot()
    allowed = row['result'] in ('Passed', 'Warning') or (
        row['result'] == 'Skipped' and result.attrib.get('label') == 'Ignored')
    if row['total'] == 0 or row['passed'] == 0 or row['failed'] != 0 or row['exitCode'] != 0 or not allowed:
        raise SystemExit('Mono suite did not complete successfully: ' + row['suite'])
PY
exit "$status"
