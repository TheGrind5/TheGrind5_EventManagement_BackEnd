start "Backend" cmd /k "cd /d %~dp0\src && dotnet run"
start "Frontend" cmd /k "cd /d %~dp0\..\TheGrind5_EventManagement_FrontEnd && npm start"

