using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FigmaSharp.Wpf
{
    public static class ViewConfigureExtensions
    {
        public static void Configure(this FrameworkElement view, FigmaFrameEntity child)
        {
            Configure(view, (FigmaNode)child);
            view.Opacity = child.opacity;
            if (view is Panel canvas)
            {
                canvas.Background = child.backgroundColor.ToColor();
            }
        }

        public static void Configure(this FrameworkElement view, FigmaNode child)
        {
            view.Visibility = child.visible ? Visibility.Visible : Visibility.Collapsed;

            if (child is IAbsoluteBoundingBox container)
            {
                view.Width = (int) container.absoluteBoundingBox.width;
                view.Height = (int) container.absoluteBoundingBox.height;
            }
        }

        public static void Configure(this UserControl view, FigmaElipse elipse)
        {
            Configure(view, (FigmaVectorEntity)elipse);
        }

        public static void Configure(this UserControl figmaLineView, FigmaLine figmaLine)
        {
            Configure(figmaLineView, (FigmaVectorEntity)figmaLine);

            var fills = figmaLine.fills.OfType<FigmaPaint> ().FirstOrDefault ();
            if (fills != null) {
                figmaLineView.Background = fills.color.ToColor ();
            }

            var absolute = figmaLine.absoluteBoundingBox;
            var lineWidth = absolute.width == 0 ? figmaLine.strokeWeight : absolute.width;

            figmaLineView.Width = (int)lineWidth;

            var lineHeight = absolute.height == 0 ? figmaLine.strokeWeight : absolute.height;

            figmaLineView.Height = (int)lineHeight;
        }

        public static void Configure(this FrameworkElement view, FigmaVectorEntity child)
        {
            Configure(view, (FigmaNode)child);

            if (view is Panel canvas)
            {
                if (child.HasFills && child.fills[0].color != null)
                {
                    canvas.Background = child.fills[0].color.ToColor();
                }
            }
        }

        public static void Configure(this FrameworkElement view, FigmaRectangleVector child)
        {
            Configure(view, (FigmaVectorEntity)child);

            //view.Layer.CornerRadius = child.cornerRadius;
        }

        public static void Configure(this Label label, FigmaText text)
        {
            Configure(label, (FigmaNode)text);

            label.HorizontalAlignment = text.style.textAlignHorizontal == "CENTER" ? HorizontalAlignment.Center : text.style.textAlignHorizontal == "LEFT" ? HorizontalAlignment.Left : HorizontalAlignment.Right;
            label.Opacity = text.opacity;

            label.ConfigureStyle(text.style);

            var fills = text.fills.FirstOrDefault();
            if (fills != null)
            {
                label.Foreground = fills.color.ToColor();
            }

            //if (text.characterStyleOverrides != null && text.characterStyleOverrides.Length > 0)
            //{
            //    var attributedText = new NSMutableAttributedString(label.AttributedStringValue);
            //    for (int i = 0; i < text.characterStyleOverrides.Length; i++)
            //    {
            //        var key = text.characterStyleOverrides[i].ToString();
            //        if (!text.styleOverrideTable.ContainsKey(key))
            //        {
            //            continue;
            //        }
            //        var element = text.styleOverrideTable[key];
            //        if (element.fontFamily == null)
            //        {
            //            continue;
            //        }
            //        var localFont = FigmaExtensions.ToNSFont(element);
            //        var range = new NSRange(i, 1);
            //        attributedText.AddAttribute(NSStringAttributeKey.Font, localFont, range);
            //        attributedText.AddAttribute(NSStringAttributeKey.ForegroundColor, label.TextColor, range);
            //    }

            //    label.AttributedStringValue = attributedText;
            //}
        }
    }
}
