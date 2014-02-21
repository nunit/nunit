@@@ setlocal
@@@ for /f %%i in ('dir /b /ad /on "%windir%\Microsoft.NET\Framework\v*"') do @if exist "%windir%\Microsoft.NET\Framework\%%i\msbuild".exe set msbuild=%windir%\Microsoft.NET\Framework\%%i\msbuild.exe
@@@ if not defined msbuild (echo MSBuild.exe not found>&2 & exit /b 42)
@@@ if defined msbuild "%msbuild%" NUnitFramework.msbuild %*