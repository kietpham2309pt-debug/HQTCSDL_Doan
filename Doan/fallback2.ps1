# Second-pass fallback with longer delays and broader fallback slugs
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$imgDir = "D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars"
$ua = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120 Safari/537.36"

Write-Host "Waiting 25s for rate limit reset..."
Start-Sleep -Seconds 25

$tasks = @(
  @{ MaXe='XE32'; Slugs=@('Hyundai_Creta','Hyundai_Creta_(SU2)','Hyundai_ix25') }
  @{ MaXe='XE33'; Slugs=@('Hyundai_Stargazer') }
  @{ MaXe='XE34'; Slugs=@('Hyundai_Kona','Hyundai_Kona_Electric') }
  @{ MaXe='XE36'; Slugs=@('Kia_Seltos') }
  @{ MaXe='XE38'; Slugs=@('Kia_Carnival','Kia_Sedona') }
  @{ MaXe='XE40'; Slugs=@('Kia_Sonet') }
  @{ MaXe='XE41'; Slugs=@('Kia_Picanto','Kia_Morning') }
  @{ MaXe='XE42'; Slugs=@('Kia_EV6') }
  @{ MaXe='XE43'; Slugs=@('Ford_Ranger','Ford_Ranger_(T6)') }
  @{ MaXe='XE44'; Slugs=@('Ford_Explorer') }
  @{ MaXe='XE45'; Slugs=@('Ford_Territory') }
  @{ MaXe='XE47'; Slugs=@('Ford_Transit') }
  @{ MaXe='XE49'; Slugs=@('Mercedes-Benz_E-Class','Mercedes-Benz_W214') }
  @{ MaXe='XE65'; Slugs=@('VinFast_VF_5') }
  @{ MaXe='XE66'; Slugs=@('VinFast_VF_e34') }
  @{ MaXe='XE67'; Slugs=@('Tesla_Model_S') }
  @{ MaXe='XE68'; Slugs=@('Tesla_Model_X') }
  @{ MaXe='XE69'; Slugs=@('Tesla_Model_Y') }
  @{ MaXe='XE70'; Slugs=@('Tesla_Cybertruck') }
  @{ MaXe='XE71'; Slugs=@('Tesla_Roadster_(second_generation)','Tesla_Roadster_(2020)') }
)

function Get-Thumb([string]$slug, [string]$ua) {
  # Try summary first
  $u1 = "https://en.wikipedia.org/api/rest_v1/page/summary/$slug"
  try {
    $r = Invoke-RestMethod -Uri $u1 -UserAgent $ua -TimeoutSec 15
    if ($r.thumbnail.source) {
      $t = $r.thumbnail.source
      if ($t -match '^(.+/thumb/[^/]+/[^/]+/[^/]+/)(\d+)(px-.+)$') { return $matches[1] + '500' + $matches[3] }
      return $t
    }
    if ($r.originalimage.source) { return $r.originalimage.source }
  } catch {}
  Start-Sleep -Milliseconds 600
  # Try pageimages
  $u2 = "https://en.wikipedia.org/w/api.php?action=query&prop=pageimages&format=json&pithumbsize=500&redirects=1&titles=$slug"
  try {
    $r = Invoke-RestMethod -Uri $u2 -UserAgent $ua -TimeoutSec 15
    foreach ($pid_ in $r.query.pages.PSObject.Properties.Name) {
      $p = $r.query.pages.$pid_
      if ($p.thumbnail.source) { return $p.thumbnail.source }
    }
  } catch {}
  return $null
}

$results = @{}
$cnt = 0
foreach ($t in $tasks) {
  $cnt++
  $found = $null
  foreach ($slug in $t.Slugs) {
    $found = Get-Thumb -slug $slug -ua $ua
    if ($found) { break }
    Start-Sleep -Seconds 2
  }
  if (-not $found) {
    Write-Host ("[" + $cnt + "/" + $tasks.Count + "] " + $t.MaXe + " STILL NO IMG")
    Start-Sleep -Seconds 2
    continue
  }
  $localPath = Join-Path $imgDir ($t.MaXe + ".jpg")
  $okDownload = $false
  for ($i = 0; $i -lt 5; $i++) {
    $code = & curl.exe -s -o $localPath -w "%{http_code}" -A $ua $found
    if ($code -eq '200' -and (Test-Path $localPath) -and ((Get-Item $localPath).Length -gt 1000)) {
      $okDownload = $true
      break
    }
    Start-Sleep -Seconds (3 + $i)
  }
  if ($okDownload) {
    $results[$t.MaXe] = $localPath
    Write-Host ("[" + $cnt + "/" + $tasks.Count + "] " + $t.MaXe + " OK: " + $found)
  } else {
    Write-Host ("[" + $cnt + "/" + $tasks.Count + "] " + $t.MaXe + " DL FAIL")
  }
  Start-Sleep -Seconds 3
}

Write-Host ""
Write-Host ("Resolved this pass: " + $results.Count + "/" + $tasks.Count)

$updates = @("USE DL_OTO;", "GO")
foreach ($k in $results.Keys) {
  $path = $results[$k].Replace("'", "''")
  $updates += "UPDATE Xe SET HinhAnh = N'" + $path + "' WHERE MaXe = '" + $k + "';"
}
$updates += "GO"
$outSql = "D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\FALLBACK2.sql"
[System.IO.File]::WriteAllLines($outSql, $updates, (New-Object System.Text.UTF8Encoding($false)))
Write-Host ("SQL: " + $outSql)
