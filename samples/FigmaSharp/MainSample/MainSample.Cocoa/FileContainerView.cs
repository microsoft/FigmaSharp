// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using AppKit;
using CoreGraphics;

namespace FigmaSharpApp
{
	public partial class FileContainerView : NSView
	{
		public FileContainerView (IntPtr handle) : base (handle)
		{
		}

		public override void SetFrameSize (CGSize newSize)
		{
			base.SetFrameSize (newSize);

			foreach (var item in Subviews) {
				if (item is NSScrollView) {
					item.Frame = this.Bounds;
					return;
				}
			}

		}
	}
}
