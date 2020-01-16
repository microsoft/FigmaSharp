/* 
 * FigmaDisplayBinding.cs 
 * 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *
 * Copyright (C) 2018 Microsoft, Corp
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.IO;
using CoreGraphics;
using FigmaSharp;
using FigmaSharp.Cocoa;
using FigmaSharp.Services;
using FigmaSharp.Views;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Projects;

namespace MonoDevelop.Figma.Commands
{
	public class FigmaOpenBundleFormCommandHandler : CommandHandler
	{
		const string AddFigmaDocumentSource = "AddFigmaDocument.figma";

		protected override void Update (CommandInfo info)
		{
			info.Visible = info.Enabled = IdeApp.ProjectOperations.CurrentSelectedItem is Project ||
				(
					IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder folder
					&& folder.IsFigmaBundleDirectory ()
				);
		}
	
		protected override void Run ()
		{
			var window = new Gtk.Dialog ();
			window.Modal = true;
			window.SetSizeRequest (200, 300);
			window.Title = "Figma Bundle Generator";

			var fileProvider = new FigmaManifestFileProvider (this.GetType ().Assembly, AddFigmaDocumentSource);
			fileProvider.Load (AddFigmaDocumentSource);

			var converters = FigmaSharp.NativeControls.Cocoa.Resources.GetConverters ();
			var rendererService = new FigmaViewRendererService (fileProvider, converters);
			var rendererOptions = new FigmaViewRendererServiceOptions () { ScanChildrenFromFigmaInstances = false };
			var view = rendererService.RenderByName<IView> ("1.0. Bundle Figma Document", rendererOptions);

			//var urlTextField = rendererService.Find<TextBox> ("FigmaUrlTextField");
			//var bundleButton = rendererService.FindViewByName<Button> ("BundleButton");
			//var cancelButton = rendererService.FindViewByName<Button> ("CancelButton");

			var gtkViewHost = new Gtk.GtkNSViewHost (view.NativeObject as AppKit.NSView);
			if (window.Child is Gtk.VBox vb) {
				vb.PackStart (gtkViewHost, true, true, 0);
				vb.ShowAll ();
			}
			MessageService.ShowCustomDialog (window, IdeApp.Workbench.RootWindow);
		}
	}

	public class FigmaInitCommand : CommandHandler
	{
		protected override void Run ()
		{
			Resources.Init ();
			PropertyPad.Instance.Initialize ();
			FigmaApplication.Init (FigmaRuntime.Token);
		}
	}
}


