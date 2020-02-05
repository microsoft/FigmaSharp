using FigmaSharp.Models;
using FigmaSharp.NativeControls.Cocoa.Converters;
using System.Linq;

namespace FigmaSharp.NativeControls.Cocoa
{
	public class FigmaNativeControlsDelegate : IFigmaNativeControlsDelegate
	{
		static readonly FigmaCodePropertyConverterBase addNativeChildConverter = new NativeControlsPropertyConverter ();
		static FigmaViewConverter[] allConverters;
		static FigmaViewConverter[] converters;

		public FigmaCodePropertyConverterBase GetCodePropertyConverter ()
		  => addNativeChildConverter;

		public FigmaViewConverter[] GetConverters (bool includeAll = true)
		{
			if (converters == null) {
				converters = new FigmaViewConverter[] {
					new SpinnerConverter (),
					new CheckConverter (),
					new ComboBoxConverter (),
					new PopUpButtonConverter (),
					new RadioConverter (),
					new ButtonConverter (),
					new TextFieldConverter (),
				};
			}

			if (includeAll) {
				if (allConverters == null) {
					allConverters = AppContext.Current.GetFigmaConverters ().Concat (converters).ToArray ();
				}
				return allConverters;
			} else {
				return converters;
			}
		}

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
