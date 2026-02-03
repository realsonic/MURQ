<# Windows #>
dotnet publish --os win --configuration Debug --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:EnableCompressionInSingleFile=true `
&& [System.Console]::Beep(1000, 700) `
&& start .\bin\Debug\net9.0\win-x64\publish\ `
<# ---------------------------------------- #>`
<# Linux #>`
&& dotnet publish --os linux --configuration Debug --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:EnableCompressionInSingleFile=true `
&& Copy-Item .\bin\Debug\net9.0\linux-x64\publish\MURQ.Console .\bin\Debug\net9.0\linux-x64\publish\MURQ.Console.Linux -Force `
&& [System.Console]::Beep(2000, 700) `
&& start .\bin\Debug\net9.0\linux-x64\publish\ `
<# ---------------------------------------- #>`
<# Mac #>`
&& dotnet publish --os osx --configuration Debug --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:EnableCompressionInSingleFile=true `
&& Copy-Item .\bin\Debug\net9.0\osx-x64\publish\MURQ.Console .\bin\Debug\net9.0\osx-x64\publish\MURQ.Console.OSX -Force `
&& [System.Console]::Beep(3000, 700) `
&& start .\bin\Debug\net9.0\osx-x64\publish\ `
