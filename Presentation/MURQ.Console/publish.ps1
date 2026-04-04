$ErrorActionPreference = 'Stop'

<# Windows #>
dotnet publish --os win --configuration Debug --self-contained true -p:PublishAot=true
if ($LastExitCode -ne 0) { throw "Внешняя программа завершилась с ошибкой (код: $LastExitCode)" }
$productVersion = (Get-Item .\bin\Debug\net10.0\win-x64\publish\MURQ.Console.exe).VersionInfo.ProductVersionRaw
$version = "$($productVersion.Major).$($productVersion.Minor).$($productVersion.Build)"
Copy-Item .\bin\Debug\net10.0\win-x64\publish\MURQ.Console.exe .\bin\Debug\net10.0\win-x64\publish\MURQ.Console.$version.exe -Force
[System.Console]::Beep(1000, 700)
start .\bin\Debug\net10.0\win-x64\publish\

<# Linux #>
dotnet publish --os linux --configuration Debug --self-contained true -p:StripSymbols=true
if ($LastExitCode -ne 0) { throw "Внешняя программа завершилась с ошибкой (код: $LastExitCode)" }
Copy-Item .\bin\Debug\net10.0\linux-x64\publish\MURQ.Console .\bin\Debug\net10.0\linux-x64\publish\MURQ.Console.Linux.$version -Force
[System.Console]::Beep(2000, 700)
start .\bin\Debug\net10.0\linux-x64\publish\

<# OS X #>`
dotnet publish --os osx --configuration Debug --self-contained true -p:StripSymbols=true
if ($LastExitCode -ne 0) { throw "Внешняя программа завершилась с ошибкой (код: $LastExitCode)" }
Copy-Item .\bin\Debug\net10.0\osx-x64\publish\MURQ.Console .\bin\Debug\net10.0\osx-x64\publish\MURQ.Console.OSX.$version -Force
[System.Console]::Beep(3000, 700)
start .\bin\Debug\net10.0\osx-x64\publish\
