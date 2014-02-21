@echo off
setlocal
for %%a in (%*) do echo "%%a" | findstr /C:"mono">nul && set buildtool=xbuild.bat
if not defined buildtool for /f %%i in ('dir /b /ad /on "%windir%\Microsoft.NET\Framework\v*"') do @if exist "%windir%\Microsoft.NET\Framework\%%i\msbuild".exe set buildtool=%windir%\Microsoft.NET\Framework\%%i\msbuild.exe
if not defined buildtool (echo no MSBuild.exe or xbuild was found>&2 & exit /b 42)
if defined buildtool "%buildtool%" %~dp0NUnitFramework.msbuild %*