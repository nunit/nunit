# NUnit Test Reporter - TRX Parser
# Parses TRX files and generates GitHub summary with framework information

$ErrorActionPreference = 'Stop'

$name = $env:INPUT_NAME
$pathPattern = $env:INPUT_PATH
$failOnError = $env:INPUT_FAIL_ON_ERROR -eq 'true'
$repoUrl = $env:GITHUB_SERVER_URL + "/" + $env:GITHUB_REPOSITORY
# Use COMMIT_SHA from action (handles PR head SHA correctly) or fall back to GITHUB_SHA
$commitSha = if ($env:COMMIT_SHA) { $env:COMMIT_SHA } else { $env:GITHUB_SHA }
Write-Host "Using commit SHA: $commitSha"

# Find all TRX files
$trxFiles = Get-ChildItem -Path $pathPattern -Recurse -ErrorAction SilentlyContinue

if ($trxFiles.Count -eq 0) {
    Write-Host "::warning::No TRX files found matching pattern: $pathPattern"
    exit 0
}

Write-Host "Found $($trxFiles.Count) TRX file(s)"

# Data structures
$results = @()
$allFailedTests = @()
$totalPassed = 0
$totalFailed = 0
$totalSkipped = 0
$totalTests = 0

# Regex to extract framework from paths
$frameworkRegex = '[/\\](net\d+\.\d+|net\d+|netcoreapp\d+\.\d+|netstandard\d+\.\d+)[/\\]'

# Function to convert stack trace to markdown with source links (as bullet list, NOT in code blocks)
function Convert-StackTraceToMarkdown {
    param([string]$stackTrace, [string]$repoUrl, [string]$sha)

    if ([string]::IsNullOrWhiteSpace($stackTrace)) {
        return ""
    }

    $lines = $stackTrace -split "`n"
    $result = @()

    foreach ($line in $lines) {
        $trimmedLine = $line.Trim()
        if ([string]::IsNullOrWhiteSpace($trimmedLine)) {
            continue
        }

        # Match patterns like "at Method() in C:\path\file.cs:line 123"
        if ($trimmedLine -match 'at (.+?) in (.+?):line (\d+)') {
            $methodCall = $Matches[1]
            $filePath = $Matches[2]
            $lineNum = $Matches[3]

            # Try to extract relative path from src/
            if ($filePath -match '[/\\](src[/\\].+)$') {
                $relativePath = $Matches[1] -replace '\\', '/'
                $sourceLink = "$repoUrl/blob/$sha/$relativePath#L$lineNum"
                # Format: bullet with method in backticks, then clickable link
                $result += "| ``$methodCall`` | [$relativePath#L$lineNum]($sourceLink) |"
            } else {
                $fileName = [System.IO.Path]::GetFileName($filePath)
                $result += "| ``$methodCall`` | $fileName`:$lineNum |"
            }
        } elseif ($trimmedLine -match 'at (.+)') {
            $methodCall = $Matches[1]
            $result += "| ``$methodCall`` | (no source) |"
        }
    }

    if ($result.Count -gt 0) {
        # Return as a table
        $header = "| Method | Source |`n|:-------|:-------|"
        return $header + "`n" + ($result -join "`n")
    }
    return ""
}

foreach ($trxFile in $trxFiles) {
    Write-Host "Processing: $($trxFile.Name)"

    try {
        # Use -LiteralPath to handle filenames with brackets like [1]
        [xml]$xml = Get-Content -LiteralPath $trxFile.FullName
        $ns = @{ t = $xml.DocumentElement.NamespaceURI }

        # Get counters
        $counters = Select-Xml -Xml $xml -XPath "//t:Counters" -Namespace $ns | Select-Object -First 1
        if ($null -eq $counters) {
            Write-Host "  No counters found, skipping"
            continue
        }

        $passed = [int]$counters.Node.passed
        $failed = [int]$counters.Node.failed
        $total = [int]$counters.Node.total
        $executed = [int]$counters.Node.executed
        $skipped = $total - $executed

        # Extract framework and project from test assembly path
        $framework = "unknown"
        $project = "unknown"

        $unitTests = Select-Xml -Xml $xml -XPath "//t:UnitTest" -Namespace $ns | Select-Object -First 1
        if ($null -ne $unitTests) {
            $storage = $unitTests.Node.storage
            if ($storage) {
                # Extract framework
                if ($storage -match $frameworkRegex) {
                    $framework = $Matches[1].ToLower()
                }
                # Extract project name from DLL name
                $dllName = [System.IO.Path]::GetFileNameWithoutExtension($storage)
                if ($dllName) {
                    $project = $dllName
                }
            }
        }

        # Collect failed tests with ALL available information
        $failedTestResults = Select-Xml -Xml $xml -XPath "//t:UnitTestResult[@outcome='Failed']" -Namespace $ns
        foreach ($failedTest in $failedTestResults) {
            $node = $failedTest.Node
            $testName = $node.testName
            $testId = $node.testId
            $duration = $node.duration
            $startTime = $node.startTime
            $endTime = $node.endTime
            $computerName = $node.computerName

            $errorMessage = ""
            $stackTrace = ""
            $stdOut = ""
            $stdErr = ""
            $debugTrace = ""

            $output = $node.Output
            if ($output) {
                $errorInfo = $output.ErrorInfo
                if ($errorInfo) {
                    $errorMessage = $errorInfo.Message
                    $stackTrace = $errorInfo.StackTrace
                }
                $stdOut = $output.StdOut
                $stdErr = $output.StdErr
                $debugTrace = $output.DebugTrace
            }

            # Try to get the test method info for source location
            $testMethod = Select-Xml -Xml $xml -XPath "//t:UnitTest[@id='$testId']/t:TestMethod" -Namespace $ns | Select-Object -First 1
            $className = ""
            $methodName = ""
            $codeBase = ""
            if ($null -ne $testMethod) {
                $className = $testMethod.Node.className
                $methodName = $testMethod.Node.name
                $codeBase = $testMethod.Node.codeBase
            }

            $allFailedTests += @{
                Name = $testName
                Framework = $framework
                Project = $project
                ErrorMessage = $errorMessage
                StackTrace = $stackTrace
                StdOut = $stdOut
                StdErr = $stdErr
                DebugTrace = $debugTrace
                Duration = $duration
                StartTime = $startTime
                EndTime = $endTime
                ComputerName = $computerName
                ClassName = $className
                MethodName = $methodName
                CodeBase = $codeBase
            }
        }

        $results += @{
            Project = $project
            Framework = $framework
            Passed = $passed
            Failed = $failed
            Skipped = $skipped
            Total = $total
            Duration = $counters.Node.GetAttribute("duration")
        }

        $totalPassed += $passed
        $totalFailed += $failed
        $totalSkipped += $skipped
        $totalTests += $total

    } catch {
        Write-Host "::warning::Failed to parse $($trxFile.Name): $_"
    }
}

# Group results by project and framework
$groupedResults = $results | Group-Object { "$($_.Project)|$($_.Framework)" } | ForEach-Object {
    $items = $_.Group
    @{
        Project = $items[0].Project
        Framework = $items[0].Framework
        Passed = ($items | Measure-Object -Property Passed -Sum).Sum
        Failed = ($items | Measure-Object -Property Failed -Sum).Sum
        Skipped = ($items | Measure-Object -Property Skipped -Sum).Sum
        Total = ($items | Measure-Object -Property Total -Sum).Sum
    }
} | Sort-Object { $_.Project }, { $_.Framework }

# Determine overall status
$overallStatus = if ($totalFailed -gt 0) { "failure" } else { "success" }
$statusIcon = if ($totalFailed -gt 0) { ":x:" } else { ":white_check_mark:" }
$statusBadge = if ($totalFailed -gt 0) { "![Failed](https://img.shields.io/badge/tests-failed-red)" } else { "![Passed](https://img.shields.io/badge/tests-passed-brightgreen)" }

# Build markdown summary
$summary = @"
# $statusIcon $name

$statusBadge

## Summary

| Total | Passed | Failed | Skipped |
|------:|-------:|-------:|--------:|
| $totalTests | $totalPassed | $totalFailed | $totalSkipped |

## Results by Project and Framework

| Project | Framework | Passed | Failed | Skipped | Status |
|:--------|:----------|-------:|-------:|--------:|:------:|
"@

foreach ($r in $groupedResults) {
    $icon = if ($r.Failed -gt 0) { ":x:" } else { ":white_check_mark:" }
    $summary += "`n| $($r.Project) | $($r.Framework) | $($r.Passed) | $($r.Failed) | $($r.Skipped) | $icon |"
}

# Add failed tests section if any
if ($allFailedTests.Count -gt 0) {
    $summary += @"

## :x: Failed Tests ($($allFailedTests.Count))

"@

    foreach ($test in $allFailedTests) {
        $shortName = $test.Name
        if ($shortName.Length -gt 100) {
            $shortName = $shortName.Substring(0, 97) + "..."
        }

        $summary += @"

### :x: $shortName

| Property | Value |
|:---------|:------|
| **Project** | $($test.Project) |
| **Framework** | $($test.Framework) |
| **Class** | ``$($test.ClassName)`` |
| **Method** | ``$($test.MethodName)`` |
| **Duration** | $($test.Duration) |

"@

        # Error Message
        if ($test.ErrorMessage) {
            $summary += @"

**Error Message:**
``````
$($test.ErrorMessage)
``````

"@
        }

        # Stack Trace with source links (as list, not code block, so links are clickable)
        if ($test.StackTrace) {
            $linkedStackTrace = Convert-StackTraceToMarkdown -stackTrace $test.StackTrace -repoUrl $repoUrl -sha $commitSha
            $summary += @"

**Stack Trace:**

$linkedStackTrace

"@
        }

        # Standard Output
        if ($test.StdOut) {
            $summary += @"

<details>
<summary>Standard Output</summary>

``````
$($test.StdOut)
``````

</details>

"@
        }

        # Standard Error
        if ($test.StdErr) {
            $summary += @"

<details>
<summary>Standard Error</summary>

``````
$($test.StdErr)
``````

</details>

"@
        }

        # Debug Trace
        if ($test.DebugTrace) {
            $summary += @"

<details>
<summary>Debug Trace</summary>

``````
$($test.DebugTrace)
``````

</details>

"@
        }

        $summary += "`n---`n"
    }
}

# Write to GitHub Step Summary
$summary | Out-File -FilePath $env:GITHUB_STEP_SUMMARY -Encoding utf8 -Append

# Set outputs
"passed=$totalPassed" | Out-File -FilePath $env:GITHUB_OUTPUT -Encoding utf8 -Append
"failed=$totalFailed" | Out-File -FilePath $env:GITHUB_OUTPUT -Encoding utf8 -Append
"skipped=$totalSkipped" | Out-File -FilePath $env:GITHUB_OUTPUT -Encoding utf8 -Append

Write-Host ""
Write-Host "========================================"
Write-Host "  Test Results: $totalTests total"
Write-Host "  Passed: $totalPassed"
Write-Host "  Failed: $totalFailed"
Write-Host "  Skipped: $totalSkipped"
Write-Host "========================================"

# Create GitHub Check Run via API
$conclusion = if ($totalFailed -gt 0) { "failure" } else { "success" }
$title = "$totalTests tests: $totalPassed passed, $totalFailed failed, $totalSkipped skipped"

# Truncate summary if too long for Check Run (max 65535 chars)
$checkSummary = $summary
if ($checkSummary.Length -gt 65000) {
    $checkSummary = $checkSummary.Substring(0, 65000) + "`n`n... (truncated, see Job Summary for full details)"
}

# Only create check run if we have a token
if ($env:GITHUB_TOKEN) {
    $apiUrl = "https://api.github.com/repos/$($env:GITHUB_REPOSITORY)/check-runs"

    Write-Host "Creating Check Run '$name'..."
    Write-Host "  API URL: $apiUrl"
    Write-Host "  Commit SHA: $commitSha"
    Write-Host "  Conclusion: $conclusion"
    Write-Host "  Summary length: $($checkSummary.Length) chars"
    Write-Host "  Title: $title"

    # Build the check run payload
    $checkRunPayload = @{
        name = $name
        head_sha = $commitSha
        status = "completed"
        conclusion = $conclusion
        output = @{
            title = $title
            summary = $checkSummary
        }
    } | ConvertTo-Json -Depth 10

    $headers = @{
        "Authorization" = "Bearer $($env:GITHUB_TOKEN)"
        "Accept" = "application/vnd.github+json"
        "X-GitHub-Api-Version" = "2022-11-28"
    }

    try {
        $response = Invoke-RestMethod -Uri $apiUrl -Method Post -Headers $headers -Body $checkRunPayload -ContentType "application/json; charset=utf-8"
        Write-Host "Check Run created successfully: $($response.html_url)"
    } catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        $errorBody = $_.ErrorDetails.Message
        Write-Host "::warning::Failed to create Check Run (HTTP $statusCode)"
        Write-Host "::warning::Error: $_"
        if ($errorBody) {
            Write-Host "::warning::Response: $errorBody"
        }
        Write-Host "::warning::This may be expected for PRs from forks (limited token permissions)"
    }
} else {
    Write-Host "::warning::GITHUB_TOKEN not available, skipping Check Run creation"
}

# Exit with error if tests failed and fail-on-error is set
if ($failOnError -and $totalFailed -gt 0) {
    Write-Host "::error::$totalFailed test(s) failed"
    exit 1
}
