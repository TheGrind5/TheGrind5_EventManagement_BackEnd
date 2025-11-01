@echo off
setlocal enabledelayedexpansion

echo ========================================
echo   VNPay Auto Setup Script
echo ========================================
echo.

REM 1. Start ngrok
echo [1/5] Starting ngrok...
start "ngrok" cmd /k "cd /d %~dp0 && .\ngrok\ngrok.exe http 5000"
timeout /t 6 /nobreak >nul

REM 2. Get ngrok URL and update appsettings.json
echo [2/5] Getting ngrok URL and updating config...
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "$maxRetries = 10; $retry = 0; $url = ''; " ^
    "while ($retry -lt $maxRetries) { " ^
    "  try { " ^
    "    $resp = Invoke-RestMethod -Uri 'http://localhost:4040/api/tunnels' -ErrorAction Stop; " ^
    "    if ($resp.tunnels -and $resp.tunnels.Length -gt 0) { " ^
    "      $url = $resp.tunnels[0].public_url; " ^
    "      echo 'Found: ' + $url; break; " ^
    "    } " ^
    "  } catch { Start-Sleep -Seconds 2; $retry++; } " ^
    "}; " ^
    "if ($url) { " ^
    "  $json = Get-Content '%~dp0src\appsettings.json' -Raw | ConvertFrom-Json; " ^
    "  $json.VNPay.IpnUrl = $url + '/api/Payment/vnpay/webhook'; " ^
    "  $json | ConvertTo-Json -Depth 10 | Set-Content '%~dp0src\appsettings.json' -Encoding UTF8; " ^
    "  echo 'Config updated!'; " ^
    "} else { echo 'ERROR: Could not get ngrok URL' }"

if !ERRORLEVEL! NEQ 0 (
    echo ERROR: Failed to update config
    pause
    exit /b 1
)

REM 3. Start backend
echo [3/5] Starting backend...
start "Backend API" cmd /k "cd /d %~dp0src && dotnet run"
timeout /t 3 /nobreak >nul

REM 4. Start frontend
echo [4/5] Starting frontend...
start "Frontend React" cmd /k "cd /d %~dp0..\TheGrind5_EventManagement_FrontEnd && npm start"
timeout /t 3 /nobreak >nul

REM 5. Done
echo [5/5] All services started!
echo.
echo ========================================
echo   Status
echo ========================================
echo ngrok:      https://*.ngrok-free.app
echo Backend:    http://localhost:5000
echo Frontend:   http://localhost:3000
echo ngrok UI:   http://localhost:4040
echo ========================================
echo.
echo All done! Press any key to exit...
pause >nul
