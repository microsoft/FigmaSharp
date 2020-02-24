/* 
 * CustomTextFieldConverter.cs
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
using System.Linq;

using AppKit;

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.NativeControls;
using FigmaSharp.Services;

namespace FigmaSharp
{
    public class FigmaBundleWindow : FigmaBundleViewBase
	{
		public FigmaBundleWindow (FigmaBundle figmaBundle, string viewName, FigmaNode figmaNode) : base (figmaBundle, viewName, figmaNode)
		{
		}

		const string frameEntity = "frame";

		protected override void OnGetPartialDesignerClass (FigmaPartialDesignerClass partialDesignerClass, FigmaCodeRendererService codeRendererService)
		{
			if (FigmaNode == null)
				return;

			partialDesignerClass.Usings.Add (nameof (AppKit));

			//restore this state
			var builder = new System.Text.StringBuilder ();
			//builder.AppendLine ("//HACK: Temporal Window Frame Size");

			if (FigmaNode is IFigmaNodeContainer container) {
				var properties = container.children
					.FirstOrDefault (s => s.name == "Properties" && s.visible == false);
				if (properties != null) {

				}
			}

			//default configuration status bar
			builder.AppendLine (string.Format ("{0}.{1} |= {2};",
				CodeGenerationHelpers.This,
				nameof (AppKit.NSWindow.StyleMask),
				AppKit.NSWindowStyle.Closable.GetFullName ()
			));

			builder.AppendLine(string.Format("{0}.{1} |= {2};",
				CodeGenerationHelpers.This,
				nameof(AppKit.NSWindow.StyleMask),
				AppKit.NSWindowStyle.Resizable.GetFullName()
			));

			var windowComponent = FigmaNode.GetDialogInstanceFromParentContainer () as FigmaInstance;
			if (windowComponent != null) {

				if (windowComponent.TryGetNativeControlComponentType (out var nativeControlComponentType)) {
					windowComponent.TryGetNativeControlType (out var nativeControlType);


					if (nativeControlType == NativeControlType.WindowStandard) {
						var title = windowComponent.children.OfType<FigmaText> ().FirstOrDefault (s => s.name == "window title");
						if (title != null) {
							builder.WriteEquality (CodeGenerationHelpers.This, nameof (AppKit.NSWindow.Title), title.characters ?? "", inQuotes: true);
						}
					}

					string appareanceStyle;
					if (nativeControlComponentType.ToString ().EndsWith ("Dark", StringComparison.Ordinal)) {
						appareanceStyle = $"{typeof (NSAppearance).FullName}.{nameof (NSAppearance.NameDarkAqua)}";
					} else {
						appareanceStyle = $"{typeof (NSAppearance).FullName}.{nameof (NSAppearance.NameVibrantLight)}";
					}

					var getAppareanceMethod = CodeGenerationHelpers.GetMethod (typeof (NSAppearance).FullName, nameof (NSAppearance.GetAppearance), appareanceStyle);
					builder.WriteEquality (CodeGenerationHelpers.This, nameof (NSWindow.Appearance), getAppareanceMethod);
				}
			}

			if (FigmaNode is IAbsoluteBoundingBox box) {
				builder.WriteEquality (frameEntity, null, nameof (AppKit.NSWindow.Frame), instanciate: true);

				string instance = typeof (CoreGraphics.CGSize).
					GetConstructor (new string[] {
						box.absoluteBoundingBox.Width.ToDesignerString (),
						box.absoluteBoundingBox.Height.ToDesignerString ()
					});
				builder.WriteEquality (frameEntity, nameof (AppKit.NSWindow.Frame.Size), instance, instanciate: false);

				string parameters = $"{frameEntity},{true.ToDesignerString ()}";

				builder.WriteMethod (CodeGenerationHelpers.This, nameof (AppKit.NSWindow.SetFrame), parameters);

				builder.AppendLine ();
			}

			codeRendererService.GetCode (builder, new FigmaCodeNode(FigmaNode, null), null);
			partialDesignerClass.InitializeComponentContent = builder.ToString ();
		}

		protected override void OnGetPublicDesignerClass (FigmaPublicPartialClass publicPartialClass)
		{
			publicPartialClass.Usings.Add (nameof (AppKit));
			publicPartialClass.BaseClass = typeof (AppKit.NSWindow).FullName;
		}
	}
}
