# Download placeholder images cho các GUID missing
# Chạy: .\download_missing_images.ps1

Write-Host ""
Write-Host "Downloading placeholder images for missing GUIDs..." -ForegroundColor Cyan
Write-Host ""

# List GUIDs từ output của bạn
$missingGuids = @(
    "b33c34c7-d13b-4531-a7b7-a21add3fd730.jpg",
    "d031d269-2b9e-4981-9087-72332ce748f9.png",
    "8ba8951c-e857-4564-a27f-ece4ef9a6f06.png",
    "fd85325e-d50e-40ee-8d45-f9d6f2606ecf.png",
    "d81cf3f5-0052-4338-acbf-eaaab9ddc883.png",
    "7ca0e109-966b-48c1-99eb-c5e92faab8ff.png",
    "f09b13c3-f68a-45f4-96aa-8e448cdd3328.png",
    "5fd99852-3985-4cb2-bb55-455f077e9dab.png",
    "c5d33b8c-efc2-4234-8c9e-2f200ee0008c.png",
    "360872c9-e969-4b9c-b569-f40e166cf9d5.jpg"
)

$assetsFolder = "assets\images\events\"

if (-not (Test-Path $assetsFolder)) {
    New-Item -ItemType Directory -Path $assetsFolder -Force | Out-Null
    Write-Host "Created directory: $assetsFolder" -ForegroundColor Green
}

$successCount = 0
$failCount = 0

foreach ($guid in $missingGuids) {
    $ext = [System.IO.Path]::GetExtension($guid).ToLower()
    $output = Join-Path $assetsFolder $guid
    
    # Chọn URL placeholder dựa trên extension
    if ($ext -eq ".jpg" -or $ext -eq ".jpeg") {
        $url = "https://placehold.co/1200x630/667eea/ffffff?text=Event+Image"
    } elseif ($ext -eq ".png") {
        $url = "https://placehold.co/1200x630/764ba2/ffffff?text=Event+Image"
    } else {
        $url = "https://placehold.co/1200x630/667eea/ffffff?text=Event+Image"
    }
    
    try {
        Write-Host "Downloading: $guid" -ForegroundColor Yellow
        Invoke-WebRequest -Uri $url -OutFile $output -ErrorAction Stop
        Write-Host "  [OK] Saved: $output" -ForegroundColor Green
        $successCount++
    } catch {
        Write-Host "  [FAILED] $guid - $_" -ForegroundColor Red
        $failCount++
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Success: $successCount files" -ForegroundColor Green
Write-Host "  Failed: $failCount files" -ForegroundColor Red
Write-Host ""
Write-Host "Next: Run .\auto_export_all.bat again" -ForegroundColor Yellow
Write-Host ""

