#General vars
ARGS?=/restore /p:Configuration=Release

all:
	echo "Building FigmaSharp.Mac..."
	msbuild /restore FigmaSharp.Mac.sln

clean:
	find . -type d -name bin -exec rm -rf {} \;
	find . -type d -name obj -exec rm -rf {} \;
	find . -type d -name packages -exec rm -rf {} \;

pack:
	msbuild FigmaSharp.Mac.sln $(ARGS) /p:CreatePackage=true

install:
	msbuild FigmaSharp.Mac.sln $(ARGS) /p:InstallAddin=true

check-dependencies:
	#updating the submodules
	git submodule update --init --recursive

	if [ ! -f ./nuget.exe ]; then \
	    echo "nuget.exe not found! downloading latest version" ; \
	    curl -O https://dist.nuget.org/win-x86-commandline/latest/nuget.exe ; \
	fi

sdk: nuget-download
	mono src/.nuget/nuget.exe pack FigmaSharp.Mac.nuspec

package: check-dependencies

	msbuild build.proj
	# msbuild FigmaSharp/FigmaSharp/FigmaSharp.csproj /p:Configuration=Release /restore
	# msbuild FigmaSharp/FigmaSharp.Cocoa/FigmaSharp.Cocoa.csproj /p:Configuration=Release /restore

	# mono nuget.exe restore FigmaSharp.Mac.sln
	# msbuild FigmaSharp/FigmaSharp/FigmaSharp.csproj $(ARGS)
	# msbuild FigmaSharp/FigmaSharp.Cocoa/FigmaSharp.Cocoa.csproj $(ARGS)

	# msbuild FigmaSharp.Controls/FigmaSharp.Controls/FigmaSharp.Controls.csproj $(ARGS)
	# msbuild FigmaSharp.Controls/FigmaSharp.Controls.Cocoa/FigmaSharp.Controls.Cocoa.csproj $(ARGS)

	# mono nuget.exe pack NuGet/FigmaSharp.Views.nuspec 
	# mono nuget.exe pack NuGet/FigmaSharp.Views.Cocoa.nuspec

	# mono nuget.exe pack NuGet/FigmaSharp.nuspec
	# mono nuget.exe pack NuGet/FigmaSharp.Cocoa.nuspec

	# mono nuget.exe pack NuGet/FigmaSharp.Controls.nuspec
	# mono nuget.exe pack NuGet/FigmaSharp.Controls.Cocoa.nuspec

.PHONY: all clean pack install submodules sdk nuget-download check-dependencies
