using FigmaSharp.Services;
using FigmaSharp.Models;

namespace FigmaSharp
{
	public class FigmaBundleView : FigmaBundleViewBase
	{
		public FigmaBundleView (FigmaBundle figmaBundle, string viewName, FigmaNode figmaNode) : base (figmaBundle, viewName, figmaNode)
		{
		}

		protected override void OnGetPartialDesignerClass (FigmaPartialDesignerClass partialDesignerClass, FigmaCodeRendererService codeRendererService)
		{
			if (FigmaNode == null)
				return;

			partialDesignerClass.Usings.Add (nameof (AppKit));

			//restore this state
			var builder = new System.Text.StringBuilder ();
			codeRendererService.GetCode (builder, new FigmaCodeNode (FigmaNode, null), null);
			partialDesignerClass.InitializeComponentContent = builder.ToString ();
		}

		protected override void OnGetPublicDesignerClass (FigmaPublicPartialClass publicPartialClass)
		{
			publicPartialClass.Usings.Add (nameof (AppKit));
			publicPartialClass.BaseClass = typeof (AppKit.NSView).FullName;
		}
	}
}
