@echo off
echo Starting TheGrind5 Event Management System...
echo.

REM Get the current directory (backend directory)
set BACKEND_DIR=%~dp0
set FRONTEND_DIR=%BACKEND_DIR%..\TheGrind5_EventManagement_FrontEnd

echo Backend Directory: %BACKEND_DIR%
echo Frontend Directory: %FRONTEND_DIR%
echo.

REM Check if frontend directory exists
if not exist "%FRONTEND_DIR%" (
    echo Error: Frontend directory not found at %FRONTEND_DIR%
    echo Please make sure the frontend project is in the correct location.
    pause
    exit /b 1
)

echo Starting Backend Server...
start "Backend Server" cmd /k "cd /d %BACKEND_DIR% && dotnet run"

echo Waiting 3 seconds for backend to start...
timeout /t 3 /nobreak > nul

echo Starting Frontend Server...
start "Frontend Server" cmd /k "cd /d %FRONTEND_DIR% && npm start"

echo.
echo Both servers are starting...
echo Backend: http://localhost:5000
echo Frontend: http://localhost:3000
echo.
echo Press any key to close this window...
pause > nul
