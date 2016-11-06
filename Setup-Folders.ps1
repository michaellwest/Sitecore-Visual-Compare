# This is where your git-hub repository is located
$projectPath = "C:\Projects\Sitecore-Visual-Compare"
if(-not (Test-Path -Path $projectPath)) {
    Write-Error "The project path defined does not exist."
    exit
}

# This is where your sitecore sites are
# The sites need to have the standard \Data \Web folders in them
$sites = @{Path = "C:\Websites\versions.dev.local"; Version="8"}

#Set the below to true to remove junction points only and not set them back
$removeOnly = $false

# --------------------------------------------------------------
# Ignore everything after this - all you need to provide is above
# --------------------------------------------------------------

function Create-Junction{
    [cmdletbinding(
        DefaultParameterSetName = 'Directory',
        SupportsShouldProcess=$True
    )]
    Param (
        [string]$path,
        [string]$source
        )
    Write-Host "$path --> $source"
    if(Test-Path "$path"){
        cmd.exe /c "rmdir `"$path`" /Q /S" 
    }
    if(-not $removeOnly){
        cmd.exe /c "mklink /J `"$path`" `"$source`""
    }
}

function Create-ProjectJunctions{
    [cmdletbinding(
        DefaultParameterSetName = 'Directory',
        SupportsShouldProcess=$True
    )]
    Param (
        [string]$path, 
        [int]$version
        )

    Write-Host "--------------------------------------------------------------------------------------------"
    Write-Host "$project\$version --> $path"
    Write-Host "--------------------------------------------------------------------------------------------"

    Create-Junction "$path\Data\serialization" "$projectPath\src\serialization"
}

foreach($sitecoreSite in $sites){
    if(Test-Path -Path $sitecoreSite.Path) {
	    Create-ProjectJunctions @sitecoreSite
    }
}