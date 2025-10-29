# Script to apply Feedback tables migration
Write-Host "Applying Feedback tables migration..."

# Navigate to src directory
Set-Location -Path $PSScriptRoot

# Run EF Core migration
dotnet ef database update --project src/TheGrind5_EventManagement.csproj

Write-Host "Migration completed!"
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
