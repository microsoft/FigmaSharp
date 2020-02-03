using System;
using FigmaSharp.Models;

namespace FigmaSharp
{
	public class NativeControlsContext : IFigmaNativeControlsDelegate
    {
        IFigmaNativeControlsDelegate figmaDelegate;

        public FigmaBundleViewBase GetBundleView (FigmaBundle bundle, string name, FigmaNode figmaNode)
			=> figmaDelegate.GetBundleView (bundle, name, figmaNode);

        #region Static Methods

        static NativeControlsContext current;
        /// <summary>
        /// The shared AppContext for the application.
        /// </summary>
        /// <value>The current.</value>
        public static NativeControlsContext Current {
            get {
                if (current == null) {
                    current = new NativeControlsContext ();
                }
                return current;
            }
        }

		internal void Configuration (IFigmaNativeControlsDelegate applicationDelegate)
		{
            figmaDelegate = applicationDelegate;
        }

		#endregion
	}
}
