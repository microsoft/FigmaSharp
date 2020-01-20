#!/bin/bash

msbuild FigmaSharp/FigmaSharp/FigmaSharp.csproj /p:Configuration=Release /restore
msbuild FigmaSharp/FigmaSharp.Cocoa/FigmaSharp.Cocoa.csproj /p:Configuration=Release /restore
msbuild FigmaSharp.NativeControls/FigmaSharp.NativeControls/FigmaSharp.NativeControls.csproj /p:Configuration=Release /restore
msbuild FigmaSharp.NativeControls/FigmaSharp.NativeControls.Cocoa/FigmaSharp.NativeControls.Cocoa.csproj /p:Configuration=Release /restore

nuget pack -Version ${TRAVIS_TAG} NuGet/FigmaSharp.nuspec
nuget pack -Version ${TRAVIS_TAG} NuGet/FigmaSharp.Cocoa.nuspec
nuget pack -Version ${TRAVIS_TAG} NuGet/FigmaSharp.Views.nuspec
nuget pack -Version ${TRAVIS_TAG} NuGet/FigmaSharp.Views.Cocoa.nuspec
nuget pack -Version ${TRAVIS_TAG} NuGet/FigmaSharp.NativeControls.nuspec
nuget pack -Version ${TRAVIS_TAG} NuGet/FigmaSharp.NativeControls.Cocoa.nuspec

nuget push FigmaSharp.${TRAVIS_TAG} .nupkg -ApiKey ${NUGET_API_KEY} -Source https://api.nuget.org/v3/index.json  
nuget push FigmaSharp.${TRAVIS_TAG}.nupkg -ApiKey ${NUGET_API_KEY} -Source https://api.nuget.org/v3/index.json
nuget push FigmaSharp.Views.${TRAVIS_TAG}.nupkg -ApiKey ${NUGET_API_KEY} -Source https://api.nuget.org/v3/index.json
nuget push FigmaSharp.Views.Cocoa.${TRAVIS_TAG}.nupkg -ApiKey ${NUGET_API_KEY} -Source https://api.nuget.org/v3/index.json
nuget push FigmaSharp.NativeControls.${TRAVIS_TAG}.nupkg -ApiKey ${NUGET_API_KEY} -Source https://api.nuget.org/v3/index.json
nuget push FigmaSharp.NativeControls.Cocoa.${TRAVIS_TAG}.nupkg -ApiKey ${NUGET_API_KEY} -Source https://api.nuget.org/v3/index.json

