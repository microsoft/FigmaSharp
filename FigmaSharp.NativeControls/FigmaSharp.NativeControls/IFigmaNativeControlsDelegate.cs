using FigmaSharp.Models;

namespace FigmaSharp
{
	public interface IFigmaNativeControlsDelegate
	{
		FigmaBundleViewBase GetBundleView (FigmaBundle bundle, string name, FigmaNode figmaNode);
	}
}
