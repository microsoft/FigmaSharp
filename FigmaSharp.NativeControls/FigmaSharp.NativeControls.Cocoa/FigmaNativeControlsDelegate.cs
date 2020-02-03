using FigmaSharp.Models;
using System.Linq;

namespace FigmaSharp.NativeControls.Cocoa
{
	public class FigmaNativeControlsDelegate : IFigmaNativeControlsDelegate
	{
		public FigmaBundleViewBase GetBundleView (FigmaBundle bundle, string name, FigmaNode figmaNode)
		{
			if (figmaNode is IFigmaNodeContainer nodeContainer) {
				foreach (var figmaInstance in nodeContainer.children.OfType<FigmaInstance> ()) {
					if (figmaInstance.ToNativeControlType () == NativeControlType.WindowStandard) 
						return new FigmaBundleWindow (bundle, name, figmaNode);
					if (figmaInstance.ToNativeControlType () == NativeControlType.WindowSheet)
						return new FigmaBundleWindow (bundle, name, figmaNode);
					if (figmaInstance.ToNativeControlType () == NativeControlType.WindowPanel)
						return new FigmaBundleWindow (bundle, name, figmaNode);
				}
			}
			return new FigmaBundleView (bundle, name, figmaNode);
		}
	}
}
