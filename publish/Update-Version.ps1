# updates version strings in files and sets git tag.

# Check if everything is committed.

$currentBranch = git rev-parse --abbrev-ref HEAD

#Write-Host `n$currentBranch `n

if($currentBranch -ne "main")
{
    Write-Host you must be in the `'main`' branch`n -ForegroundColor Magenta
    return
}

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
$cssFileName = "..\blazug.css"
$jsFileName = "..\blazug.js"
$htmlFileName = "..\index.html"

$cssFile = Get-Content -Path $cssFileName
$jsFile = Get-Content -Path $jsFileName
$htmlFile = Get-Content -Path $htmlFileName

# Get verions tags count

$cssCount = ([regex]::Matches($cssFile, $previousVersionStrL )).count

$jsCount = ([regex]::Matches($jsFile, $previousVersionStrL )).count

$htmlCount = ([regex]::Matches($htmlFile, $previousVersionStrU )).count

if($cssCount -ne 1)
{
    Write-Host `n`'$versionStrL`' has been found $cssCount times in $cssFileName but it should appear 1 times`n -ForegroundColor Magenta
    return
}

if($jsCount -ne 2)
{
    Write-Host `n`'$versionStrL`' has been found $jsCount times in $jsFileName but it should appear 2 times`n -ForegroundColor Magenta
    return
}

if($htmlCount -ne 7)
{
    Write-Host `n`'$versionStrL`' has been found $htmlCount times in $htmlFileName but it should appear 7 times`n -ForegroundColor Magenta
    return
}

# Replace version tags

$cssFile.replace($previousVersionStrL,$versionStrL) | Out-File -FilePath $cssFileName
$jsFile.replace($previousVersionStrL,$versionStrL) | Out-File -FilePath $jsFileName
$htmlFile.replace($previousVersionStrU,$versionStrU) | Out-File -FilePath $htmlFileName

# Set git tag.
git tag $version

Write-Host `nPlease check changes and commit them to main branch before publishing release. -ForegroundColor Cyan
Write-Host You should update release-notes.md.`n -ForegroundColor Cyan
