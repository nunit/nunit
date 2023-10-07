@echo off
set DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
set DOTNET_CLI_TELEMETRY_OPTOUT=1
set DOTNET_NOLOGO=1

dotnet tool restore
if %ERRORLEVEL% EQU 0 dotnet cake %1 %2 %3 %4 %5 %6 %7 %8 %9
