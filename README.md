# FigmaSharp

*FigmaSharp converts Figma documents into .NET objects and provides the tools to turn them into working native UI views. Free and Open Source software under the [MIT LICENSE]().*

> Here at Microsoft we ❤️ [Figma](https://www.figma.com/). We use it for everything and anything. So much so, we thought why not use it to actually implement our user interfaces? Sounds crazy enough to work. Let's give this a go.


## Getting started

To get documents from [figma.com](https://www.figma.com/) you'll need to generate a **Personal Access Token**.
Sign in to Figma and in the main menu, go to **Help and Account  →  Account Settings** and select **Create new token**.
This will be your only chance to copy the token, so make sure you keep a copy in a secure place.

Restore the external dependencies by running `git submodule update --init --recursive`.

To run the examples, open `FigmaSharp.sln` in [Visual Studio for Mac](https://visualstudio.microsoft.com/vs/mac/).
In each example project's **Project Options**, go to **Run → Configurations → Default** and add an environment variable called `TOKEN`, then paste in your Personal Access Token.


## The FigmaSharp API

There are several ways to load Figma documents into your app:

* [FigmaDocument.FromUrl](FigmaSharp/Helpers/FigmaApiHelper.cs#L95-L99) – to load from a figma.com  URL
* [FigmaDocument.FromFile](FigmaSharp/Helpers/FigmaApiHelper.cs#L101-L105) – to load from a local .figma file
* [FigmaDocument.FromString](FigmaSharp/blob/master/FigmaSharp/Helpers/FigmaApiHelper.cs#L107-L111) – to load from raw data string

Here's how to do it with a URL:

```csharp
using System;
using FigmaSharp;

namespace FigmaSharpExample
{
    static class Main
    {
        // Put your Figma token and URL here
        const string FIGMA_TOKEN = "13c44-b0f20d98-815c-48b7-83de-1f94504b98bd";
        const string FIGMA_URL = "https://www.figma.com/file/QzEga2172k21eMF2s4Nc5keY";

        static void Main (string [] args)
        {
            FigmaEnvironment.SetAccessToken (FIGMA_TOKEN);
            var document = FigmaDocument.FromUrl (FIGMA_URL);
        }
    }
}
```

This results in a `FigmaDocument`, which is a hierarchy of `FigmaNode`s, and some metadata.
[Browse the documentation]() to learn more about everything you can do with a Figma document, .


## Rendering native views and controls

This is where the real magic happens. Being able to use Figma documents in C# is nice enough, but we can go one step further and use them to draw native UI views.

Currently FigmaSharp only supports Cocoa using [Xamarin.Mac](), but others (e.g. WPF, Windows, WinForms) may be added later (contribute!).


### Cocoa

**FigmaSharp.Cocoa** provides renderers and some helper methods to generate `NSView`s from Figma documents dynamically to use in your macOS apps.

* [NSView.FromFigmaFile](FigmaSharp/blob/master/FigmaSharp.Cocoa/FigmaViewExtensions.cs#L44)
* [NSView.FromFigmaUrl](FigmaSharp/blob/master/FigmaSharp.Cocoa/FigmaViewExtensions.cs#L55)
* [NSView.FromFigmaString](FigmaSharp/blob/master/FigmaSharp.Cocoa/FigmaViewExtensions.cs#L80)


```csharp
using System;
using AppKit;

using FigmaSharp;
using FigmaSharp.Views;
using FigmaSharp.Cocoa;

namespace CocoaExample
{
    public partial class ViewController : NSViewController
    {
        const string FIGMA_TOKEN = "13c44-b0f20d98-815c-48b7-83de-1f94504b98bd";
        const string FIGMA_URL = "https://www.figma.com/file/QzEga2172k21eMF2s4Nc5keY";
    
        public ViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            
            FigmaEnvironment.SetAccessToken (FIGMA_TOKEN);
            
            List<FigmaImageView> figmaImageViews; 
            View = NSView.FromFigmaUrl (FIGMA_URL, out figmaImageViews);
            figmaImageViews.Load (FIGMA_URL);
        }
    }
}
```


### Cocoa.NativeControls

Our views are now drawn natively, but what about actual working controls?
Using a [set of special Figma components](https://www.figma.com/file/QzEgq2772k2eeMF2sVNc3kEY/macOS-Components?node-id=7%3A1788), we can tell **FigmaSharp.Cocoa.NativeControls** what layers to render as real controls.


## Bundling Figma files

It's not always possible nor desirable to connect to [figma.com]() to load your documents each time. Here's how you can bundle Figma documents and use them as resources using the `FigmaFile` [API]():

`.figma` files are essentially JSON files aqcuired from the Figma REST API and are accompanied by a `.figma.cs` file that defines all the UI objects. Image files need to be named like their corresponding Figma object ID.

Make sure the name part of each file is the same and and their **Build Action** should be set to `EmbeddedResource`.

In your solution tree it should look something like this:

```
|
|---+ MyDialog.figma
|   |- MyDialog.figma.cs
|   |- MyDialog.cs
|
|---+ Resources/
|   |- icon.png
|   |- icon@2x.png
|
```

Let's take the `MyDialog.figma` file and render the view using Cocoa. We'll need the following two files:


```csharp
// MyDialog.figma.cs
//
// This file has been automatically generated by FigmaSharp 0.1
//
// File: https://www.figma.com/file/QzEgq2772k2eeMF2sVNc3kEY
// Date: 2019-12-03 18:22

using AppKit;

public partial class MyDialog
{
    NSTextField Title;
    NSTexfield Description;

    NSButton CancelButton;
    NSButton DoneButton;
}
```

```csharp
// MyDialog.cs

using FigmaSharp;
using FigmaSharp.Views;
using FigmaSharp.Cocoa;

public partial class MyDialog : FigmaFile
{
    public MyDialog () : base ("MyDialog.figma")
    {
        Initialize ();
        
        Load (withImages: true,
              withControls: true);
    }
}
```

Here `Initialize ()`  sets  `FigmaFile.Document` as a [FigmaDocument]().

`Reload ()` takes the initialized `FigmaDocument` and images and creates a native `NSView` as `FigmaFile.ContentView`, which you can now use in your Cocoa app.

This process can be automated by using the [bundler]() tool included with the…


## Tools

The **FigmaSharp.Tools** folder contains some helpful tools for handling Figma files:

- **Inspector** – preview how your Figma document looks when rendered natively and copy snippets
- **Code Generator** – turns a Figma document into code
- **Bundler** – takes a figma.com URL and downloads the .figma file including all images and adds it to your project

 [This extension](https://www.nuget.org/packages/FigmaSharp/) integrates the inspector and bundler directly into [Visual Studio for Mac](https://visualstudio.microsoft.com/vs/mac/) for ease of use.


## Contributing

Enjoy and hack the planet!
