# FigmaSharp + UI Kit  <img src="https://github.com/netonjm/FigmaSharp/blob/master/icons/image-logo.png" data-canonical-src="https://github.com/netonjm/FigmaSharp/blob/master/icons/image-logo.png" width="50" />

FigmaSharp turns your Figma designs into working code.

![](https://github.com/netonjm/FigmaSharp/blob/master/icons/figmasharp-show.gif)


Right now FigmaSharp only supports Cocoa, but more UI frameworks may be added later.

| Framework                   | Status                    | 
| --------------------------- | ------------------------- |
| Cocoa (with Xamarin.Mac)    | Implemented               |
| Xamarin.Forms               | In progress…              |
| WPF                         | In progress…              |
| WinForms                    | Not implemented           |


# Setting Up

To get documents from Figma you'll need to generate a Personal Access Token. Sign in to Figma and in the main menu, go to `Help and Account` -> `Account Settings` and `Create new token`. This will be your only chance to copy the token, so make sure you keep a copy of this in a secure place.


```CSharp
using FigmaSharp;

namespace ExampleFigmaMac
{
    static class MainClass
    {
        const string FIGMA_TOKEN = "13c44-b0f20d98-815c-48b7-83de-1f94504b98bd";
     
        static void Main(string[] args)
        {
                FigmaEnvironment.SetAccessToken (FIGMA_TOKEN);
                FigmaResponse response = FigmaHelper.GetFigmaDialogFromUrlFile(urlFile);
        }

    }
}
```

`FigmaResponse` contains a `FigmaDocument` and metadata. This is the main way of getting documents from figma.com.






# How to use


To get all models from Figma we provide some helper methods


* [FigmaHelper.GetFigmaDialogFromUrlFile](https://github.com/netonjm/FigmaSharp/blob/master/FigmaSharp/Helpers/FigmaApiHelper.cs#L95-L99)

* [FigmaHelper.GetFigmaDialogFromFilePath](https://github.com/netonjm/FigmaSharp/blob/master/FigmaSharp/Helpers/FigmaApiHelper.cs#L101-L105)

* [FigmaHelper.GetFigmaDialogFromContent](https://github.com/netonjm/FigmaSharp/blob/master/FigmaSharp/Helpers/FigmaApiHelper.cs#L107-L111)


This returns a [IFigmaDocumentContainer](https://github.com/netonjm/FigmaSharp/blob/master/FigmaSharp/FigmaModels.cs#L164-L167) which inherits from IFigmaNodeContainer


All this methods internally deserialize a FigmaResponse class using a FigmaResponseConverter generating a hierarchy of [FigmaNode](https://github.com/netonjm/FigmaSharp/blob/master/FigmaSharp/FigmaModels.cs)'s.


```CSharp

using FigmaSharp;

namespace ExampleFigmaMac
{
    static class MainClass
    {
        static void Main(string[] args)
        {
		FigmaEnvironment.SetAccessToken ("YOUR TOKEN");
		FigmaResponse response = FigmaHelper.GetFigmaDialogFromUrlFile(urlFile);
        }

    }
}

```

As simple as this!!

Hey! It would be great generate UI's on the fly with this models! It's possible to do it with this Toolkit?
* Of course yes! *

# FigmaSharp UI Kit

FigmaSharp UI kit adds tools to generate easy Views of your favorite GUI Frameworks(like Forms, WPF, WinForms) as demand.


All this code platform specific is included in FigmaSharp.Cocoa library, this provides some extension methods to generate dynamically NSViews on the fly based in our provided model.


* [LoadFigmaFromFilePath](https://github.com/netonjm/FigmaSharp/blob/master/FigmaSharp.Cocoa/FigmaViewExtensions.cs#L44)


* [LoadFigmaFromUrlFile](https://github.com/netonjm/FigmaSharp/blob/master/FigmaSharp.Cocoa/FigmaViewExtensions.cs#L55)


* [LoadFigmaFromResource](https://github.com/netonjm/FigmaSharp/blob/master/FigmaSharp.Cocoa/FigmaViewExtensions.cs#L65)


* [LoadFigmaFromContent](https://github.com/netonjm/FigmaSharp/blob/master/FigmaSharp.Cocoa/FigmaViewExtensions.cs#L80)


* [LoadFigmaFromFrameEntity](https://github.com/netonjm/FigmaSharp/blob/master/FigmaSharp.Cocoa/FigmaViewExtensions.cs#L94)


All this extension methods work over any NSView or NSWindow entities.



```CSharp

using FigmaSharp;

namespace ExampleFigmaMac
{
	public partial class ViewController : NSViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Do any additional setup after loading the view.
			FigmaEnvirontment.SetAccessToken ("YOUR TOKEN");

			var stackView = new NSStackView () { Orientation = NSUserInterfaceLayoutOrientation.Vertical };

			List<FigmaImageView> figmaImageViews; // you will get all the images in this array

			View.LoadFigmaFromUrlFile (fileName, out figmaImageViews);

			figmaImageViews.Load (fileName); // request to Figma API all the images and converts into NSImage files
		}
	}
}

```

Hey I love all of this! but… why not create a standard type of file to pack all of this? I want work with local files in my project!!

# Introducing Figma Files (in preview)

Figma files is a new way to generate local Figma storyboards with images.

Structure:

--  ExampleFigmaFile.figma (json)

|_ ExampleFigmaFile.cs
 
  |_ FigmaFile
       

Figma files are basically a .JSON file (provided by Figma API) and the code behind .cs class which is based in our [FigmaFile](https://github.com/netonjm/FigmaSharp/blob/master/FigmaSharp.Cocoa/FigmaFile.cs). All the image files needs to be named like its corresponding figmaId and include in your project like a EmbeddedResource. 


Figma files are basically a .JSON file (provided by Figma API) and the code behind .cs class which is based in our [FigmaFile](https://github.com/netonjm/FigmaSharp/blob/master/FigmaSharp.Cocoa/FigmaFile.cs)

This is an example


```csharp
public class FigmaStoryboard : FigmaFile
{
	public FigmaStoryboard () : base ("FigmaStoryboard.figma")
	{
		Initialize ();
	}
}
```

Note: the base class initializes with your json FileName


The *Initialize()* method, generates all figma models into FigmaStoryboard.Document hierarchy.. but wait this don't generate views yet!


If you want to generate it you will need to do an extra call!


```csharp
public class FigmaStoryboard : FigmaFile
{
	public FigmaStoryboard () : base ("FigmaStoryboard.figma")
	{
		Initialize ();
      		Reload (true); //Reload including images
	}
}
```

Call to: **Reload (true)** creates all the native views into your [FigmaStoryboard.ContentView](https://github.com/netonjm/FigmaSharp/blob/master/FigmaSharp.Cocoa/FigmaFile.cs#L14) property and also your  images in [FigmaImages](https://github.com/netonjm/FigmaSharp/blob/master/FigmaSharp.Cocoa/FigmaFile.cs#L12) property.


Wow!! That's cool! But I am not sure how generate this automagically!



# Figma Local Exporter (only mac for now)

In the source code we provided a simple tool to export Figma files from remote Figma API

This tools downloads your .figma storyboard and all the images on it into an output directory. 


1) Type your Figma file name

2) Type your output directory

3) Press the button!!



Result: this will download and generate all files into output directory



<img src="https://github.com/netonjm/FigmaSharp/blob/master/icons/FigmaExporter.png" data-canonical-src="https://github.com/netonjm/FigmaSharp/blob/master/icons/FigmaExporter.png" />


**Now in your solution project**

1) Rename your **downloaded.figma** file to **yourdesiredname.figma** and copy it to your project and set the Build action to **EmbeddedResource**.

2) Copy all the images in your Resources directory and set the Build action to **EmbeddedResource**.

3) Generate a .cs file like we described in FigmaFiles section



**Congrats!! you created your first figma.file!**



# Figma Renderer for VSforMac


We also included a Figma Renderer to show Figma.Files in Visual Studio for Mac!!!!!

<img src="https://github.com/netonjm/FigmaSharp/blob/master/icons/FigmaRenderer.png" data-canonical-src="https://github.com/netonjm/FigmaSharp/blob/master/icons/FigmaRenderer.png" />



Download Figma Extension from Extension Gallery and have a Fun time!


<img src="https://github.com/netonjm/FigmaSharp/blob/master/icons/VSMacExtension.png" data-canonical-src="https://github.com/netonjm/FigmaSharp/blob/master/icons/VSMacExtension.png" />

# Other interesting tools

You can combine this awesome library with realtime .NET Inspector with a simple NuGet!

https://github.com/netonjm/MonoDevelop.Mac.Debug


# Future

* Other GUI frameworks integration
* Code generation 
* Extension: Include drag and drop to Source Editor 

Easy as this!


Contribute and hope you enjoy!


Hack the planet.


MIT LICENSE

