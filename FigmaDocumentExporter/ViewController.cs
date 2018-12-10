using System;

using AppKit;
using Foundation;
using FigmaSharp;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace FigmaDocumentExporter
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
		
		}
		const string outputFile = "downloaded.figma";
		async partial void DownloadAction (NSObject sender)
		{
			var outputDirectory = outputPathTextField.StringValue;
			if (!Directory.Exists (outputDirectory)) {
				Directory.CreateDirectory (outputDirectory);
			}

            var fileId = documentTextField.StringValue;

            var content = FigmaHelper.GetFigmaFileContent (fileId);

            var outputFilePath = Path.Combine (outputDirectory, outputFile);
            if (File.Exists (outputFilePath)) {
                File.Delete (outputFilePath);
            }
            File.WriteAllText (outputFilePath, content);

            var mainNode = FigmaHelper.GetFigmaDialogFromContent (content) as FigmaNode;
            var figmaModelImages = mainNode.OfTypeImage ().ToArray();
            var figmaImageIds = figmaModelImages.Select(s => s.ID).ToArray();

            var alert = new NSAlert();

            if (figmaImageIds.Length > 0)
            {
                var figmaResponse = FigmaHelper.GetFigmaImages(fileId, figmaImageIds);
                var urls = figmaResponse.images.Select(s => s.Value).ToArray();
                await FigmaHelper.SaveFilesAsync(outputDirectory, ".png", urls);

                alert.MessageText = "Process finished correctly";
            }
            else
            {
                alert.MessageText = "No images detected in the Figma File";
            }

            alert.AddButton("Close");
            alert.RunModal();
        }

		public override NSObject RepresentedObject {
			get {
				return base.RepresentedObject;
			}
			set {
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}
	}
}
