#!/bin/zsh
set -e
rm -rf artifacts
dotnet pack -c Release -o artifacts