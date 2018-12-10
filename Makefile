#General vars
CONFIG?=Debug
ARGS:=/p:Configuration="${CONFIG}" $(ARGS)

all:
	echo "Building FigmaSharp..."
	msbuild FigmaSharp.sln $(ARGS)

clean:
	find . -type d -name bin -exec rm -rf {} \;
	find . -type d -name obj -exec rm -rf {} \;
	find . -type d -name packages -exec rm -rf {} \;

pack:
	msbuild MonoDevelop.Figma/MonoDevelop.Figma.csproj $(ARGS) /p:CreatePackage=true

install:
	msbuild MonoDevelop.Figma/MonoDevelop.Figma.csproj $(ARGS) /p:InstallAddin=true

check-dependencies:
	#if test "x$(CONFIG)" = "xDebug" || test "x$(CONFIG)" = "xRelease"; then \
	#	./bot-provisioning/system_dependencies.sh || exit 1; \
	#fi

nuget-download:
	# nuget restoring
	if [ ! -f src/.nuget/nuget.exe ]; then \
		mkdir -p src/.nuget ; \
	    echo "nuget.exe not found! downloading latest version" ; \
	    curl -O https://dist.nuget.org/win-x86-commandline/latest/nuget.exe ; \
	    mv nuget.exe src/.nuget/ ; \
	fi

submodules: nuget-download
	echo "Restoring FigmaSharp..."
	msbuild FigmaSharp.sln /t:Restore

sdk: nuget-download
	mono src/.nuget/nuget.exe pack FigmaSharp.nuspec

template:
	./install.sh
	rm -rf src/IDE/Xamarin.VisualStudio.IoT.Mac/templates/Xamarin.Templates.IoT.0.0.1.nupkg
	mv Xamarin.Templates.IoT.0.0.1.nupkg src/IDE/Xamarin.VisualStudio.IoT.Mac/templates

vsts:
	echo "THIS TARGET IS ONLY TO GENERATE THE PACKAGE FOR VSTS"
	msbuild src/IDE/Xamarin.VisualStudio.IoT.Mac/Xamarin.VisualStudio.IoT.csproj $(ARGS) /p:CreatePackage=true
	echo "DONE!"



.PHONY: all clean pack install submodules sdk nuget-download check-dependencies
