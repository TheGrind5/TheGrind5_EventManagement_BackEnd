@echo off
start "Backend" cmd /k "cd /d %~dp0src && dotnet run --urls http://localhost:5000"
start "Frontend" cmd /k "cd /d C:\Users\ASUS\source\repos\TheGrind5_EventManagement_FrontEnd && npm start"