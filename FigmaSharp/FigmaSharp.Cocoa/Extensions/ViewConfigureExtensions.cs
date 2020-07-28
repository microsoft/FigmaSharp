﻿// Authors:
//   Jose Medrano <josmed@microsoft.com>
//
// Copyright (C) 2018 Microsoft, Corp
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Linq;

using AppKit;
using Foundation;

using FigmaSharp.Models;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Cocoa
{
	public static class ViewConfigureExtensions
    {
        public static void Configure(this NSView view, FigmaFrame child)
        {
            Configure(view, (FigmaNode)child);
		
			view.AlphaValue = child.opacity;
			view.Layer.BackgroundColor = child.backgroundColor.MixOpacity(child.opacity).ToCGColor();
		}

        public static void Configure(this NSView view, FigmaNode child)
        {
            view.Hidden = !child.visible;
            view.WantsLayer = true;
        }

        public static void Configure(this NSView figmaLineView, FigmaLine figmaLine)
        {
            //we draw a figma line from images or svg
            Configure(figmaLineView, (FigmaNode)figmaLine);

            var absolute = figmaLine.absoluteBoundingBox;
            var lineWidth = absolute.Width == 0 ? figmaLine.strokeWeight : absolute.Width;

            var constraintWidth = figmaLineView.WidthAnchor.ConstraintEqualToConstant(lineWidth);
            constraintWidth.Priority = (uint)NSLayoutPriority.DefaultLow;
            constraintWidth.Active = true;

            var lineHeight = absolute.Height == 0 ? figmaLine.strokeWeight : absolute.Height;

            var constraintHeight = figmaLineView.HeightAnchor.ConstraintEqualToConstant(lineHeight);
            constraintHeight.Priority = (uint)NSLayoutPriority.DefaultLow;
            constraintHeight.Active = true;

            figmaLineView.AlphaValue = figmaLine.opacity;
        }
		
        public static void Configure(this NSView view, FigmaVector child)
        {
            Configure(view, (FigmaNode)child);
            view.AlphaValue = child.opacity;
        }

        public static Color MixOpacity (this Color color, float opacity)
        {
            return new Color { A = color.A, R = color.R, G = color.G, B = color.B };
        }

        public static void Configure(this NSView view, RectangleVector child)
        {
            Configure(view, (FigmaVector)child);

            //var shapeLayer = new CAShapeLayer
            //{
            //    Path = NSBezierPath.FromRect(view.Bounds).ToCGPath(),
            //    Frame = view.Layer.Frame
            //};
            //view.Layer = shapeLayer;

            //var fill = child.fills?.FirstOrDefault();
            //if (fill != null && fill.visible && fill.color != null)
            //{
            //    shapeLayer.FillColor = fill.color.MixOpacity (fill.opacity).ToNSColor().CGColor;
            //} else
            //{
            //    shapeLayer.FillColor = NSColor.Clear.CGColor;
            //}
            
            //if (child.strokeDashes != null)
            //{
            //    var number = new NSNumber[child.strokeDashes.Length];
            //    for (int i = 0; i < child.strokeDashes.Length; i++)
            //    {
            //        number[i] = child.strokeDashes[i];
            //    }
            //    shapeLayer.LineDashPattern = number;
            //}

            ////shapeLayer.BackgroundColor = child.col
            //shapeLayer.LineWidth = child.strokeWeight * 2;

            //var strokes = child.strokes.FirstOrDefault();
            //if (strokes != null && strokes.visible)
            //{
            //    if (strokes.color != null)
            //    {
            //        shapeLayer.StrokeColor = strokes.color.MixOpacity (strokes.opacity).ToNSColor().CGColor;
            //        if (shapeLayer.LineWidth == 0)
            //        {
            //            shapeLayer.LineWidth = 1f;
            //        }
            //    }
            //}
            
            //shapeLayer.CornerRadius = child.cornerRadius;
            //view.AlphaValue = child.opacity;
        }

        public static void Configure(this NSTextField label, FigmaText text, bool configureColor = true)
        {
            label.Alignment = text.style.textAlignHorizontal == "CENTER" ? NSTextAlignment.Center : text.style.textAlignHorizontal == "LEFT" ? NSTextAlignment.Left : NSTextAlignment.Right;
            label.AlphaValue = text.opacity;
            //label.LineBreakMode = NSLineBreakMode.ByWordWrapping;
            //label.SetContentCompressionResistancePriority(250, NSLayoutConstraintOrientation.Horizontal);
            if (label.Cell is VerticalAlignmentTextCell cell)
            {
                cell.VerticalAligment = text.style.textAlignVertical == "CENTER" ? VerticalTextAlignment.Center : text.style.textAlignVertical == "TOP" ? VerticalTextAlignment.Top : VerticalTextAlignment.Bottom;
            }

            if (!configureColor) {
                return;
			}

            var fills = text.fills.FirstOrDefault();
            if (fills != null && fills.visible) {
                label.TextColor = fills.color.ToNSColor();
            }

            if (text.characterStyleOverrides != null && text.characterStyleOverrides.Length > 0)
            {
                var attributedText = new NSMutableAttributedString(label.AttributedStringValue);
                for (int i = 0; i < text.characterStyleOverrides.Length; i++)
                {
                    var range = new NSRange(i, 1);

                    var key = text.characterStyleOverrides[i].ToString();
                    if (!text.styleOverrideTable.ContainsKey(key) )
                    {
                        //we want the default values
                        attributedText.AddAttribute(NSStringAttributeKey.ForegroundColor, label.TextColor, range);
                        attributedText.AddAttribute(NSStringAttributeKey.Font, label.Font, range);
                        continue;
                    }

                    //if there is a style to override
                    var styleOverrided = text.styleOverrideTable[key];

                    //set the color
                    NSColor fontColorOverrided = label.TextColor;
                    var fillOverrided = styleOverrided.fills?.FirstOrDefault();
                    if (fillOverrided != null && fillOverrided.visible)
                        fontColorOverrided = fillOverrided.color.ToNSColor();

                    attributedText.AddAttribute(NSStringAttributeKey.ForegroundColor, fontColorOverrided, range);

                    //TODO: we can improve this
                    //set the font for this character
                    NSFont fontOverrided = label.Font;
                    if (styleOverrided.fontFamily != null)
                    {
                        fontOverrided = FigmaExtensions.ToNSFont(styleOverrided);
                    }
                    attributedText.AddAttribute(NSStringAttributeKey.Font, fontOverrided, range);
                }

                label.AttributedStringValue = attributedText;
            }
        }
    }
}
