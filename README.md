# FigmaSharp – Create apps with Figma

> Here at Microsoft we ❤️ [Figma](https://www.figma.com/). We use it for everything and anything. So much so, we thought why not use it to actually implement our user interfaces? Sounds crazy enough to work. Let's give this a go. 

![A macOS app window created with FigmaSharp](.github/screenshot.png)

FigmaSharp turns your Figma design into .NET objects and can generate code and layout files to create native apps. Free and Open Source software under the [MIT LICENSE]().

[![Build Status](https://travis-ci.org/netonjm/FigmaSharp.svg?branch=master)](https://travis-ci.org/netonjm/FigmaSharp)

# Getting started

We recommend reading through the [Wiki](https://github.com/netonjm/FigmaSharp/wiki) to get a sense of the FigmaSharp workflow.

To get documents from [figma.com](https://www.figma.com/) you'll need to generate a **Personal Access Token**.
Sign in to Figma and in the main menu, go to **Help and Account  →  Account Settings** and select **Create new token**.
This will be your only chance to copy the token, so make sure you keep a copy in a secure place.

Try out automatic builds of the FigmaSharp app and Visual Studio extension from the [Releases](https://github.com/netonjm/FigmaSharp/releases) page. 

Do you have questions, need support, or want to contribute? Join the [chat on Discord](https://discord.com/channels/710495994667728996/710495994667728999).

Do you want to know more?

[![FigmaSharp demo](https://img.youtube.com/vi/mr8HjS4rb9E/0.jpg)](https://www.youtube.com/watch?v=mr8HjS4rb9E)


<br/>


## Visual Studio extension

The [Visual Studio for Mac](https://visualstudio.microsoft.com/vs/mac/) extension contains the tools to preview Figma documents and bundle them into your projects. Download the .mpack from the [Releases](https://github.com/netonjm/FigmaSharp/releases) page. 


<br/>


## FigmaSharp app

The app previews Figma documents without having to install Visual Studio. Download the .app from the [Releases](https://github.com/netonjm/FigmaSharp/releases) page.


<br/>


## Building from source

To run the samples, open `FigmaSharp.Mac.sln` in [Visual Studio for Mac](https://visualstudio.microsoft.com/vs/mac/).
In each sample project's **Project Options**, go to **Run → Configurations → Default** and add an environment variable called `TOKEN`, then paste in your Personal Access Token.
 
