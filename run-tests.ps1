# run-tests.ps1 - Script to run both Frontend and Backend tests
# Usage: .\run-tests.ps1

Write-Host "🚀 Starting Unit Tests for Buy Ticket Flow..." -ForegroundColor Green
Write-Host ""

# Function to run tests with error handling
function Run-Tests {
    param(
        [string]$Name,
        [string]$Path,
        [string]$Command
    )
    
    Write-Host "📁 Running $Name tests..." -ForegroundColor Yellow
    Write-Host "Path: $Path" -ForegroundColor Gray
    Write-Host "Command: $Command" -ForegroundColor Gray
    Write-Host ""
    
    Push-Location $Path
    
    try {
        Invoke-Expression $Command
        Write-Host "✅ $Name tests completed successfully!" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ $Name tests failed!" -ForegroundColor Red
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    }
    finally {
        Pop-Location
        Write-Host ""
    }
}

# Run Backend tests
Run-Tests -Name "Backend (.NET)" -Path "TheGrind5_EventManagement.Tests" -Command "dotnet test --collect:`"XPlat Code Coverage`""

# Run Frontend tests  
Run-Tests -Name "Frontend (React)" -Path "../TheGrind5_EventManagement_FrontEnd" -Command "npm run test:coverage"

Write-Host "🎉 All tests completed!" -ForegroundColor Green
Write-Host ""
Write-Host "📊 Test Results Summary:" -ForegroundColor Cyan
Write-Host "- Backend: Check TestResults folder for coverage" -ForegroundColor White
Write-Host "- Frontend: Check src/__tests__/coverage/lcov-report/index.html" -ForegroundColor White
Write-Host ""
Write-Host "💡 To run tests individually:" -ForegroundColor Yellow
Write-Host "  Backend:  dotnet test" -ForegroundColor White
Write-Host "  Frontend: npm test" -ForegroundColor White
