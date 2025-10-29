# ========================================
# SCRIPT AUTO EXPORT - SIMPLIFIED VERSION
# ========================================
# Chay: .\auto_export_all_simple.ps1
# ========================================

param(
    [string]$ServerName = ".\SQLEXPRESS",
    [string]$DatabaseName = "EventDB",
    [string]$OutputFile = "ExtendedSampleData_Insert.sql"
)

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  AUTO EXPORT SAMPLE DATA" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# STEP 1: Connect database
Write-Host "[STEP 1/5] Connecting to database..." -ForegroundColor Yellow

try {
    $connectionString = "Server=$ServerName;Database=$DatabaseName;Integrated Security=True;TrustServerCertificate=True;"
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    Write-Host "  [OK] Connected to: $DatabaseName" -ForegroundColor Green
    
    # Mappings
    $imageMapping = @{}
    $avatarMapping = @{}
    
    # Get events
    $eventCmd = $connection.CreateCommand()
    $eventCmd.CommandText = "SELECT EventId, Title, EventDetails FROM Event ORDER BY EventId"
    $eventReader = $eventCmd.ExecuteReader()
    
    $eventCount = 0
    while ($eventReader.Read()) {
        $eventId = $eventReader["EventId"]
        $title = $eventReader["Title"]
        $eventDetails = $eventReader["EventDetails"]
        
        # Create simple slug
        $slug = $title -replace '[^a-zA-Z0-9\s]', ''
        $slug = $slug -replace '\s+', '_'
        $slug = $slug.ToLower()
        if ($slug.Length -gt 20) {
            $slug = $slug.Substring(0, 20)
        }
        
        # Parse JSON for images
        if ($eventDetails -ne [DBNull]::Value -and $eventDetails -ne $null) {
            try {
                $details = $eventDetails | ConvertFrom-Json
                
                if ($details.images -and $details.images.Count -gt 0) {
                    $imageIndex = 1
                    foreach ($imageUrl in $details.images) {
                        if ($imageUrl -match '/assets/images/events/(.+)$') {
                            $guidFileName = $Matches[1]
                            $ext = [System.IO.Path]::GetExtension($guidFileName)
                            $friendlyName = "event_" + $eventId + "_" + $slug + "_" + $imageIndex + $ext
                            $imageMapping[$guidFileName] = $friendlyName
                            
                            Write-Host "  - Event $eventId : $guidFileName -> $friendlyName" -ForegroundColor Cyan
                            $imageIndex++
                        }
                    }
                }
                
                # Legacy fields
                if ($details.EventImage) {
                    if ($details.EventImage -match '/assets/images/events/(.+)$') {
                        $guidFileName = $Matches[1]
                        $ext = [System.IO.Path]::GetExtension($guidFileName)
                        $friendlyName = "event_" + $eventId + "_main" + $ext
                        $imageMapping[$guidFileName] = $friendlyName
                        Write-Host "  - Event $eventId : $guidFileName -> $friendlyName" -ForegroundColor Cyan
                    }
                }
                
                if ($details.BackgroundImage) {
                    if ($details.BackgroundImage -match '/assets/images/events/(.+)$') {
                        $guidFileName = $Matches[1]
                        $ext = [System.IO.Path]::GetExtension($guidFileName)
                        $friendlyName = "event_" + $eventId + "_bg" + $ext
                        $imageMapping[$guidFileName] = $friendlyName
                        Write-Host "  - Event $eventId : $guidFileName -> $friendlyName" -ForegroundColor Cyan
                    }
                }
            } catch {
                Write-Host "  ! Warning: Cannot parse JSON for Event $eventId" -ForegroundColor Yellow
            }
        }
        
        $eventCount++
    }
    $eventReader.Close()
    
    Write-Host ""
    Write-Host "  [OK] Found $eventCount events, $($imageMapping.Count) images" -ForegroundColor Green
    
    # Get avatars
    $userCmd = $connection.CreateCommand()
    $userCmd.CommandText = "SELECT UserId, Username, Avatar FROM [User] WHERE Avatar IS NOT NULL ORDER BY UserId"
    $userReader = $userCmd.ExecuteReader()
    
    while ($userReader.Read()) {
        $userId = $userReader["UserId"]
        $username = $userReader["Username"]
        $avatar = $userReader["Avatar"]
        
        if ($avatar -match '/assets/images/avatars/(.+)$') {
            $guidFileName = $Matches[1]
            $ext = [System.IO.Path]::GetExtension($guidFileName)
            $friendlyName = "user_" + $userId + $ext
            $avatarMapping[$guidFileName] = $friendlyName
            Write-Host "  - User $userId : $guidFileName -> $friendlyName" -ForegroundColor Cyan
        }
    }
    $userReader.Close()
    
    Write-Host "  [OK] Found $($avatarMapping.Count) avatars" -ForegroundColor Green
    
    # STEP 2: Rename files (nếu tìm thấy)
    Write-Host ""
    Write-Host "[STEP 2/5] Looking for image files..." -ForegroundColor Yellow
    
    $assetsEventsFolder = "assets\images\events\"
    $assetsAvatarsFolder = "assets\images\avatars\"
    $wwwrootEventsFolder = "src\wwwroot\uploads\events\"
    $wwwrootAvatarsFolder = "src\wwwroot\uploads\avatars\"
    
    # Đảm bảo thư mục assets tồn tại
    if (-not (Test-Path $assetsEventsFolder)) {
        New-Item -ItemType Directory -Path $assetsEventsFolder -Force | Out-Null
        Write-Host "  - Created directory: $assetsEventsFolder" -ForegroundColor Cyan
    }
    if (-not (Test-Path $assetsAvatarsFolder)) {
        New-Item -ItemType Directory -Path $assetsAvatarsFolder -Force | Out-Null
        Write-Host "  - Created directory: $assetsAvatarsFolder" -ForegroundColor Cyan
    }
    
    $renamedCount = 0
    $notFoundCount = 0
    
    # Tạo list keys để tránh lỗi "collection modified"
    $imageKeysToRemove = @()
    $avatarKeysToRemove = @()
    
    # Tìm và rename event images
    $imageKeys = @($imageMapping.Keys)
    foreach ($guid in $imageKeys) {
        $friendlyName = $imageMapping[$guid]
        $newPath = Join-Path $assetsEventsFolder $friendlyName
        
        # Tìm file ở nhiều nơi
        $foundPath = $null
        
        # 1. Thử assets/images/events/
        $testPath = Join-Path $assetsEventsFolder $guid
        if (Test-Path $testPath) {
            $foundPath = $testPath
        }
        # 2. Thử wwwroot/uploads/events/
        else {
            $testPath = Join-Path $wwwrootEventsFolder $guid
            if (Test-Path $testPath) {
                $foundPath = $testPath
            }
        }
        # 3. Tìm trong toàn bộ thư mục bằng tên file chính xác
        if (-not $foundPath) {
            $found = Get-ChildItem -Path "." -Recurse -Filter $guid -ErrorAction SilentlyContinue | Select-Object -First 1
            if ($found) {
                $foundPath = $found.FullName
                Write-Host "  - Found in: $($found.DirectoryName)" -ForegroundColor Cyan
            }
        }
        # 4. Tìm bằng pattern nếu không tìm thấy (có thể tên hơi khác)
        if (-not $foundPath) {
            # Tách GUID ra (bỏ extension)
            $guidBase = [System.IO.Path]::GetFileNameWithoutExtension($guid)
            $ext = [System.IO.Path]::GetExtension($guid)
            
            # Tìm file có chứa GUID trong tên
            $found = Get-ChildItem -Path "src\wwwroot\uploads\events\" -ErrorAction SilentlyContinue | Where-Object { 
                $_.Name -like "*$guidBase*" 
            } | Select-Object -First 1
            
            if ($found) {
                $foundPath = $found.FullName
                Write-Host "  - Found similar file: $($found.Name)" -ForegroundColor Cyan
            }
        }
        
        if ($foundPath) {
            if (-not (Test-Path $newPath)) {
                Copy-Item -Path $foundPath -Destination $newPath -Force
                Write-Host "  [OK] $guid -> $friendlyName" -ForegroundColor Green
                $renamedCount++
            } else {
                Write-Host "  ! Skip (exists): $friendlyName" -ForegroundColor Yellow
            }
        } else {
            Write-Host "  ! Not found: $guid (will use GUID in SQL)" -ForegroundColor Yellow
            $notFoundCount++
            # Lưu lại để xóa sau
            $imageKeysToRemove += $guid
        }
    }
    
    # Xóa keys không tìm thấy
    foreach ($guid in $imageKeysToRemove) {
        $imageMapping.Remove($guid)
    }
    
    # Tìm và rename avatars
    $avatarKeys = @($avatarMapping.Keys)
    foreach ($guid in $avatarKeys) {
        $friendlyName = $avatarMapping[$guid]
        $newPath = Join-Path $assetsAvatarsFolder $friendlyName
        
        $foundPath = $null
        
        # 1. Thử assets/images/avatars/
        $testPath = Join-Path $assetsAvatarsFolder $guid
        if (Test-Path $testPath) {
            $foundPath = $testPath
        }
        # 2. Thử wwwroot/uploads/avatars/
        else {
            $testPath = Join-Path $wwwrootAvatarsFolder $guid
            if (Test-Path $testPath) {
                $foundPath = $testPath
            }
        }
        # 3. Tìm trong toàn bộ thư mục
        if (-not $foundPath) {
            $found = Get-ChildItem -Path "." -Recurse -Filter $guid -ErrorAction SilentlyContinue | Select-Object -First 1
            if ($found) {
                $foundPath = $found.FullName
            }
        }
        
        if ($foundPath) {
            if (-not (Test-Path $newPath)) {
                Copy-Item -Path $foundPath -Destination $newPath -Force
                Write-Host "  [OK] $guid -> $friendlyName" -ForegroundColor Green
                $renamedCount++
            }
        } else {
            Write-Host "  ! Not found: $guid (will use GUID in SQL)" -ForegroundColor Yellow
            # Lưu lại để xóa sau
            $avatarKeysToRemove += $guid
        }
    }
    
    # Xóa keys không tìm thấy
    foreach ($guid in $avatarKeysToRemove) {
        $avatarMapping.Remove($guid)
    }
    
    Write-Host ""
    Write-Host "  [OK] Processed $renamedCount files" -ForegroundColor Green
    if ($notFoundCount -gt 0) {
        Write-Host "  [!] $notFoundCount files not found (will use original GUID in SQL)" -ForegroundColor Yellow
        Write-Host "     Note: SQL will still work, just with GUID filenames" -ForegroundColor Gray
    }
    
    # STEP 3: Export database
    Write-Host ""
    Write-Host "[STEP 3/5] Exporting database..." -ForegroundColor Yellow
    
    $exportSQL = "-- TheGrind5 Extended Sample Data`r`n"
    $exportSQL += "-- Auto-generated: " + (Get-Date -Format "yyyy-MM-dd HH:mm:ss") + "`r`n"
    $exportSQL += "-- Events: $eventCount | Images: $($imageMapping.Count)`r`n"
    $exportSQL += "`r`nUSE EventDB;`r`nGO`r`n`r`n"
    
    # Clear data
    $exportSQL += "-- Clear existing data`r`n"
    $exportSQL += "DELETE FROM Payment;`r`n"
    $exportSQL += "DELETE FROM Ticket;`r`n"
    $exportSQL += "DELETE FROM OrderItem;`r`n"
    $exportSQL += "DELETE FROM [Order];`r`n"
    $exportSQL += "DELETE FROM TicketType;`r`n"
    $exportSQL += "DELETE FROM Event;`r`n"
    $exportSQL += "DELETE FROM Voucher;`r`n"
    $exportSQL += "DELETE FROM Wishlist;`r`n"
    $exportSQL += "DELETE FROM WalletTransaction;`r`n"
    $exportSQL += "DELETE FROM OtpCode;`r`n"
    $exportSQL += "DELETE FROM [User];`r`n`r`n"
    
    # Reset identity
    $exportSQL += "DBCC CHECKIDENT ('Payment', RESEED, 0);`r`n"
    $exportSQL += "DBCC CHECKIDENT ('Ticket', RESEED, 0);`r`n"
    $exportSQL += "DBCC CHECKIDENT ('OrderItem', RESEED, 0);`r`n"
    $exportSQL += "DBCC CHECKIDENT ('[Order]', RESEED, 0);`r`n"
    $exportSQL += "DBCC CHECKIDENT ('TicketType', RESEED, 0);`r`n"
    $exportSQL += "DBCC CHECKIDENT ('Event', RESEED, 0);`r`n"
    $exportSQL += "DBCC CHECKIDENT ('Voucher', RESEED, 0);`r`n"
    $exportSQL += "DBCC CHECKIDENT ('Wishlist', RESEED, 0);`r`n"
    $exportSQL += "DBCC CHECKIDENT ('WalletTransaction', RESEED, 0);`r`n"
    $exportSQL += "DBCC CHECKIDENT ('OtpCode', RESEED, 0);`r`n"
    $exportSQL += "DBCC CHECKIDENT ('[User]', RESEED, 0);`r`n`r`n"
    
    # Export Users
    $exportSQL += "-- Insert Users`r`nSET IDENTITY_INSERT [User] ON;`r`n`r`n"
    
    $userCmd2 = $connection.CreateCommand()
    $userCmd2.CommandText = "SELECT * FROM [User] ORDER BY UserId"
    $userReader2 = $userCmd2.ExecuteReader()
    
    while ($userReader2.Read()) {
        $userId = $userReader2["UserId"]
        $username = $userReader2["Username"]
        $fullName = ($userReader2["FullName"] -replace "'", "''")
        $email = $userReader2["Email"]
        $passwordHash = $userReader2["PasswordHash"]
        $phone = if ($userReader2["Phone"] -eq [DBNull]::Value) { "NULL" } else { "'" + $userReader2["Phone"] + "'" }
        $role = $userReader2["Role"]
        $walletBalance = $userReader2["WalletBalance"]
        
        # Avatar with new name
        $avatar = "NULL"
        if ($userReader2["Avatar"] -ne [DBNull]::Value) {
            $oldAvatar = $userReader2["Avatar"]
            if ($oldAvatar -match '/assets/images/avatars/(.+)$') {
                $guidFile = $Matches[1]
                if ($avatarMapping.ContainsKey($guidFile)) {
                    $newFile = $avatarMapping[$guidFile]
                    $avatar = "'/assets/images/avatars/" + $newFile + "'"
                } else {
                    $avatar = "'" + $oldAvatar + "'"
                }
            } else {
                $avatar = "'" + $oldAvatar + "'"
            }
        }
        
        $dateOfBirth = if ($userReader2["DateOfBirth"] -eq [DBNull]::Value) { "NULL" } else { "'" + $userReader2["DateOfBirth"].ToString("yyyy-MM-dd") + "'" }
        $gender = if ($userReader2["Gender"] -eq [DBNull]::Value) { "NULL" } else { "N'" + ($userReader2["Gender"] -replace "'", "''") + "'" }
        
        $exportSQL += "INSERT INTO [User] (UserId, Username, FullName, Email, PasswordHash, Phone, Role, WalletBalance, CreatedAt, UpdatedAt, Avatar, DateOfBirth, Gender)`r`n"
        $exportSQL += "VALUES ($userId, '$username', N'$fullName', '$email', '$passwordHash', $phone, '$role', $walletBalance, GETUTCDATE(), GETUTCDATE(), $avatar, $dateOfBirth, $gender);`r`n`r`n"
    }
    $userReader2.Close()
    
    $exportSQL += "SET IDENTITY_INSERT [User] OFF;`r`n`r`n"
    
    # Export Events
    $exportSQL += "-- Insert Events`r`nSET IDENTITY_INSERT Event ON;`r`n`r`n"
    
    $eventCmd2 = $connection.CreateCommand()
    $eventCmd2.CommandText = "SELECT * FROM Event ORDER BY EventId"
    $eventReader2 = $eventCmd2.ExecuteReader()
    
    while ($eventReader2.Read()) {
        $eventId = $eventReader2["EventId"]
        $hostId = $eventReader2["HostId"]
        $title = ($eventReader2["Title"] -replace "'", "''")
        $description = if ($eventReader2["Description"] -eq [DBNull]::Value) { "NULL" } else { "N'" + ($eventReader2["Description"] -replace "'", "''") + "'" }
        $location = if ($eventReader2["Location"] -eq [DBNull]::Value) { "NULL" } else { "N'" + ($eventReader2["Location"] -replace "'", "''") + "'" }
        $eventType = if ($eventReader2["EventType"] -eq [DBNull]::Value) { "NULL" } else { "N'" + ($eventReader2["EventType"] -replace "'", "''") + "'" }
        $eventMode = if ($eventReader2["EventMode"] -eq [DBNull]::Value) { "'Offline'" } else { "'" + $eventReader2["EventMode"] + "'" }
        $category = if ($eventReader2["Category"] -eq [DBNull]::Value) { "NULL" } else { "'" + $eventReader2["Category"] + "'" }
        $status = if ($eventReader2["Status"] -eq [DBNull]::Value) { "'Draft'" } else { "'" + $eventReader2["Status"] + "'" }
        
        # EventDetails with updated paths
        $eventDetails = "NULL"
        if ($eventReader2["EventDetails"] -ne [DBNull]::Value) {
            $detailsJson = $eventReader2["EventDetails"]
            
            # Replace GUID with friendly names
            foreach ($guid in $imageMapping.Keys) {
                $friendlyName = $imageMapping[$guid]
                $oldPath = "/assets/images/events/" + $guid
                $newPath = "/assets/images/events/" + $friendlyName
                $detailsJson = $detailsJson.Replace($oldPath, $newPath)
            }
            
            $eventDetails = "N'" + ($detailsJson -replace "'", "''") + "'"
        }
        
        $termsAndConditions = if ($eventReader2["TermsAndConditions"] -eq [DBNull]::Value) { "NULL" } else { "N'" + ($eventReader2["TermsAndConditions"] -replace "'", "''") + "'" }
        $organizerInfo = if ($eventReader2["OrganizerInfo"] -eq [DBNull]::Value) { "NULL" } else { "N'" + ($eventReader2["OrganizerInfo"] -replace "'", "''") + "'" }
        $venueLayout = if ($eventReader2["VenueLayout"] -eq [DBNull]::Value) { "NULL" } else { "N'" + ($eventReader2["VenueLayout"] -replace "'", "''") + "'" }
        
        $exportSQL += "INSERT INTO Event (EventId, HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, VenueLayout, CreatedAt, UpdatedAt)`r`n"
        $exportSQL += "VALUES ($eventId, $hostId, N'$title', $description, DATEADD(day, 7, GETUTCDATE()), DATEADD(day, 7, DATEADD(hour, 3, GETUTCDATE())), $location, $eventType, $eventMode, $category, $status, $eventDetails, $termsAndConditions, $organizerInfo, $venueLayout, GETUTCDATE(), GETUTCDATE());`r`n`r`n"
    }
    $eventReader2.Close()
    
    $exportSQL += "SET IDENTITY_INSERT Event OFF;`r`n`r`n"
    
    # Export TicketTypes
    Write-Host "  - Exporting Ticket Types..." -ForegroundColor Cyan
    $exportSQL += "-- Insert Ticket Types`r`nSET IDENTITY_INSERT TicketType ON;`r`n`r`n"
    
    $ticketCmd = $connection.CreateCommand()
    $ticketCmd.CommandText = "SELECT * FROM TicketType ORDER BY EventId, TicketTypeId"
    $ticketReader = $ticketCmd.ExecuteReader()
    
    while ($ticketReader.Read()) {
        $ticketTypeId = $ticketReader["TicketTypeId"]
        $eventId = $ticketReader["EventId"]
        $typeName = ($ticketReader["TypeName"] -replace "'", "''")
        $price = $ticketReader["Price"]
        $quantity = $ticketReader["Quantity"]
        $minOrder = $ticketReader["MinOrder"]
        $maxOrder = $ticketReader["MaxOrder"]
        $ticketStatus = if ($ticketReader["Status"] -eq [DBNull]::Value) { "'Active'" } else { "'" + $ticketReader["Status"] + "'" }
        
        $exportSQL += "INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)`r`n"
        $exportSQL += "VALUES ($ticketTypeId, $eventId, N'$typeName', $price, $quantity, $minOrder, $maxOrder, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), $ticketStatus);`r`n`r`n"
    }
    $ticketReader.Close()
    
    $exportSQL += "SET IDENTITY_INSERT TicketType OFF;`r`n`r`n"
    
    # Verification
    $exportSQL += "-- Verification`r`n"
    $exportSQL += "SELECT 'Users:' as Info, COUNT(*) as Count FROM [User];`r`n"
    $exportSQL += "SELECT 'Events:' as Info, COUNT(*) as Count FROM Event;`r`n"
    $exportSQL += "SELECT 'Ticket Types:' as Info, COUNT(*) as Count FROM TicketType;`r`n"
    $exportSQL += "PRINT 'Sample data loaded successfully!';`r`n"
    
    $connection.Close()
    
    Write-Host "  [OK] Database exported" -ForegroundColor Green
    
    # STEP 4: Save SQL file
    Write-Host ""
    Write-Host "[STEP 4/5] Saving SQL file..." -ForegroundColor Yellow
    
    $exportSQL | Out-File -FilePath $OutputFile -Encoding UTF8
    
    Write-Host "  [OK] Saved to: $OutputFile" -ForegroundColor Green
    
    # STEP 5: Report
    Write-Host ""
    Write-Host "[STEP 5/5] Summary" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "           SUCCESS!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "  Events exported: $eventCount" -ForegroundColor Cyan
    Write-Host "  Images renamed: $($imageMapping.Count)" -ForegroundColor Cyan
    Write-Host "  Avatars renamed: $($avatarMapping.Count)" -ForegroundColor Cyan
    Write-Host "  SQL file: $OutputFile" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "  1. git add $OutputFile assets/images/" -ForegroundColor White
    Write-Host "  2. git commit -m 'Add $eventCount extended sample events'" -ForegroundColor White
    Write-Host "  3. git push" -ForegroundColor White
    Write-Host ""
    
} catch {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "           ERROR!" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host ""
    
    if ($connection -and $connection.State -eq 'Open') {
        $connection.Close()
    }
    
    exit 1
}

