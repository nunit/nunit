Function Exec
{
    [CmdletBinding()]
    param (
        [Parameter(Position=0, Mandatory=1)]
        [scriptblock]$Command,
        [Parameter(Position=1, Mandatory=0)]
        [string]$ErrorMessage = "Execution of command failed.`n$Command"
    )
    $ErrorActionPreference = "Continue"
    & $Command 2>&1 | %{ "$_" }
    if ($LastExitCode -ne 0) {
        throw "Exec: $ErrorMessage`nExit code: $LastExitCode"
    }
}

Function Bootstrap {
  [CmdletBinding()]
    Param()

    Progress "Bootstrap: Start"

    if(!(Test-Administrator)) 
    {
        throw "Current executing user is not an administrator, please check your settings and try again."
    }  
    Progress "Adding GnuWin32 tools to PATH"
    $env:PATH = "C:\Program Files (x86)\Git\bin;" + $env:PATH

    InstallCFTools

    Progress "Bootstrap: Done"
}

Function InstallCFTools {
  [CmdletBinding()]
  Param()
   $url= "https://github.com/dicko2/CompactFrameworkBuildBins/raw/master/NETCFSetupv35.msi";
    Progress ("Downloading NETCFSetupv35 from: " + $url)
    Invoke-WebRequest -Uri $url -OutFile NETCFSetupv35.msi
    
    $url= "https://github.com/dicko2/CompactFrameworkBuildBins/raw/master/NETCFv35PowerToys.msi";
    Progress ("Downloading NETCFv35PowerToys from: " + $url)
    Invoke-WebRequest -Uri $url -OutFile NETCFv35PowerToys.msi
    
    Progress("Running NETCFSetupv35 installer")
  
    $msi = @("NETCFSetupv35.msi","NETCFv35PowerToys.msi")
    foreach ($msifile in $msi) 
    {
    if(!(Test-Path($msi)))
    {
        throw "MSI files are not present, please check logs."
    }
    Progress("Installing msi " + $msifile )
    Start-Process -FilePath "$env:systemroot\system32\msiexec.exe" -ArgumentList "/i `"$msifile`" /qn /norestart" -Wait -WorkingDirectory $pwd  -RedirectStandardOutput stdout.txt -RedirectStandardError stderr.txt
    $OutputText = get-content stdout.txt
    Progress($OutputText)
    $OutputText = get-content stderr.txt
    Progress($OutputText) 
    }
    if(!(Test-Path("C:\Windows\Microsoft.NET\Framework\v3.5\Microsoft.CompactFramework.CSharp.targets")))
    {
        throw "Compact framework files not found after install, install may have failed, please check logs."
    }
    RegistryWorkAround
}

Function Progress
{
    [CmdletBinding()]
    param (
        [Parameter(Position=0, Mandatory=0)]
        [string]$Message = ""
    )
    $ProgressMessage = '== ' + (Get-Date) + ': ' + $Message

    Write-Host $ProgressMessage -ForegroundColor Magenta
}

function Test-Administrator  
{  
    $user = [Security.Principal.WindowsIdentity]::GetCurrent();
    (New-Object Security.Principal.WindowsPrincipal $user).IsInRole([Security.Principal.WindowsBuiltinRole]::Administrator)  
}

function RegistryWorkAround
{
    ## http://community.sharpdevelop.net/forums/t/10548.aspx
    ## see above link for work around for error 
    ## The "AddHighDPIResource" task failed unexpectedly.
    ## System.ArgumentNullException: Value cannot be null.
    ## Parameter name: path1 
    $registryPaths = @("HKLM:\SOFTWARE\Microsoft\VisualStudio\9.0\Setup\VS","HKLM:\SOFTWARE\Wow6432Node\Microsoft\VisualStudio\9.0\Setup\VS")
    ## not last reply in forum post about needing the entry in WOW is why there is two paths
    $Name = "ProductDir"
    $value = "C:\Program Files (x86)\Microsoft Visual Studio 9.0"
    
    foreach($registryPath in $registryPaths)
    {
        If(!(Test-Path $registryPath))
        {
            New-Item -Path $registryPath -Force | Out-Null
        }
        If(!(Test-Path $registryPath+"\"+$Name))
        {
        New-ItemProperty -Path $registryPath -Name $name -Value $value `
            -PropertyType String -Force | Out-Null
        }
        If(!((Get-ItemProperty -Path $registryPath -Name $Name).ProductDir -eq "C:\Program Files (x86)\Microsoft Visual Studio 9.0"))
        {
            throw "Registry path " + $registryPath + " not set to correct value, please check logs"
        }
        else
        {
            Progress("Registry update ok to value " + (Get-ItemProperty -Path $registryPath -Name $Name).ProductDir)
        }                           
    }
}