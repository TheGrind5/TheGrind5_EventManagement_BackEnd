# TheGrind5 Event Management System Startup Script
Write-Host "Starting TheGrind5 Event Management System..." -ForegroundColor Green

# Kill any existing processes
Write-Host "Cleaning up existing processes..." -ForegroundColor Yellow
Get-Process -Name "node" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue

# Start Backend API
Write-Host "Starting Backend API..." -ForegroundColor Cyan
$backendPath = "C:\Users\Lenovo\OneDrive\Desktop\TheGrind5\TheGrind5_EventManagement_BackEnd\src"
Start-Process -FilePath "dotnet" -ArgumentList "run" -WorkingDirectory $backendPath -WindowStyle Normal

# Wait for Backend to start
Write-Host "Waiting for Backend to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 15

# Check if Backend is running
$backendRunning = $false
for ($i = 1; $i -le 10; $i++) {
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5000/api/events" -TimeoutSec 5 -ErrorAction Stop
        $backendRunning = $true
        Write-Host "Backend is running!" -ForegroundColor Green
        break
    }
    catch {
        Write-Host "Waiting for Backend... ($i/10)" -ForegroundColor Yellow
        Start-Sleep -Seconds 3
    }
}

if (-not $backendRunning) {
    Write-Host "Backend failed to start properly!" -ForegroundColor Red
}

# Start Frontend React
Write-Host "Starting Frontend React..." -ForegroundColor Cyan
$frontendPath = "C:\Users\Lenovo\OneDrive\Desktop\TheGrind5\TheGrind5_EventManagement_FrontEnd"
Start-Process -FilePath "npm" -ArgumentList "start" -WorkingDirectory $frontendPath -WindowStyle Normal

# Wait for Frontend to start
Write-Host "Waiting for Frontend to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 20

# Check if Frontend is running
$frontendRunning = $false
for ($i = 1; $i -le 10; $i++) {
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:3000" -TimeoutSec 5 -ErrorAction Stop
        $frontendRunning = $true
        Write-Host "Frontend is running!" -ForegroundColor Green
        break
    }
    catch {
        Write-Host "Waiting for Frontend... ($i/10)" -ForegroundColor Yellow
        Start-Sleep -Seconds 3
    }
}

if (-not $frontendRunning) {
    Write-Host "Frontend failed to start properly!" -ForegroundColor Red
}

# Open website
Write-Host "Opening website..." -ForegroundColor Cyan
Start-Process "http://localhost:3000"

Write-Host "===========================================" -ForegroundColor Green
Write-Host "Services Status:" -ForegroundColor Green
Write-Host "Backend: http://localhost:5000" -ForegroundColor White
Write-Host "Frontend: http://localhost:3000" -ForegroundColor White
Write-Host "===========================================" -ForegroundColor Green

Write-Host "Press any key to exit..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
