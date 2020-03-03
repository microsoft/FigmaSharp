#General vars
CONFIG?=Debug
ARGS:=/p:Configuration="${CONFIG}" $(ARGS)

all:
	echo "Building FigmaSharp.Mac..."
	msbuild FigmaSharp.Mac.sln $(ARGS)

clean:
	find . -type d -name bin -exec rm -rf {} \;
	find . -type d -name obj -exec rm -rf {} \;
	find . -type d -name packages -exec rm -rf {} \;

pack:
	msbuild FigmaSharp.Mac.sln $(ARGS) /p:CreatePackage=true /restore

install:
	msbuild FigmaSharp.Mac.sln $(ARGS) /p:InstallAddin=true /restore

check-dependencies:
	#updating the submodules
	git submodule update --init --recursive

	if [ ! -f ./nuget.exe ]; then \
	    echo "nuget.exe not found! downloading latest version" ; \
	    curl -O https://dist.nuget.org/win-x86-commandline/latest/nuget.exe ; \
	fi

submodules: nuget-download
	echo "Restoring FigmaSharp.Mac..."
	msbuild FigmaSharp.Mac.sln /restore

sdk: nuget-download
	mono src/.nuget/nuget.exe pack FigmaSharp.Mac.nuspec

package: check-dependencies
	msbuild FigmaSharp/FigmaSharp/FigmaSharp.csproj /p:Configuration=Release /restore
	msbuild FigmaSharp/FigmaSharp.Cocoa/FigmaSharp.Cocoa.csproj /p:Configuration=Release /restore

	mono nuget.exe restore FigmaSharp.sln
	msbuild FigmaSharp/FigmaSharp/FigmaSharp.csproj /p:Configuration=Release /restore
	msbuild FigmaSharp/FigmaSharp.Cocoa/FigmaSharp.Cocoa.csproj /p:Configuration=Release /restore

	msbuild FigmaSharp.NativeControls/FigmaSharp.NativeControls/FigmaSharp.NativeControls.csproj /p:Configuration=Release /restore
	msbuild FigmaSharp.NativeControls/FigmaSharp.NativeControls.Cocoa/FigmaSharp.NativeControls.Cocoa.csproj /p:Configuration=Release /restore

	mono nuget.exe pack NuGet/FigmaSharp.Views.nuspec
	mono nuget.exe pack NuGet/FigmaSharp.Views.Cocoa.nuspec

	mono nuget.exe pack NuGet/FigmaSharp.nuspec
	mono nuget.exe pack NuGet/FigmaSharp.Cocoa.nuspec

	mono nuget.exe pack NuGet/FigmaSharp.NativeControls.nuspec
	mono nuget.exe pack NuGet/FigmaSharp.NativeControls.Cocoa.nuspec

.PHONY: all clean pack install submodules sdk nuget-download check-dependencies
