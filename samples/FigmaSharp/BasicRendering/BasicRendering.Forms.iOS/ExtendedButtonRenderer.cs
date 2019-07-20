using System.ComponentModel;
using FigmaSharp.Views.Forms;
using LiteForms.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedButton), typeof(ExtendedButtonRenderer))]
namespace LiteForms.Forms
{
	public class ExtendedButtonRenderer : Xamarin.Forms.Platform.iOS.ButtonRenderer
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
			Control.HorizontalAlignment = Element.HorizontalTextAlignment == Xamarin.Forms.TextAlignment.Center ? UIKit.UIControlContentHorizontalAlignment.Center :
				Element.HorizontalTextAlignment == Xamarin.Forms.TextAlignment.End ? UIKit.UIControlContentHorizontalAlignment.Right : UIKit.UIControlContentHorizontalAlignment.Left;
		}
	}
}
