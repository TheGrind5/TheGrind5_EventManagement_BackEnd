@echo off
setlocal enabledelayedexpansion

REM ========================================
REM TheGrind5 Event Management - Dev Manager
REM Single file to manage everything
REM ========================================

:MAIN_MENU
cls
echo.
echo ========================================
echo   TheGrind5 Event Management System
echo   Development Manager
echo ========================================
echo.
echo 1. Start Development Environment
echo 2. Stop All Services
echo 3. Check Status
echo 4. Restart Services
echo 5. Quick Start (Auto)
echo 6. Exit
echo.
set /p choice="Please select an option (1-6): "

if "%choice%"=="1" goto START_SERVICES
if "%choice%"=="2" goto STOP_SERVICES
if "%choice%"=="3" goto CHECK_STATUS
if "%choice%"=="4" goto RESTART_SERVICES
if "%choice%"=="5" goto QUICK_START
if "%choice%"=="6" goto EXIT
echo Invalid choice. Please try again.
pause
goto MAIN_MENU

:START_SERVICES
cls
echo.
echo ========================================
echo   Starting Development Environment
echo ========================================
echo.

REM Get the current directory (backend directory)
set BACKEND_DIR=%~dp0
set FRONTEND_DIR=%BACKEND_DIR%..\TheGrind5_EventManagement_FrontEnd

echo Backend Directory: %BACKEND_DIR%
echo Frontend Directory: %FRONTEND_DIR%
echo.

REM Check if frontend directory exists
if not exist "%FRONTEND_DIR%" (
    echo [ERROR] Frontend directory not found at %FRONTEND_DIR%
    echo Please make sure the frontend project is in the correct location.
    pause
    goto MAIN_MENU
)

REM Kill any existing processes first
echo [STEP 1] Cleaning up existing processes...
call :KILL_PROCESSES

echo.
echo [STEP 2] Starting Backend Server...
start "TheGrind5-Backend" cmd /k "cd /d %BACKEND_DIR% && echo Starting Backend Server... && dotnet run"

echo Waiting 5 seconds for backend to initialize...
timeout /t 5 /nobreak > nul

echo.
echo [STEP 3] Starting Frontend Server...
start "TheGrind5-Frontend" cmd /k "cd /d %FRONTEND_DIR% && echo Starting Frontend Server... && npm start"

echo.
echo [SUCCESS] Development environment is starting...
echo.
echo Backend API: http://localhost:5000/api/Event
echo Backend Swagger: http://localhost:5000/swagger
echo Frontend: http://localhost:3000
echo.
echo Both servers are running in separate windows.
echo Close those windows to stop the servers.
echo.
pause
goto MAIN_MENU

:QUICK_START
cls
echo.
echo ========================================
echo   Quick Start - Auto Mode
echo ========================================
echo.

REM Get the current directory (backend directory)
set BACKEND_DIR=%~dp0
set FRONTEND_DIR=%BACKEND_DIR%..\TheGrind5_EventManagement_FrontEnd

echo [INFO] Backend Directory: %BACKEND_DIR%
echo [INFO] Frontend Directory: %FRONTEND_DIR%
echo.

REM Kill any existing processes
echo [STEP 1] Cleaning up existing processes...
call :KILL_PROCESSES

echo [STEP 2] Starting Backend Server...
start "TheGrind5-Backend" cmd /k "cd /d %BACKEND_DIR% && echo Starting Backend Server... && dotnet run"

echo [STEP 3] Waiting for backend to initialize...
timeout /t 5 /nobreak > nul

echo [STEP 4] Starting Frontend Server...
start "TheGrind5-Frontend" cmd /k "cd /d %FRONTEND_DIR% && echo Starting Frontend Server... && npm start"

echo.
echo [SUCCESS] Development environment started!
echo.
echo Backend API: http://localhost:5000/api/Event
echo Backend Swagger: http://localhost:5000/swagger
echo Frontend: http://localhost:3000
echo.
echo Press any key to close this window...
pause > nul
exit /b 0

:STOP_SERVICES
cls
echo.
echo ========================================
echo   Stopping All Services
echo ========================================
echo.

call :KILL_PROCESSES

echo.
echo [SUCCESS] All services have been stopped.
echo.
pause
goto MAIN_MENU

:CHECK_STATUS
cls
echo.
echo ========================================
echo   System Status Check
echo ========================================
echo.

echo [CHECKING] Backend Server (Port 5000):
netstat -an | findstr ":5000" >nul
if %errorlevel% == 0 (
    echo [RUNNING] Backend is active on port 5000
    for /f "tokens=5" %%a in ('netstat -an ^| findstr ":5000"') do (
        echo   Process ID: %%a
    )
) else (
    echo [STOPPED] Backend is not running
)

echo.
echo [CHECKING] Frontend Server (Port 3000):
netstat -an | findstr ":3000" >nul
if %errorlevel% == 0 (
    echo [RUNNING] Frontend is active on port 3000
    for /f "tokens=5" %%a in ('netstat -an ^| findstr ":3000"') do (
        echo   Process ID: %%a
    )
) else (
    echo [STOPPED] Frontend is not running
)

echo.
echo [CHECKING] Development Windows:
tasklist /fi "WINDOWTITLE eq TheGrind5-Backend*" 2>nul | findstr "cmd.exe" >nul
if %errorlevel% == 0 (
    echo [RUNNING] Backend development window is open
) else (
    echo [CLOSED] Backend development window is closed
)

tasklist /fi "WINDOWTITLE eq TheGrind5-Frontend*" 2>nul | findstr "cmd.exe" >nul
if %errorlevel% == 0 (
    echo [RUNNING] Frontend development window is open
) else (
    echo [CLOSED] Frontend development window is closed
)

echo.
echo [URLS]
echo Backend API: http://localhost:5000/api/Event
echo Backend Swagger: http://localhost:5000/swagger
echo Frontend: http://localhost:3000
echo.
pause
goto MAIN_MENU

:RESTART_SERVICES
cls
echo.
echo ========================================
echo   Restarting Services
echo ========================================
echo.

echo [STEP 1] Stopping all services...
call :KILL_PROCESSES

echo.
echo [STEP 2] Waiting 3 seconds...
timeout /t 3 /nobreak > nul

echo.
echo [STEP 3] Starting services...
goto START_SERVICES

:KILL_PROCESSES
echo   - Force killing all development processes...

REM Kill by process name
echo   - Killing dotnet processes (Backend)...
taskkill /f /im dotnet.exe >nul 2>&1
taskkill /f /im TheGrind5_EventManagement.exe >nul 2>&1

echo   - Killing node processes (Frontend)...
taskkill /f /im node.exe >nul 2>&1
taskkill /f /im npm.exe >nul 2>&1

echo   - Closing development windows...
taskkill /f /fi "WINDOWTITLE eq TheGrind5-Backend*" >nul 2>&1
taskkill /f /fi "WINDOWTITLE eq TheGrind5-Frontend*" >nul 2>&1
taskkill /f /fi "WINDOWTITLE eq Backend Server*" >nul 2>&1
taskkill /f /fi "WINDOWTITLE eq Frontend Server*" >nul 2>&1
taskkill /f /fi "WINDOWTITLE eq *dotnet*" >nul 2>&1
taskkill /f /fi "WINDOWTITLE eq *node*" >nul 2>&1

REM Kill any cmd processes running our apps
echo   - Killing cmd processes running development apps...
for /f "tokens=2" %%a in ('tasklist /fi "imagename eq cmd.exe" /fo csv ^| findstr /v "Image Name"') do (
    set "pid=%%a"
    set "pid=!pid:"=!"
    for /f "tokens=*" %%b in ('wmic process where "ProcessId=!pid!" get CommandLine /format:list ^| findstr "dotnet\|node\|npm\|TheGrind5"') do (
        taskkill /f /pid !pid! >nul 2>&1
    )
)

REM Force kill by port - more aggressive approach
echo   - Force cleaning up ports...
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":5000"') do (
    if not "%%a"=="0" (
        taskkill /f /pid %%a >nul 2>&1
        echo     Killed process %%a on port 5000
    )
)

for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":3000"') do (
    if not "%%a"=="0" (
        taskkill /f /pid %%a >nul 2>&1
        echo     Killed process %%a on port 3000
    )
)

for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":7093"') do (
    if not "%%a"=="0" (
        taskkill /f /pid %%a >nul 2>&1
        echo     Killed process %%a on port 7093
    )
)

for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":5231"') do (
    if not "%%a"=="0" (
        taskkill /f /pid %%a >nul 2>&1
        echo     Killed process %%a on port 5231
    )
)

REM Additional PowerShell cleanup for stubborn processes
echo   - Running PowerShell cleanup...
powershell -Command "Get-Process -Name 'dotnet','node','npm' -ErrorAction SilentlyContinue | Stop-Process -Force" >nul 2>&1

echo   - Waiting for processes to terminate...
timeout /t 3 /nobreak >nul

REM Final verification
echo   - Verifying ports are free...
set "ports_free=true"
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":5000"') do set "ports_free=false"
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":3000"') do set "ports_free=false"

if "%ports_free%"=="true" (
    echo   - All ports are now free
) else (
    echo   - Warning: Some ports may still be in use
)

echo   - Force cleanup completed.
goto :eof

:EXIT
cls
echo.
echo ========================================
echo   Goodbye!
echo ========================================
echo.
echo Thank you for using TheGrind5 Dev Manager.
echo.
timeout /t 2 /nobreak > nul
exit /b 0
