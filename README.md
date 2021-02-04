# FigmaSharp – Create apps with Figma

> Here at Microsoft we ❤️ [Figma](https://www.figma.com/). We use it for everything and anything. So much so, we thought why not use it to actually implement our user interfaces? Sounds crazy enough to work. Let's give this a go. 

![A macOS app window created with FigmaSharp](.github/screenshot.png)

FigmaSharp turns your Figma design into .NET objects and can generate code and layout files to create native apps. Free and Open Source software under the [MIT LICENSE]().

[![Build Status](https://travis-ci.org/microsoft/FigmaSharp.svg?branch=master)](https://travis-ci.org/microsoft/FigmaSharp)

# Getting started

We recommend reading through the [Wiki](https://github.com/microsoft/FigmaSharp/wiki) to get a sense of the FigmaSharp workflow.

To get documents from [figma.com](https://www.figma.com/) you'll need to generate a **Personal Access Token**.
Sign in to Figma and in the main menu, go to **Help and Account  →  Account Settings** and select **Create new token**.
This will be your only chance to copy the token, so make sure you keep a copy in a secure place.

Try out automatic builds of the FigmaSharp app and Visual Studio extension from the [Releases](https://github.com/microsoft/FigmaSharp/releases) page. 

Do you have questions, need support, or want to contribute? Join the [chat on Discord](https://discord.gg/F3GEYqp).


## Visual Studio extension

The [Visual Studio for Mac](https://visualstudio.microsoft.com/vs/mac/) extension contains the tools to preview Figma documents and generate Packages ready to use in your projects. Download the .mpack from the [Releases](https://github.com/microsoft/FigmaSharp/releases) page. 


## FigmaSharp app and Visual Studio extension

The [Visual Studio for Mac](https://visualstudio.microsoft.com/vs/mac/) extension contains the tools to preview Figma documents and generate Packages ready to use in your projects. Download the .mpack from the [Releases](https://github.com/netonjm/FigmaSharp/releases) page. Install it via **Visual Studio → Extensions → Install from file…**.

The FigmaSharp app previews Figma documents without having to install Visual Studio. Download the .app from the [Releases](https://github.com/netonjm/FigmaSharp/releases) page.

<br/>


## Building from source

Make sure to have the latest version of [Xcode](https://developer.apple.com/xcode/). Also install Xamarin.Mac, you can get it via the Visual Studio for Mac Installer.

Open `FigmaSharp.Mac.sln` in [Visual Studio for Mac](https://visualstudio.microsoft.com/vs/mac/). For each project in the `Tools` or `Samples` folder you want to run, open the project's **Project Options**, go to **Run → Configurations → Default** and add an environment variable called `TOKEN`, then paste in the **Personal Access Token** for your Figma account.


### Visual Studio for Mac extension

Select `MonoDevelop.Figma`  in the target selector in the toolbar and run it. This will start a new Visual Studio for Mac instance with the extension enabled. Alternatively, you can run `make` in the FigmaSharp source folder. This will generate a **.mpack** file that can be installed on other computers.


### FigmaSharpApp

Select `FigmaSharpApp`  in the target selector in the toolbar and run it.


<br/>

Keep calm and hack the planet.
