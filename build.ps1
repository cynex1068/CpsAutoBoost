$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'WindowsCpsTool.csproj'
$out = Join-Path $PSScriptRoot 'publish'

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw 'dotnet not found. Please install the .NET 8 SDK first: https://dotnet.microsoft.com/download/dotnet/8.0'
}

if (Test-Path $out) { Remove-Item $out -Recurse -Force }
dotnet publish $project -c Release -r win-x64 --self-contained true -o $out
Write-Host "Done: $out\WindowsCpsTool.exe" -ForegroundColor Green