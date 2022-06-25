# updates version strings in files and sets git tag.

# Check if everything is committed.
$uncommitted = git status --porcelain

if($uncommitted.length -ne 0)
{
    Write-Host `n$uncommitted `n
    Write-Host Please commit everything before updating version.`n -ForegroundColor Magenta
    return
}

# Increment version 
$previousVersion = git describe --abbrev=0 --tags $(git rev-list --tags --skip=0 --max-count=1)

$version = [int]($previousVersion.Split("-"))[0].Replace(".","")+1
$version = [string]("{0:d3}" -f $version)
$version = $version.Insert($version.length-1,".").Insert($version.length-2,".")

Write-Host $previousVersion "->" $version

$previousVersionStrL = "blazug@" + $previousVersion
$previousVersionStrU = "Blazug@" + $previousVersion
$versionStrL = "blazug@" + $version
$versionStrU = "Blazug@" + $version

# Load Content
$cssFileName = ".\blazug.css"
$jsFileName = ".\blazug.js"
$htmlFileName = ".\index.html"

$cssFile = Get-Content -Path $cssFileName
$jsFile = Get-Content -Path $jsFileName
$htmlFile = Get-Content -Path $htmlFileName

# Get verions tags count

$cssCount = ([regex]::Matches($cssFile, $previousVersionStrL )).count

$jsCount = ([regex]::Matches($jsFile, $previousVersionStrL )).count

$htmlCount = ([regex]::Matches($htmlFile, $previousVersionStrU )).count

if($cssCount -ne 2)
{
    Write-Host `n$versionStrL should appear 2 time in $cssFileName. It has been found $cssCount times`n -ForegroundColor Magenta
    return
}



Write-Host $cssCount $jsCount $htmlCount



# Replace version tags


$cssFile.replace($previousVersionStrL,$versionStrL) | Out-File -FilePath .\blazug.css
$jsFile.replace($previousVersionStrL,$versionStrL) | Out-File -FilePath .\blazug.js
$htmlFile.replace($previousVersionStrU,$versionStrU) | Out-File -FilePath .\index.html
