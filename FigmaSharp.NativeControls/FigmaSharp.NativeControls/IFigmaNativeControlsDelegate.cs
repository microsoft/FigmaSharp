using FigmaSharp.Models;

namespace FigmaSharp
{
	public interface IFigmaNativeControlsDelegate
	{
		FigmaBundleViewBase GetBundleView (FigmaBundle bundle, string name, FigmaNode figmaNode);

		FigmaCodePropertyConverterBase GetCodePropertyConverter ();

		FigmaViewConverter[] GetConverters (bool includeAll = true);
	}
}
