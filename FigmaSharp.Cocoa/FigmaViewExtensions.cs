/* 
 * FigmaViewExtensions.cs - Extension methods for NSViews
 * 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *
 * Copyright (C) 2018 Microsoft, Corp
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System.Collections.Generic;
using AppKit;
using System.Linq;
using CoreGraphics;
using System;
using System.Net;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Foundation;
using CoreAnimation;
using FigmaSharp.Converters;

namespace FigmaSharp
{
    public static partial class FigmaViewExtensions
    {
        public static void LoadFigmaFromFilePath(this NSWindow window, string filePath, out List<IImageViewWrapper> figmaImageViews, string viewName = null, string nodeName = null)
        {
            figmaImageViews = new List<IImageViewWrapper>();
            var figmaDialog = FigmaApiHelper.GetFigmaDialogFromFilePath(filePath, viewName, nodeName);
            var boundingBox = figmaDialog.absoluteBoundingBox;
            if (boundingBox != null) {
                window.SetFrame(new CGRect(window.Frame.X, window.Frame.Y, boundingBox.width, boundingBox.height), true);
            }
            LoadFigma(window.ContentView, new FigmaFrameEntityResponse(filePath, figmaDialog), figmaImageViews);
        }

        public static void LoadFigmaFromUrlFile(this NSWindow window, string urlFile, out List<IImageViewWrapper> figmaImageViews, string viewName = null, string nodeName = null)
        {
            figmaImageViews = new List<IImageViewWrapper>();
            var figmaDialog = FigmaApiHelper.GetFigmaDialogFromUrlFile(urlFile, viewName, nodeName);
            var boundingBox = figmaDialog.absoluteBoundingBox;
            window.SetFrame(new CGRect(window.Frame.X, window.Frame.Y, boundingBox.width, boundingBox.height), true);
            LoadFigma(window.ContentView, new FigmaFrameEntityResponse(urlFile, figmaDialog), figmaImageViews);
        }

        public static void LoadFigmaFromResource(this NSView contentView, string resource, out List<IImageViewWrapper> figmaImageViews, Assembly assembly = null, string viewName = null, string nodeName = null)
        {
            figmaImageViews = new List<IImageViewWrapper>();
            var template = FigmaApiHelper.GetManifestResource(assembly, resource);
            var figmaDialog = FigmaApiHelper.GetFigmaDialogFromContent(template, viewName, nodeName);
            LoadFigmaFromFrameEntity(contentView, figmaDialog, figmaImageViews, viewName, nodeName);
        }

        public static void LoadFigmaFromFilePath(this NSView contentView, string filePath, out List<IImageViewWrapper> figmaImageViews, string viewName = null, string nodeName = null)
        {
            figmaImageViews = new List<IImageViewWrapper>();
            var figmaDialog = FigmaApiHelper.GetFigmaDialogFromFilePath(filePath, viewName, nodeName);
            LoadFigmaFromFrameEntity(contentView, figmaDialog, figmaImageViews, viewName, nodeName);
        }

        public static void LoadFigmaFromContent(this NSView contentView, string figmaContent, out List<IImageViewWrapper> figmaImageViews, string viewName = null, string nodeName = null)
        {
            figmaImageViews = new List<IImageViewWrapper>();
            var figmaDialog = FigmaApiHelper.GetFigmaDialogFromContent(figmaContent, viewName, nodeName);
            LoadFigmaFromFrameEntity(contentView, figmaDialog, figmaImageViews, viewName, nodeName);
        }

        public static void LoadFigmaFromUrlFile(this NSView contentView, string urlFile, out List<IImageViewWrapper> figmaImageViews, string viewName = null, string nodeName = null)
        {
            figmaImageViews = new List<IImageViewWrapper>();
            var figmaDialog = FigmaApiHelper.GetFigmaDialogFromUrlFile(urlFile, viewName, nodeName);
            LoadFigmaFromFrameEntity(contentView, figmaDialog, figmaImageViews, viewName, nodeName);
        }

        public static void LoadFigmaFromFrameEntity(this NSView view, IFigmaDocumentContainer figmaView, List<IImageViewWrapper> figmaImageViews, string figmaFileName, string viewName = null, string nodeName = null)
        {
            if (figmaView != null)
            {
                LoadFigma(view, new FigmaFrameEntityResponse(figmaFileName, figmaView), figmaImageViews);
            }
            else
            {
                var alert = new NSAlert();
                alert.MessageText = string.Format("You figma file does not have a view name:'{0}'", viewName);
                if (nodeName != null)
                {
                    alert.MessageText += string.Format(" or node name: '{0}'", nodeName);
                }
                alert.AddButton("Close");
                alert.RunModal();
            }
        }

        public static void LoadFromLocalImageResources(this List<IImageViewWrapper> figmaImageViews, Assembly assembly = null)
        {
            for (int i = 0; i < figmaImageViews.Count; i++)
            {
                try
                {
                    var image = AppContext.Current.GetImageFromManifest (assembly, figmaImageViews[i].Data.imageRef);
                    figmaImageViews[i].SetImage (image);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public static void LoadFromResourceImageDirectory(this List<IImageViewWrapper> figmaImageViews, string resourcesDirectory, string format = ".png")
        {
            for (int i = 0; i < figmaImageViews.Count; i++)
            {
                try
                {
                    string filePath = Path.Combine(resourcesDirectory, string.Concat(figmaImageViews[i].Data.imageRef, format));
                    if (!File.Exists(filePath))
                    {
                        throw new FileNotFoundException(filePath);
                    }
                    figmaImageViews[i].SetImage(AppContext.Current.GetImageFromFilePath(filePath));
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine("[FIGMA.RENDERER] Resource '{0}' not found.", ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public static void Load(this IEnumerable<IImageViewWrapper> figmaImageViews, string fileId)
        {
            var ids = figmaImageViews.Select(s => s.Data.ID).ToArray();
            var images = FigmaApiHelper.GetFigmaImages(fileId, ids);

            if (images != null) {

                List<Task> downloadImageTaks = new List<Task>();
                foreach (var imageView in figmaImageViews) {

                    Task.Run(() => {
                        var url = images.images[imageView.Data.ID];
                        Console.WriteLine($"Processing image - ID:[{imageView.Data.ID}] ImageRef:[{imageView.Data.imageRef}] Url:[{url}]");
                        try {
                            var image = AppContext.Current.GetImage(url);
                            NSApplication.SharedApplication.InvokeOnMainThread(() => {
                                imageView.SetImage (image);
                            });
                            Console.WriteLine($"[SUCCESS] Processing image - ID:[{imageView.Data.ID}] ImageRef:[{imageView.Data.imageRef}] Url:[{url}]");
                        } catch (Exception ex) {
                            Console.WriteLine($"[ERROR] Processing image - ID:[{imageView.Data.ID}] ImageRef:[{imageView.Data.imageRef}] Url:[{url}]");
                            Console.WriteLine(ex);
                        }

                    });
                }
            }
        }

        public static NSColor ToNSColor(this FigmaColor color)
        {
            return NSColor.FromRgba(color.r, color.g, color.b, color.a);
        }

        public static void LoadFigma(this NSView contentView, FigmaFrameEntityResponse frameEntityResponse, List<IImageViewWrapper> figmaImageViews = null)
        {
            //clean views from current container
            var views = contentView.Subviews;
            foreach (var item in views) {
                item.RemoveFromSuperview();
            }
            contentView.RemoveConstraints(contentView.Constraints);

            //Figma doesn't calculate the bounds of our first level
            frameEntityResponse.FigmaMainNode.CalculateBounds();

            contentView.WantsLayer = true;
            var backgroundColor = frameEntityResponse.FigmaMainNode.backgroundColor.ToNSColor();
            contentView.Layer.BackgroundColor = backgroundColor.CGColor;

            var figmaView = frameEntityResponse.FigmaMainNode as FigmaNode;
            //var mainView = figmaView.ToViewWrapper(new ViewWrapper (contentView), figmaView);
            //if (mainView != null) {
            //    contentView.AddSubview(mainView.NativeObject as NSView);
            //}
        }

        public static CGRect ToCGRect(this FigmaRectangle rectangle)
        {
            return new CGRect(0, 0, rectangle.width, rectangle.height);
        }

        public static NSFont ToNSFont(this FigmaTypeStyle style)
        {
            string family = style.fontFamily;
            if (family == "SF UI Text")
            {
                family = ".SF NS Text";
            }
            else if (family == "SF Mono")
            {
                family = ".SF NS Display";
            }
            else
            {
                Console.WriteLine("FONT: {0} - {1}", family, style.fontPostScriptName);
            }

            var font = NSFont.FromFontName(family, style.fontSize);
            var w = ToAppKitFontWeight(style.fontWeight);
            NSFontTraitMask traits = default(NSFontTraitMask);
            if (style.fontPostScriptName != null && style.fontPostScriptName.EndsWith("-Bold"))
            {
                traits = NSFontTraitMask.Bold;
            }
            else
            {

            }
            //if (font != null)
            //{
            //    var w = NSFontManager.SharedFontManager.WeightOfFont(font);
            //    var traits = NSFontManager.SharedFontManager.TraitsOfFont(font);

            //}

            font = NSFontManager.SharedFontManager.FontWithFamily(family, traits, w, style.fontSize);
            //var font = NSFont.FromFontName(".SF NS Text", 12);

            if (font == null)
            {
                Console.WriteLine($"[ERROR] Font not found :{family}");
                font = NSFont.LabelFontOfSize(style.fontSize);
            }
            return font;
        }

        public static CGPoint GetRelativePosition(this IAbsoluteBoundingBox parent, IAbsoluteBoundingBox node)
        {
            return new CGPoint(
                node.absoluteBoundingBox.x - parent.absoluteBoundingBox.x,
                node.absoluteBoundingBox.y - parent.absoluteBoundingBox.y
            );
        }

        public static void CreateConstraints(this NSView view, NSView parent, FigmaLayoutConstraint constraints, FigmaRectangle absoluteBoundingBox, FigmaRectangle absoluteBoundBoxParent)
        {
            System.Console.WriteLine("Create constraint  horizontal:{0} vertical:{1}", constraints.horizontal, constraints.vertical);

            if (constraints.horizontal.Contains("RIGHT"))
            {
                var endPosition1 = absoluteBoundingBox.x + absoluteBoundingBox.width;
                var endPosition2 = absoluteBoundBoxParent.x + absoluteBoundBoxParent.width;
                var value = Math.Max(endPosition1, endPosition2) - Math.Min(endPosition1, endPosition2);
                view.RightAnchor.ConstraintEqualToAnchor(parent.RightAnchor, -value).Active = true;

                var value2 = absoluteBoundingBox.x - absoluteBoundBoxParent.x;
                view.LeftAnchor.ConstraintEqualToAnchor(parent.LeftAnchor, value2).Active = true;
            }

            if (constraints.horizontal != "RIGHT")
            {
                var value2 = absoluteBoundingBox.x - absoluteBoundBoxParent.x;
                view.LeftAnchor.ConstraintEqualToAnchor(parent.LeftAnchor, value2).Active = true;
            }

            if (constraints.horizontal.Contains("BOTTOM"))
            {
                var value = absoluteBoundingBox.y - absoluteBoundBoxParent.y;
                view.TopAnchor.ConstraintEqualToAnchor(parent.TopAnchor, value).Active = true;

                var endPosition1 = absoluteBoundingBox.y + absoluteBoundingBox.height;
                var endPosition2 = absoluteBoundBoxParent.y + absoluteBoundBoxParent.height;
                var value2 = Math.Max(endPosition1, endPosition2) - Math.Min(endPosition1, endPosition2);

                view.BottomAnchor.ConstraintEqualToAnchor(parent.BottomAnchor, -value2).Active = true;
            }

            if (constraints.horizontal != "BOTTOM")
            {
                var value = absoluteBoundingBox.y - absoluteBoundBoxParent.y;
                view.TopAnchor.ConstraintEqualToAnchor(parent.TopAnchor, value).Active = true;
            }
        }

        static int[] app_kit_font_weights = {
            2,   // FontWeight100
      3,   // FontWeight200
      4,   // FontWeight300
      5,   // FontWeight400
      6,   // FontWeight500
      8,   // FontWeight600
      9,   // FontWeight700
      10,  // FontWeight800
      12,  // FontWeight900
            };

        public static int ToAppKitFontWeight(float font_weight)
        {
            float weight = font_weight;
            if (weight <= 50 || weight >= 950)
                return 5;

            var select_weight = (int) Math.Round(weight / 100) - 1;
            return app_kit_font_weights[select_weight];
        }

        static CGPath ToGCPath (this NSBezierPath bezierPath)
        {
            var path = new CGPath();
            CGPoint[] points;
            for (int i = 0; i < bezierPath.ElementCount; i++)
            {
                var type = bezierPath.ElementAt(i,out points);
                switch (type)
                {
                    case NSBezierPathElement.MoveTo:
                        path.MoveToPoint(points[0]);
                        break;
                    case NSBezierPathElement.LineTo:
                        path.AddLineToPoint(points[0]);
                        break;
                    case NSBezierPathElement.CurveTo:
                        path.AddCurveToPoint(points[2], points[1], points[0]);
                        break;
                    case NSBezierPathElement.ClosePath:
                        path.CloseSubpath();
                        break;
                }
            }
            return path;
        }

        public static void Configure(this NSView view, FigmaFrameEntity child)
        {
            Configure (view, (FigmaNode)child);

            view.AlphaValue = child.opacity;
            view.Layer.BackgroundColor = ToNSColor(child.backgroundColor).CGColor;
        }

        public static void Configure(this NSView view, FigmaNode child)
        {
            view.Hidden = !child.visible;
            view.WantsLayer = true;

            if (child is IFigmaDocumentContainer container)
            {
                view.SetFrameSize(new CGSize(container.absoluteBoundingBox.width, container.absoluteBoundingBox.height));
            }
        }

        public static void Configure(this NSView view, FigmaElipse elipse)
        {
            Configure(view, (FigmaVectorEntity)elipse);

            var circleLayer = new CAShapeLayer();
            var bezierPath = NSBezierPath.FromOvalInRect(new CGRect(0, 0, elipse.absoluteBoundingBox.width, elipse.absoluteBoundingBox.height));
            circleLayer.Path = bezierPath.ToGCPath();

            view.Layer.AddSublayer(circleLayer);

            var fills = elipse.fills.OfType<FigmaPaint>().FirstOrDefault();
            if (fills != null)
            {
                circleLayer.FillColor = fills.color.ToNSColor().CGColor;
            }

            var strokes = elipse.strokes.FirstOrDefault();
            if (strokes != null)
            {
                if (strokes.color != null)
                {
                    circleLayer.BorderColor = strokes.color.ToNSColor().CGColor;
                }
            }
        }

        public static void Configure(this NSView figmaLineView, FigmaLine figmaLine)
        {
            Configure(figmaLineView, (FigmaVectorEntity)figmaLine);

            var fills = figmaLine.fills.OfType<FigmaPaint>().FirstOrDefault();
            if (fills != null) {
                figmaLineView.Layer.BackgroundColor = fills.color.ToNSColor().CGColor;
            }

            var absolute = figmaLine.absoluteBoundingBox;
            var lineWidth = absolute.width == 0 ? figmaLine.strokeWeight : absolute.width;

            var constraintWidth = figmaLineView.WidthAnchor.ConstraintEqualToConstant(lineWidth);
            constraintWidth.Priority = (uint)NSLayoutPriority.DefaultLow;
            constraintWidth.Active = true;

            var lineHeight = absolute.height == 0 ? figmaLine.strokeWeight : absolute.height;

            var constraintHeight = figmaLineView.HeightAnchor.ConstraintEqualToConstant(lineHeight);
            constraintHeight.Priority = (uint)NSLayoutPriority.DefaultLow;
            constraintHeight.Active = true;
        }

        public static void Configure (this NSView view, FigmaVectorEntity child)
        {
            Configure(view, (FigmaNode)child);

            if (child.HasFills && child.fills[0].color != null)
            {
                view.Layer.BackgroundColor = child.fills[0].color.ToNSColor().CGColor;
            }

            //var currengroupView = new NSView() { TranslatesAutoresizingMaskIntoConstraints = false };
            //currengroupView.Configure(rectangleVector);

            var strokes = child.strokes.FirstOrDefault();
            if (strokes != null)
            {
                if (strokes.color != null)
                {
                    view.Layer.BorderColor = strokes.color.ToNSColor().CGColor;
                }
                view.Layer.BorderWidth = child.strokeWeight;
            }
        }

        public static void Configure(this NSView view, FigmaRectangleVector child)
        {
            Configure(view, (FigmaVectorEntity)child);

            view.Layer.CornerRadius = child.cornerRadius;
        }

        public static void Configure(this NSTextField label, FigmaText text)
        {
            Configure(label, (FigmaNode)text);

            label.Alignment = text.style.textAlignHorizontal == "CENTER" ? NSTextAlignment.Center : text.style.textAlignHorizontal == "LEFT" ? NSTextAlignment.Left : NSTextAlignment.Right;
            label.AlphaValue = text.opacity;
            label.LineBreakMode = NSLineBreakMode.ByWordWrapping;
            label.SetContentCompressionResistancePriority(250, NSLayoutConstraintOrientation.Horizontal);

            var fills = text.fills.FirstOrDefault();
            if (fills != null)
            {
                label.TextColor = ToNSColor(fills.color);
            }

            if (text.characterStyleOverrides != null && text.characterStyleOverrides.Length > 0)
            {
                var attributedText = new NSMutableAttributedString(label.AttributedStringValue);
                for (int i = 0; i < text.characterStyleOverrides.Length; i++)
                {
                    var key = text.characterStyleOverrides[i].ToString();
                    if (!text.styleOverrideTable.ContainsKey(key))
                    {
                        continue;
                    }
                    var element = text.styleOverrideTable[key];
                    if (element.fontFamily == null)
                    {
                        continue;
                    }
                    var localFont = ToNSFont(element);
                    var range = new NSRange(i, 1);
                    attributedText.AddAttribute(NSStringAttributeKey.Font, localFont, range);
                    attributedText.AddAttribute(NSStringAttributeKey.ForegroundColor, label.TextColor, range);
                }

                label.AttributedStringValue = attributedText;
            }
        }

    }
}
