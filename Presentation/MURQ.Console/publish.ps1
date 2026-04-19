param (
	[Parameter(HelpMessage="(греческие буквы для версий: alpha, beta, gamma, delta, epsilon, zeta, eta, theta, iota, kappa, lambda, mu, nu, xi, omicron, pi, rho, sigma, tau, upsilon, phi, chi, psi, omega)")]
	[string]$VersionSuffix,
	
	[Parameter(HelpMessage="Собрать для всех платформ")]
	[switch]$AllPlatforms
)

$ErrorActionPreference = 'Stop'

<# Windows #>
dotnet publish --os win --configuration Debug --self-contained true -p:VersionSuffix=$VersionSuffix -p:PublishAot=true
if ($LastExitCode -ne 0) { throw "Внешняя программа завершилась с ошибкой (код: $LastExitCode)" }
$versionInfo = (Get-Item .\bin\Debug\net10.0\win-x64\publish\MURQ.Console.exe).VersionInfo
$fileVersion = $versionInfo.FileVersionRaw
$fullVersion = $versionInfo.ProductVersion
Write-Output "Версия: $fullVersion"
$version = "$($fileVersion.Major).$($fileVersion.Minor).$($fileVersion.Build)"
Copy-Item .\bin\Debug\net10.0\win-x64\publish\MURQ.Console.exe .\bin\Debug\net10.0\win-x64\publish\MURQ.Console.$version.exe -Force
Copy-Item .\bin\Debug\net10.0\win-x64\publish\MURQ.Console.exe .\bin\Debug\net10.0\win-x64\publish\MURQ.Console.$fullVersion.exe -Force
[System.Console]::Beep(1000, 700)
start .\bin\Debug\net10.0\win-x64\publish\ -WindowStyle Minimized

if (!$AllPlatforms) { return }

<# Linux #>
dotnet publish --os linux --configuration Debug --self-contained true -p:VersionSuffix=$VersionSuffix -p:StripSymbols=true
if ($LastExitCode -ne 0) { throw "Внешняя программа завершилась с ошибкой (код: $LastExitCode)" }
Copy-Item .\bin\Debug\net10.0\linux-x64\publish\MURQ.Console .\bin\Debug\net10.0\linux-x64\publish\MURQ.Console.Linux.$version -Force
#Copy-Item .\bin\Debug\net10.0\linux-x64\publish\MURQ.Console .\bin\Debug\net10.0\linux-x64\publish\MURQ.Console.Linux.$fullVersion -Force
[System.Console]::Beep(2000, 700)
start .\bin\Debug\net10.0\linux-x64\publish\ -WindowStyle Minimized

<# OS X #>`
dotnet publish --os osx --configuration Debug --self-contained true -p:VersionSuffix=$VersionSuffix -p:StripSymbols=true
if ($LastExitCode -ne 0) { throw "Внешняя программа завершилась с ошибкой (код: $LastExitCode)" }
Copy-Item .\bin\Debug\net10.0\osx-x64\publish\MURQ.Console .\bin\Debug\net10.0\osx-x64\publish\MURQ.Console.OSX.$version -Force
#Copy-Item .\bin\Debug\net10.0\osx-x64\publish\MURQ.Console .\bin\Debug\net10.0\osx-x64\publish\MURQ.Console.OSX.$fullVersion -Force
[System.Console]::Beep(3000, 700)
start .\bin\Debug\net10.0\osx-x64\publish\ -WindowStyle Minimized
