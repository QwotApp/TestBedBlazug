$previousVersion = git describe --abbrev=0 --tags $(git rev-list --tags --skip=0 --max-count=1)

$version = [int]($previousVersion.Split("-"))[0].Replace(".","")+1
$version = [string]("{0:d3}" -f $version)
$version = $version.Insert($version.length-1,".").Insert($version.length-2,".")

Write-Host $previousVersion "->" $version



$previousVersionStr = "blazug@" + $lastVersion
$versionStr = "blazug@" + $version

$cssFile = Get-Content -Path .\blazug.css
$cssCount = ([regex]::Matches($cssFile, $previousVersionStr )).count

$jsFile = Get-Content -Path .\blazug.css
$jsCount = ([regex]::Matches($jsFile, $previousVersionStr )).count

$htmlFile = Get-Content -Path .\index.html
$htmlCount = ([regex]::Matches($htmlFile, $previousVersionStr )).count

Write-Host $cssCount $jsCount $htmlCount


$string.replace($cssFile,$versionStr) | Out-File -FilePath .\Process.txt
$string.replace($jsFile,$versionStr)
$string.replace($htmlFile,$versionStr)
