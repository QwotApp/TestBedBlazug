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

#change branch to release

git checkout release

git merge main

git push

git checkout main

Write-Host Please check github action to see if everything is fine.`n -ForegroundColor Cyan

