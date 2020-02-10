using FigmaSharp;
using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.NativeControls;
using FigmaSharp.NativeControls.Cocoa;
using System.Linq;
using System;
using AppKit;

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
