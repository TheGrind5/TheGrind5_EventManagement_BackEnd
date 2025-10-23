@echo off
echo ========================================
echo TheGrind5 Event Management System
echo Starting Backend and Frontend...
echo ========================================

start "Backend API" cmd /k "%~dp0start_backend.bat"
start "Frontend React" cmd /k "C:\Users\PHOENIX\Desktop\5GrindThe\TheGrind5_EventManagement_FrontEnd\start_frontend.bat"

echo.
echo Both services are starting...
echo Backend: http://localhost:5000
echo Frontend: http://localhost:3000
echo.
echo Press any key to exit...
pause > nul