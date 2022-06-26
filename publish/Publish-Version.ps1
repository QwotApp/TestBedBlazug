# updates version strings in files and sets git tag.

# Check if everything is committed.

$currentBranch = git rev-parse --abbrev-ref HEAD

Write-Host `n$currentBranch `n

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

# confirm

$version = git describe --abbrev=0 --tags $(git rev-list --tags --skip=0 --max-count=1)
$versionString = "#Release Blazug@" + $version

$confirmation = Read-Host "Are you sure you want to publish "`'$versionString"`' ? (y/n)"
if ($confirmation -ne 'y') {
    return
}

#change branch to release

git checkout release

git merge main -m $versionString

git push

git checkout main

Write-Host Please check github action to see if everything is fine.`n -ForegroundColor Cyan

