@echo off
echo ====================================
echo Starting TheGrind5 Event Management
echo ====================================
echo.

REM Start Backend
echo [1/2] Starting Backend Server...
start "Backend API" cmd /k "cd src && dotnet run"
timeout /t 3 /nobreak > nul

REM Start Frontend
echo [2/2] Starting Frontend Server...
start "Frontend React" cmd /k "cd TheGrind5_EventManagement_FrontEnd && npm start"

echo.
echo ====================================
echo Servers are starting...
echo Backend: http://localhost:5000
echo Frontend: http://localhost:3000
echo ====================================
