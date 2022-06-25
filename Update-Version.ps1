# updates version strings in files and sets git tag.

# Check if everything is committed.
$uncommitted = git status --porcelain

if($uncommitted -ne 0)
{
    Write-Host Please commit everything before updating version.
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

$cssFile = Get-Content -Path .\blazug.css
$jsFile = Get-Content -Path .\blazug.js
$htmlFile = Get-Content -Path .\index.html

# Get verions tags count

$cssCount = ([regex]::Matches($cssFile, $previousVersionStrL )).count

$jsCount = ([regex]::Matches($jsFile, $previousVersionStrL )).count

$htmlCount = ([regex]::Matches($htmlFile, $previousVersionStrU )).count

if($cssCount -ne 2)
{
    Write-Host $versionStrL should appear 2 time in $cssFile. It has been found $cssCount times
    return
}



Write-Host $cssCount $jsCount $htmlCount



# Replace version tags


$cssFile.replace($previousVersionStrL,$versionStrL) | Out-File -FilePath .\blazug.css
$jsFile.replace($previousVersionStrL,$versionStrL) | Out-File -FilePath .\blazug.js
$htmlFile.replace($previousVersionStrU,$versionStrU) | Out-File -FilePath .\index.html
