@echo off
echo Starting Backend Server...
start "Backend" cmd /k "cd /d %~dp0src && dotnet run --urls http://localhost:5000"
echo.
echo Backend started at http://localhost:5000
echo.
echo NOTE: Frontend chua co trong nhanh Minh.
echo De test Admin API, truy cap: http://localhost:5000/api/admin/statistics
pause