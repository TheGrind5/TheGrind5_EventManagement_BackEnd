# Script tự động tạo bảng Feedback
Write-Host "Starting automatic Feedback table creation..." -ForegroundColor Green

# Get connection string from appsettings.json
$appsettingsPath = "src\appsettings.json"
$appsettings = Get-Content $appsettingsPath | ConvertFrom-Json
$connectionString = $appsettings.ConnectionStrings.DefaultConnection

Write-Host "Connection String: $connectionString" -ForegroundColor Yellow

# Extract server, database from connection string
if ($connectionString -match "Server=([^;]+)") {
    $serverName = $Matches[1]
    Write-Host "Server: $serverName" -ForegroundColor Green
}

if ($connectionString -match "Database=([^;]+)") {
    $databaseName = $Matches[1]
    Write-Host "Database: $databaseName" -ForegroundColor Green
}

# Read SQL file
$sqlScript = Get-Content "CreateFeedbackTables.sql" -Raw

Write-Host "`nExecuting SQL script..." -ForegroundColor Yellow

# Execute SQL using sqlcmd
$env:SQLCMDPASSWORD = "123"  # Replace with your actual password
$sqlcmdResult = sqlcmd -S "$serverName" -d "$databaseName" -Q $sqlScript 2>&1

Write-Host "`nSQL Execution Result:" -ForegroundColor Cyan
Write-Host $sqlcmdResult

Write-Host "`nDone! Please check if tables were created successfully." -ForegroundColor Green
