# the version under development, update after a release
$version = '3.0.0'
$modifier = '-beta-6'

function isVersion($s){
    $v = New-Object Version
    [Version]::TryParse($s, [ref]$v)
}

# append the AppVeyor build number as the pre-release version
if ($env:appveyor){
    $modifier = $modifier + '-' + [int]::Parse($env:appveyor_build_number).ToString('000')
    if ($env:appveyor_repo_tag -eq 'true'){
        $tag = $env:appveyor_repo_tag_name
        $i = $tag.IndexOf('-')
        if($i -gt 0)
        {
            $version = $tag.Substring(0, $i)
            $modifier = $tag.Substring($i)
        } else {
            $version = $tag
            $modifier = ''
        }
    }
    if(-not(isVersion($version)))
    {
        Write-Error "error parsing version '$version' in tag '$tag'"
        exit
    }
    Update-AppveyorBuild -Version "$version$modifier"
}

./build.cmd NUnit.proj /t:BuildAll /p:Configuration=Release /p:PackageVersion="$version" /p:PackageModifier="$modifier"
./build.cmd NUnit.proj /t:TestAll /p:Configuration=Release /p:PackageVersion="$version" /p:PackageModifier="$modifier"
./build.cmd NUnit.proj /t:Package /p:Configuration=Release /p:PackageVersion="$version" /p:PackageModifier="$modifier"