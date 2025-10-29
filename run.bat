@echo off
start "Backend" cmd /k "cd /d %~dp0src && dotnet run --urls http://localhost:5000"
start "Frontend" cmd /k "cd /d %~dp0TheGrind5_EventManagement_FrontEnd && npm start"