# Fallback: fetch missing images using Wikipedia pageimages API + retries
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$imgDir = "D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars"
$ua = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120 Safari/537.36"

# Map MaXe to candidate slugs and direct fallback URLs
$tasks = @(
  @{ MaXe='XE07'; Slugs=@('Kia_Sorento_(MQ4)','Kia_Sorento') }
  @{ MaXe='XE26'; Slugs=@('Mazda3_(BP)','Mazda_Axela','Mazda3') }
  @{ MaXe='XE27'; Slugs=@('Mazda6','Mazda_Atenza') }
  @{ MaXe='XE28'; Slugs=@('Mazda_CX-9','Mazda_CX-90') }
  @{ MaXe='XE29'; Slugs=@('Hyundai_Tucson_(NX4)','Hyundai_Tucson') }
  @{ MaXe='XE30'; Slugs=@('Hyundai_Santa_Fe_(MX5)','Hyundai_Santa_Fe') }
  @{ MaXe='XE32'; Slugs=@('Hyundai_Creta_(SU2)','Hyundai_Creta') }
  @{ MaXe='XE33'; Slugs=@('Hyundai_Stargazer','Hyundai_Stargazer_X') }
  @{ MaXe='XE34'; Slugs=@('Hyundai_Kona_(SX2)','Hyundai_Kona') }
  @{ MaXe='XE36'; Slugs=@('Kia_Seltos','Kia_Seltos_(SP2)') }
  @{ MaXe='XE38'; Slugs=@('Kia_Carnival_(KA4)','Kia_Carnival') }
  @{ MaXe='XE40'; Slugs=@('Kia_Sonet_(AY)','Kia_Sonet') }
  @{ MaXe='XE41'; Slugs=@('Kia_Morning','Kia_Picanto_(JA)','Kia_Picanto') }
  @{ MaXe='XE42'; Slugs=@('Kia_EV6','Kia_EV6_GT') }
  @{ MaXe='XE43'; Slugs=@('Ford_Ranger_(2022)','Ford_Ranger_(T6)','Ford_Ranger') }
  @{ MaXe='XE44'; Slugs=@('Ford_Explorer_(sixth_generation)','Ford_Explorer') }
  @{ MaXe='XE45'; Slugs=@('Ford_Territory_(CX743)','Ford_Territory') }
  @{ MaXe='XE47'; Slugs=@('Ford_Transit','Ford_Transit_(fourth_generation)') }
  @{ MaXe='XE49'; Slugs=@('Mercedes-Benz_W214','Mercedes-Benz_E-Class') }
  @{ MaXe='XE65'; Slugs=@('VinFast_VF_5','VinFast') }
  @{ MaXe='XE66'; Slugs=@('VinFast_VF_e34','VinFast') }
  @{ MaXe='XE67'; Slugs=@('Tesla_Model_S','Tesla_Model_S_Plaid') }
  @{ MaXe='XE68'; Slugs=@('Tesla_Model_X','Tesla_Model_X_Plaid') }
  @{ MaXe='XE69'; Slugs=@('Tesla_Model_Y','Tesla_Model_Y_Long_Range') }
  @{ MaXe='XE70'; Slugs=@('Tesla_Cybertruck','Cybertruck') }
  @{ MaXe='XE71'; Slugs=@('Tesla_Roadster_(second_generation)','Tesla_Roadster') }
)

function Get-ThumbViaPageImages([string]$slug, [string]$ua) {
  $u = "https://en.wikipedia.org/w/api.php?action=query&prop=pageimages&format=json&pithumbsize=500&redirects=1&titles=$slug"
  try {
    $resp = Invoke-RestMethod -Uri $u -UserAgent $ua -TimeoutSec 15
    foreach ($pageId in $resp.query.pages.PSObject.Properties.Name) {
      $page = $resp.query.pages.$pageId
      if ($page.thumbnail -and $page.thumbnail.source) { return $page.thumbnail.source }
    }
  } catch {}
  return $null
}

$results = @{}
$count = 0
foreach ($t in $tasks) {
  $count++
  $found = $null
  foreach ($slug in $t.Slugs) {
    $url = Get-ThumbViaPageImages -slug $slug -ua $ua
    if ($url) { $found = $url; break }
    Start-Sleep -Milliseconds 400
  }
  if (-not $found) {
    Write-Host ("[" + $count + "/" + $tasks.Count + "] " + $t.MaXe + " NO IMG via API")
    continue
  }
  # Try downloading with retries
  $localPath = Join-Path $imgDir ($t.MaXe + ".jpg")
  $okDownload = $false
  for ($attempt = 0; $attempt -lt 5; $attempt++) {
    $code = & curl.exe -s -o $localPath -w "%{http_code}" -A $ua $found
    if ($code -eq '200' -and (Test-Path $localPath) -and ((Get-Item $localPath).Length -gt 1000)) {
      $okDownload = $true
      break
    }
    Start-Sleep -Seconds (2 + $attempt)
  }
  if ($okDownload) {
    $results[$t.MaXe] = $localPath
    Write-Host ("[" + $count + "/" + $tasks.Count + "] " + $t.MaXe + " OK: " + $found)
  } else {
    Write-Host ("[" + $count + "/" + $tasks.Count + "] " + $t.MaXe + " DL FAIL: " + $found)
  }
  Start-Sleep -Milliseconds 600
}

Write-Host ""
Write-Host ("Resolved: " + $results.Count + "/" + $tasks.Count)

# Output mapping as SQL fragment
$updates = @()
$updates += "USE DL_OTO;"
$updates += "GO"
foreach ($k in $results.Keys) {
  $path = $results[$k].Replace("'", "''")
  $updates += "UPDATE Xe SET HinhAnh = N'" + $path + "' WHERE MaXe = '" + $k + "';"
}
$updates += "GO"
$outSql = "D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\FALLBACK_IMAGES.sql"
[System.IO.File]::WriteAllLines($outSql, $updates, (New-Object System.Text.UTF8Encoding($false)))
Write-Host ("Fallback SQL written: " + $outSql)
