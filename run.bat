@echo off
start "Backend API" cmd /k "cd /d %~dp0\src && dotnet run"
start "Frontend React" cmd /k "cd /d C:\Users\ASUS\source\repos\TheGrind5_EventManagement_FrontEnd && npm start"