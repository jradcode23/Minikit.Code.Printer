# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/LPotC.Minikit.Codes/*" -Force -Recurse
dotnet publish "./LPotC.Minikit.Codes.csproj" -c Release -o "$env:RELOADEDIIMODS/LPotC.Minikit.Codes" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location