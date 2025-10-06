@echo off
echo Stopping TheGrind5 Event Management System...
echo.

REM Kill all dotnet processes (backend)
echo Stopping Backend Server...
taskkill /f /im dotnet.exe >nul 2>&1

REM Kill all node processes (frontend)
echo Stopping Frontend Server...
taskkill /f /im node.exe >nul 2>&1

REM Kill all cmd processes with specific titles
echo Stopping Development Windows...
taskkill /f /fi "WINDOWTITLE eq Backend Server*" >nul 2>&1
taskkill /f /fi "WINDOWTITLE eq Frontend Server*" >nul 2>&1

echo.
echo All servers have been stopped.
echo.
pause
