#!/bin/bash

msbuild FigmaSharp/FigmaSharp/FigmaSharp.csproj /p:Configuration=Release /restore
msbuild FigmaSharp/FigmaSharp.Cocoa/FigmaSharp.Cocoa.csproj /p:Configuration=Release /restore
msbuild FigmaSharp.Controls/FigmaSharp.Controls/FigmaSharp.Controls.csproj /p:Configuration=Release /restore
msbuild FigmaSharp.Controls/FigmaSharp.Controls.Cocoa/FigmaSharp.Controls.Cocoa.csproj /p:Configuration=Release /restore

nuget pack -Version ${TRAVIS_TAG} NuGet/FigmaSharp.nuspec
nuget pack -Version ${TRAVIS_TAG} NuGet/FigmaSharp.Cocoa.nuspec
nuget pack -Version ${TRAVIS_TAG} NuGet/FigmaSharp.Views.nuspec
nuget pack -Version ${TRAVIS_TAG} NuGet/FigmaSharp.Views.Cocoa.nuspec
nuget pack -Version ${TRAVIS_TAG} NuGet/FigmaSharp.Controls.nuspec
nuget pack -Version ${TRAVIS_TAG} NuGet/FigmaSharp.Controls.Cocoa.nuspec

nuget push FigmaSharp.${TRAVIS_TAG} .nupkg -ApiKey ${NUGET_API_KEY} -Source https://api.nuget.org/v3/index.json  
nuget push FigmaSharp.${TRAVIS_TAG}.nupkg -ApiKey ${NUGET_API_KEY} -Source https://api.nuget.org/v3/index.json
nuget push FigmaSharp.Views.${TRAVIS_TAG}.nupkg -ApiKey ${NUGET_API_KEY} -Source https://api.nuget.org/v3/index.json
nuget push FigmaSharp.Views.Cocoa.${TRAVIS_TAG}.nupkg -ApiKey ${NUGET_API_KEY} -Source https://api.nuget.org/v3/index.json
nuget push FigmaSharp.Controls.${TRAVIS_TAG}.nupkg -ApiKey ${NUGET_API_KEY} -Source https://api.nuget.org/v3/index.json
nuget push FigmaSharp.Controls.Cocoa.${TRAVIS_TAG}.nupkg -ApiKey ${NUGET_API_KEY} -Source https://api.nuget.org/v3/index.json

