using System.ComponentModel;
using Android.Views;
using FigmaSharp.Views.Forms;
using LiteForms.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExtendedButton), typeof(ExtendedButtonRenderer))]
namespace LiteForms.Forms
{
	public static class AlignmentHelper
	{
		[System.Obsolete]
		public static GravityFlags ToHorizontalGravityFlags(this Xamarin.Forms.TextAlignment alignment)
		{
			if (alignment == Xamarin.Forms.TextAlignment.Center)
				return GravityFlags.AxisSpecified;
			return alignment == Xamarin.Forms.TextAlignment.End ? GravityFlags.Right : GravityFlags.Left;
		}

	}

	[System.Obsolete]
	public class ExtendedButtonRenderer : Xamarin.Forms.Platform.Android.ButtonRenderer
	{
		public new ExtendedButton Element
		{
			get
			{
				return (ExtendedButton)base.Element;
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement == null)
			{
				return;
			}

			SetTextAlignment();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == ExtendedButton.HorizontalTextAlignmentProperty.PropertyName)
			{
				SetTextAlignment();
			}
		}

		public void SetTextAlignment()
		{
			Control.Gravity = AlignmentHelper.ToHorizontalGravityFlags(Element.HorizontalTextAlignment);
		}

	}
}
