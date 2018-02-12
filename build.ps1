#!/usr/bin/env pwsh
$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

$artifacts = "$PSScriptRoot/artifacts"
if (Test-Path $artifacts) {
	Remove-Item $artifacts -Recurse -Force
}

& dotnet pack -c Release -o $artifacts
exit $LASTEXITCODE