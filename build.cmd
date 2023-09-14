@echo off
powershell -ExecutionPolicy ByPass -NoProfile ./build.ps1 %CAKE_ARGS% %*
