# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/LMSH.Minikit.Codes/*" -Force -Recurse
dotnet publish "./LMSH.Minikit.Codes.csproj" -c Release -o "$env:RELOADEDIIMODS/LMSH.Minikit.Codes" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location