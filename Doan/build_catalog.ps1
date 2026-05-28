# ASCII-only orchestrator: reads catalog.json (UTF-8), downloads images, generates SQL
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$ErrorActionPreference = 'Continue'

$base = "D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan"
$jsonPath = Join-Path $base "catalog.json"
$imgDir = Join-Path $base "Doan\images\cars"
$sqlPath = Join-Path $base "SEED_XE.sql"
$ua = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120 Safari/537.36"

# Load catalog JSON as UTF-8
$jsonText = [System.IO.File]::ReadAllText($jsonPath, [System.Text.UTF8Encoding]::new($false))
$catalog = $jsonText | ConvertFrom-Json
Write-Host ("Catalog loaded: " + $catalog.Count + " cars")

if (-not (Test-Path $imgDir)) { New-Item -ItemType Directory -Path $imgDir -Force | Out-Null }

# Fetch thumbnails via Wikipedia REST API
$i = 0
foreach ($car in $catalog) {
  $i++
  $slug = $car.WikiSlug
  $apiUrl = "https://en.wikipedia.org/api/rest_v1/page/summary/$slug"
  $thumb = $null
  for ($attempt = 0; $attempt -lt 3; $attempt++) {
    try {
      $resp = Invoke-RestMethod -Uri $apiUrl -UserAgent $ua -TimeoutSec 15 -ErrorAction Stop
      if ($resp.thumbnail -and $resp.thumbnail.source) { $thumb = $resp.thumbnail.source; break }
      if ($resp.originalimage -and $resp.originalimage.source) { $thumb = $resp.originalimage.source; break }
      break
    } catch {
      Start-Sleep -Seconds 2
    }
  }
  if (-not $thumb) {
    Write-Host ("[" + $i + "/" + $catalog.Count + "] NO IMG: " + $slug)
    $car | Add-Member -NotePropertyName ThumbUrl -NotePropertyValue $null -Force
    continue
  }
  if ($thumb -match '^(.+/thumb/[^/]+/[^/]+/[^/]+/)(\d+)(px-.+)$') {
    $thumb500 = $matches[1] + '500' + $matches[3]
  } else {
    $thumb500 = $thumb
  }
  $car | Add-Member -NotePropertyName ThumbUrl -NotePropertyValue $thumb500 -Force
  Write-Host ("[" + $i + "/" + $catalog.Count + "] " + $car.MaXe + " -> " + $thumb500)
  Start-Sleep -Milliseconds 250
}

# Download images locally
Write-Host "`n=== DOWNLOADING IMAGES ==="
$downloaded = 0; $skipped = 0; $failedList = @()
$j = 0
foreach ($car in $catalog) {
  $j++
  $localName = $car.MaXe + ".jpg"
  $localPath = Join-Path $imgDir $localName
  if (-not $car.ThumbUrl) { $failedList += $car.MaXe; continue }
  if ((Test-Path $localPath) -and ((Get-Item $localPath).Length -gt 1000)) {
    $car | Add-Member -NotePropertyName LocalPath -NotePropertyValue $localPath -Force
    $skipped++
    continue
  }
  $success = $false
  for ($attempt = 0; $attempt -lt 4; $attempt++) {
    $code = & curl.exe -s -o $localPath -w "%{http_code}" -A $ua $car.ThumbUrl
    if ($code -eq '200' -and (Test-Path $localPath) -and ((Get-Item $localPath).Length -gt 1000)) {
      $car | Add-Member -NotePropertyName LocalPath -NotePropertyValue $localPath -Force
      $downloaded++
      $success = $true
      break
    }
    Start-Sleep -Milliseconds ([Math]::Min(8000, 1500 * ($attempt + 1)))
  }
  if (-not $success) {
    $failedList += $car.MaXe
    Write-Host ("[" + $j + "/" + $catalog.Count + "] FAIL " + $car.MaXe + " <- " + $car.ThumbUrl)
  } else {
    Write-Host ("[" + $j + "/" + $catalog.Count + "] OK   " + $car.MaXe)
  }
  Start-Sleep -Milliseconds 200
}

Write-Host ""
Write-Host ("Downloaded: " + $downloaded + ", Skipped: " + $skipped + ", Failed: " + $failedList.Count)
if ($failedList.Count -gt 0) { Write-Host ("Failed IDs: " + ($failedList -join ', ')) }

# Generate SQL
$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("USE DL_OTO;")
[void]$sb.AppendLine("GO")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("DELETE FROM HoaDon;")
[void]$sb.AppendLine("DELETE FROM Xe;")
[void]$sb.AppendLine("GO")
[void]$sb.AppendLine("")
foreach ($car in $catalog) {
  $tenEsc = $car.TenXe.Replace("'", "''")
  $mauEsc = $car.MauSac.Replace("'", "''")
  $moTaEsc = $car.MoTa.Replace("'", "''")
  $loaiEsc = $car.LoaiXe.Replace("'", "''")
  $imgRaw = $null
  if ($car.PSObject.Properties.Match('LocalPath').Count -gt 0) { $imgRaw = $car.LocalPath }
  if (-not $imgRaw) { $imgRaw = '' }
  $imgEsc = $imgRaw.Replace("'", "''")
  [void]$sb.AppendLine("INSERT INTO Xe (MaXe, TenXe, LoaiXe, NamSX, GiaBan, MauSac, MoTa, HinhAnh, MaHang, SoLuongTon) VALUES ('$($car.MaXe)', N'$tenEsc', N'$loaiEsc', $($car.NamSX), $($car.GiaBan), N'$mauEsc', N'$moTaEsc', N'$imgEsc', '$($car.MaHang)', $($car.SoLuongTon));")
}
[void]$sb.AppendLine("GO")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("SELECT 'Xe seeded: ' + CAST((SELECT COUNT(*) FROM Xe) AS NVARCHAR(10));")
[void]$sb.AppendLine("GO")
[System.IO.File]::WriteAllText($sqlPath, $sb.ToString(), (New-Object System.Text.UTF8Encoding($false)))
Write-Host ("SQL written: " + $sqlPath)
