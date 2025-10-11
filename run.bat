@echo off
start "Backend" cmd /k "cd /d %~dp0\src && dotnet run"
start "Frontend" cmd /k "cd /d %~dp0\..\5GrindThe\TheGrind5_EventManagement_FrontEnd && npm start"
