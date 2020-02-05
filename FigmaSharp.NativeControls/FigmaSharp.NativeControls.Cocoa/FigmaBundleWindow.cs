using FigmaSharp;
using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.NativeControls;
using FigmaSharp.NativeControls.Cocoa;
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
			builder.AppendLine ("//HACK: Temporal Window Frame Size");
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
