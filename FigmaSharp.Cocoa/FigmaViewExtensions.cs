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

namespace FigmaSharp
{
    public static class FigmaViewExtensions
    {
        public static void LoadFigmaFromFilePath(this NSWindow window, string filePath, out List<FigmaImageView> figmaImageViews, string viewName = null, string nodeName = null)
        {
            figmaImageViews = new List<FigmaImageView>();
            var figmaDialog = FigmaHelper.GetFigmaDialogFromFilePath(filePath, viewName, nodeName);
            var boundingBox = figmaDialog.absoluteBoundingBox;
            if (boundingBox != null) {
                window.SetFrame(new CGRect(window.Frame.X, window.Frame.Y, boundingBox.width, boundingBox.height), true);
            }
            LoadFigma(window.ContentView, new FigmaFrameEntityResponse(filePath, figmaDialog), figmaImageViews);
        }

        public static void LoadFigmaFromUrlFile(this NSWindow window, string urlFile, out List<FigmaImageView> figmaImageViews, string viewName = null, string nodeName = null)
        {
            figmaImageViews = new List<FigmaImageView>();
            var figmaDialog = FigmaHelper.GetFigmaDialogFromUrlFile(urlFile, viewName, nodeName);
            var boundingBox = figmaDialog.absoluteBoundingBox;
            window.SetFrame(new CGRect(window.Frame.X, window.Frame.Y, boundingBox.width, boundingBox.height), true);
            LoadFigma(window.ContentView, new FigmaFrameEntityResponse(urlFile, figmaDialog), figmaImageViews);
        }


        public static void LoadFigmaFromResource(this NSView contentView, string resource, out List<FigmaImageView> figmaImageViews, Assembly assembly = null, string viewName = null, string nodeName = null)
        {
            figmaImageViews = new List<FigmaImageView>();
            var template = FigmaHelper.GetManifestResource(assembly, resource);
            var figmaDialog = FigmaHelper.GetFigmaDialogFromContent(template, viewName, nodeName);
            LoadFigmaFromFrameEntity(contentView, figmaDialog, figmaImageViews, viewName, nodeName);
        }

        public static void LoadFigmaFromFilePath(this NSView contentView, string filePath, out List<FigmaImageView> figmaImageViews, string viewName = null, string nodeName = null)
        {
            figmaImageViews = new List<FigmaImageView>();
            var figmaDialog = FigmaHelper.GetFigmaDialogFromFilePath(filePath, viewName, nodeName);
            LoadFigmaFromFrameEntity(contentView, figmaDialog, figmaImageViews, viewName, nodeName);
        }

        public static void LoadFigmaFromContent(this NSView contentView, string figmaContent, out List<FigmaImageView> figmaImageViews, string viewName = null, string nodeName = null)
        {
            figmaImageViews = new List<FigmaImageView>();
            var figmaDialog = FigmaHelper.GetFigmaDialogFromContent(figmaContent, viewName, nodeName);
            LoadFigmaFromFrameEntity(contentView, figmaDialog, figmaImageViews, viewName, nodeName);
        }

        public static void LoadFigmaFromUrlFile(this NSView contentView, string urlFile, out List<FigmaImageView> figmaImageViews, string viewName = null, string nodeName = null)
        {
            figmaImageViews = new List<FigmaImageView>();
            var figmaDialog = FigmaHelper.GetFigmaDialogFromUrlFile(urlFile, viewName, nodeName);
            LoadFigmaFromFrameEntity(contentView, figmaDialog, figmaImageViews, viewName, nodeName);
        }

        public static void LoadFigmaFromFrameEntity(this NSView view, IFigmaDocumentContainer figmaView, List<FigmaImageView> figmaImageViews, string figmaFileName, string viewName = null, string nodeName = null)
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

        public static void LoadFromLocalImageResources(this List<FigmaImageView> figmaImageViews, Assembly assembly = null)
        {

            for (int i = 0; i < figmaImageViews.Count; i++)
            {
                try
                {
                    var image = FigmaViewsHelper.GetManifestImageResource(assembly, string.Format("{0}.png", figmaImageViews[i].Data.imageRef));
                    figmaImageViews[i].Image = image;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public static void LoadFromResourceImageDirectory(this List<FigmaImageView> figmaImageViews, string resourcesDirectory, string format = ".png")
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
                    figmaImageViews[i].Image = new NSImage(filePath);
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

        public static void Load(this IEnumerable<FigmaImageView> figmaImageViews, string fileId)
        {
            var ids = figmaImageViews.Select(s => s.Data.ID).ToArray();
            var images = FigmaHelper.GetFigmaImages(fileId, ids);

            if (images != null) {

                List<Task> downloadImageTaks = new List<Task>();
                foreach (var imageView in figmaImageViews) {

                    Task.Run(() => {
                        var url = images.images[imageView.Data.ID];
                        Console.WriteLine($"Processing image - ID:[{imageView.Data.ID}] ImageRef:[{imageView.Data.imageRef}] Url:[{url}]");
                        try {

                            var image = new NSImage(new Foundation.NSUrl(url));

                            NSApplication.SharedApplication.InvokeOnMainThread(() => {
                                imageView.Image = image;
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

        //TODO: Change to async multithread
        public static async Task SaveFigmaImageFiles(this FigmaPaint[] paints, string fileId, string directoryPath, string format = ".png")
        {
            var ids = paints.Select(s => s.ID).ToArray();
            var query = new FigmaImageQuery(FigmaEnvirontment.Token, fileId, ids);
            var images = FigmaHelper.GetFigmaImage(query);
            if (images != null)
            {
                var urls = paints.Select(s => images.images[s.ID]).ToArray();
                await FigmaHelper.SaveFilesAsync(directoryPath, format, urls);
            }
        }

        public static void LoadFigma(this NSView contentView, FigmaFrameEntityResponse frameEntityResponse, List<FigmaImageView> figmaImageViews = null)
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
            var mainView = figmaView.ToNSView(contentView, figmaView, figmaImageViews);
            if (mainView != null) {
                contentView.AddSubview(mainView);
            }
        }

        public static CGRect ToCGRect(this FigmaRectangle rectangle)
        {
            return new CGRect(0, 0, rectangle.width, rectangle.height);
        }

        public static NSColor ToNSColor(this FigmaColor color)
        {
            return NSColor.FromRgba(color.r, color.g, color.b, color.a);
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
            if (style.fontPostScriptName != null && style.fontPostScriptName.EndsWith ("-Bold"))
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

        public static void CalculateBounds (this IFigmaNodeContainer figmaNodeContainer)
        {
            if (figmaNodeContainer is IAbsoluteBoundingBox calculatedBounds)
            {
                calculatedBounds.absoluteBoundingBox = FigmaRectangle.Zero;
                foreach (var item in figmaNodeContainer.children)
                {
                    if (item is IAbsoluteBoundingBox itmBoundingBox)
                    {
                        calculatedBounds.absoluteBoundingBox.x = Math.Min(calculatedBounds.absoluteBoundingBox.x, itmBoundingBox.absoluteBoundingBox.x);
                        calculatedBounds.absoluteBoundingBox.y = Math.Min(calculatedBounds.absoluteBoundingBox.y, itmBoundingBox.absoluteBoundingBox.y);

                        if (itmBoundingBox.absoluteBoundingBox.x + itmBoundingBox.absoluteBoundingBox.width > calculatedBounds.absoluteBoundingBox.x + calculatedBounds.absoluteBoundingBox.width)
                        {
                            calculatedBounds.absoluteBoundingBox.width += (itmBoundingBox.absoluteBoundingBox.x + itmBoundingBox.absoluteBoundingBox.width) - (calculatedBounds.absoluteBoundingBox.x + calculatedBounds.absoluteBoundingBox.width);
                        }

                        if (itmBoundingBox.absoluteBoundingBox.y + itmBoundingBox.absoluteBoundingBox.height > calculatedBounds.absoluteBoundingBox.y + calculatedBounds.absoluteBoundingBox.height)
                        {
                            calculatedBounds.absoluteBoundingBox.height += (itmBoundingBox.absoluteBoundingBox.y + itmBoundingBox.absoluteBoundingBox.height) - (calculatedBounds.absoluteBoundingBox.y + calculatedBounds.absoluteBoundingBox.height);
                        }
                    }
                }
            }
        }

        public static IEnumerable<FigmaPaint> OfTypeImage (this FigmaNode child)
        {
            if (child.GetType() == typeof(FigmaRectangleVector))
            {
                var rectangleVector = ((FigmaVectorEntity)child);

                var fills = rectangleVector.fills.FirstOrDefault();
                if (fills?.type == "IMAGE" && fills is FigmaPaint figmaPaint)
                {
                    figmaPaint.ID = child.id;
                    yield return figmaPaint;
                }
            }

            if (child is IFigmaNodeContainer nodeContainer)
            {
                foreach (var item in nodeContainer.children)
                {
                    foreach (var resultItems in OfTypeImage(item))
                    {
                        yield return resultItems;
                    }
                }
            }
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

        //TODO: This 
        public static NSView ToNSView(this FigmaNode parent, NSView parentView, FigmaNode child, List<FigmaImageView> figmaImageViews = null)
        {
            Console.WriteLine("[{0}({1})] Processing {2}..", child.id, child.name, child.GetType());
            if (child is IFigmaDocumentContainer instance && child is IConstraints instanceConstrains)
            {
                var absolute = instance.absoluteBoundingBox;
                var parentFrame = (IAbsoluteBoundingBox)parent;

                if (child.name == "button" || child.name == "button default")
                {
                    var button = new NSButton() { TranslatesAutoresizingMaskIntoConstraints = false };
                    parentView.AddSubview(button);
                    button.Hidden = !child.visible;

                    button.WantsLayer = true;

                    var figmaText = instance.children.OfType<FigmaText>().FirstOrDefault();
                    if (figmaText != null)
                    {
                        button.Font = ToNSFont(figmaText.style);
                    }

                    button.WidthAnchor.ConstraintEqualToConstant(absolute.width).Active = true;
                    button.HeightAnchor.ConstraintEqualToConstant(absolute.height).Active = true;
                    CreateConstraints(button, parentView, instanceConstrains.constraints, absolute, parentFrame.absoluteBoundingBox);

                    if (instance.children.OfType<FigmaGroup>().Any())
                    {
                        //button.Bordered  false;
                        //button.SetButtonType(NSButtonType.MomentaryPushIn);
                        button.Title = "";
                        //button.Transparent = true;
                        button.AlphaValue = 0.15f;
                        button.BezelStyle = NSBezelStyle.TexturedSquare;
                    }
                    else
                    {
                        if (figmaText != null)
                        {
                            button.AlphaValue = figmaText.opacity;
                            button.Title = figmaText.characters;
                        }
                    
                        button.BezelStyle = NSBezelStyle.Rounded;
                        button.Layer.BackgroundColor = ToNSColor(instance.backgroundColor).CGColor;
                        return null;
                    }

                }

                if (child.name == "text field" || child.name == "Field")
                {
                    var textField = new NSTextField() { TranslatesAutoresizingMaskIntoConstraints = false };
                    parentView.AddSubview(textField);
  
                    textField.Hidden = !child.visible;
                    var figmaText = instance.children.OfType<FigmaText>()
                        .FirstOrDefault();

                    textField.AlphaValue = figmaText.opacity;
                    textField.StringValue = figmaText.characters;
                    textField.Font = ToNSFont(figmaText.style);
                    textField.WidthAnchor.ConstraintEqualToConstant(absolute.width).Active = true;
                    textField.HeightAnchor.ConstraintEqualToConstant(absolute.height).Active = true;
                    CreateConstraints(textField, parentView, instanceConstrains.constraints, absolute, parentFrame.absoluteBoundingBox);
                    //return null;
                }

            }

            if (child.GetType() == typeof(FigmaVector))
            {
                var vector = ((FigmaVector)child);
                 Console.WriteLine(vector);
            }
            else if (child.GetType() == typeof(FigmaInstance))
            {
                Console.WriteLine("Not implemented {0}", child.name);
                if (child is IFigmaNodeContainer nodeContainer)
                {
                    foreach (var item in nodeContainer.children)
                    {
                        ToNSView(parent, parentView, item, figmaImageViews);
                    }
                }
            }
            else if (child is FigmaFrameEntity figmaFrameEntity)
            {
                var absolute = figmaFrameEntity.absoluteBoundingBox;
                var parentFrame = (IAbsoluteBoundingBox)parent;

                var currengroupView = new NSView() { TranslatesAutoresizingMaskIntoConstraints = false };
                currengroupView.WantsLayer = true;
                currengroupView.Hidden = !child.visible;
                currengroupView.AlphaValue = figmaFrameEntity.opacity;
                currengroupView.Layer.BackgroundColor = ToNSColor(figmaFrameEntity.backgroundColor).CGColor;

                parentView.AddSubview(currengroupView);

                var constraintWidth = currengroupView.WidthAnchor.ConstraintEqualToConstant(absolute.width);
                constraintWidth.Priority = (uint)NSLayoutPriority.DefaultLow;
                constraintWidth.Active = true;
                var constraintHeight = currengroupView.HeightAnchor.ConstraintEqualToConstant(absolute.height);
                constraintHeight.Priority = (uint)NSLayoutPriority.DefaultLow;
                constraintHeight.Active = true;

                if (parentView?.Superview is NSClipView)
                {
                    parentView.Frame = new CGRect(
                    parentFrame.absoluteBoundingBox.x,
                        parentFrame.absoluteBoundingBox.y,
                    parentFrame.absoluteBoundingBox.width,
                        parentFrame.absoluteBoundingBox.height);
                }

                var constraints = figmaFrameEntity.constraints;

                if (parent is FigmaCanvas canvas)
                {
                    CreateConstraints(currengroupView, parentView, constraints, absolute, canvas.absoluteBoundingBox);
                }
                else if (parent is FigmaFrameEntity parentFigmaFrameEntity)
                {
                    CreateConstraints(currengroupView, parentView, constraints, absolute, parentFigmaFrameEntity.absoluteBoundingBox ?? FigmaRectangle.Zero);
                }

                foreach (var item in figmaFrameEntity.children)
                {
                    ToNSView(figmaFrameEntity, currengroupView, item, figmaImageViews);
                }

                Console.WriteLine(figmaFrameEntity);
            }
            else if (child.GetType() == typeof(FigmaText))
            {
                var text = ((FigmaText)child);

                var absolute = text.absoluteBoundingBox;
                var parentFrame = (FigmaFrameEntity)parent;
                var position = GetRelativePosition(parentFrame, text);
                var constraints = text.constraints;

                var font = ToNSFont(text.style);
                var label = FigmaViewsHelper.CreateLabel(text.characters, font);
                label.Alignment = text.style.textAlignHorizontal == "CENTER" ? NSTextAlignment.Center : text.style.textAlignHorizontal == "LEFT" ? NSTextAlignment.Left : NSTextAlignment.Right;
                label.AlphaValue = text.opacity;
                label.Hidden = !child.visible;
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

                parentView.AddSubview(label);

                label.WidthAnchor.ConstraintGreaterThanOrEqualToConstant(absolute.width).Active = true;
                //label.HeightAnchor.ConstraintEqualToConstant (absolute.height).Active = true;
                CreateConstraints(label, parentView, constraints, absolute, parentFrame.absoluteBoundingBox);
                Console.WriteLine(text);
            }
            else if (child.GetType() == typeof(FigmaVectorEntity))
            {
                var vector = ((FigmaVectorEntity)child);
                var absolute = vector.absoluteBoundingBox;
                var parentEntityFrame = (IAbsoluteBoundingBox)parent;
                var constraints = vector.constraints;
               
                var currengroupView = new NSView() { TranslatesAutoresizingMaskIntoConstraints = false };
                currengroupView.WantsLayer = true;
                currengroupView.AlphaValue = vector.opacity;
                currengroupView.Hidden = !child.visible;
                var fills = vector.fills.FirstOrDefault();
                if (fills != null && fills.color != null)
                {
                    currengroupView.Layer.BackgroundColor = ToNSColor(fills.color).CGColor;
                }

                parentView.AddSubview(currengroupView);

                var constraintWidth = currengroupView.WidthAnchor.ConstraintEqualToConstant(absolute.width);
                constraintWidth.Priority = (uint)NSLayoutPriority.DefaultLow;
                constraintWidth.Active = true;
                var constraintHeight = currengroupView.HeightAnchor.ConstraintEqualToConstant(absolute.height);
                constraintHeight.Priority = (uint)NSLayoutPriority.DefaultLow;
                constraintHeight.Active = true;
                CreateConstraints(currengroupView, parentView, constraints, absolute, parentEntityFrame.absoluteBoundingBox);

            }
            else if (child.GetType() == typeof(FigmaRectangleVector))
            {
                var rectangleVector = ((FigmaVectorEntity)child);
                var absolute = rectangleVector.absoluteBoundingBox;
                var parentEntityFrame = (IAbsoluteBoundingBox)parent;
                var position = GetRelativePosition(parentEntityFrame, rectangleVector);
                var constraints = rectangleVector.constraints;

                NSView currengroupView = null; // = new NSView () { TranslatesAutoresizingMaskIntoConstraints = false };

                var fills = rectangleVector.fills.FirstOrDefault();
                if (fills?.type == "IMAGE" && fills is FigmaPaint figmaPaint)
                {
                    figmaPaint.ID = child.id;
                    FigmaImageView figmaImageView;
                    currengroupView = figmaImageView = new FigmaImageView() { Data = figmaPaint };
                    figmaImageViews?.Add(figmaImageView);
                }
                else
                {
                    currengroupView = new NSView() { TranslatesAutoresizingMaskIntoConstraints = false };
                }

                currengroupView.WantsLayer = true;
                currengroupView.Hidden = !child.visible;
                currengroupView.AlphaValue = rectangleVector.opacity;
                  
                if (child is FigmaRectangleVector vector)
                {
                    currengroupView.Layer.CornerRadius = vector.cornerRadius;
                }

                if (fills?.color != null)
                {
                    currengroupView.Layer.BackgroundColor = ToNSColor(fills.color).CGColor;
                }
                var strokes = rectangleVector.strokes.FirstOrDefault();
                if (strokes != null)
                {
                    if (strokes.color != null)
                    {
                        currengroupView.Layer.BorderColor = ToNSColor(strokes.color).CGColor;
                    }
                    currengroupView.Layer.BorderWidth = rectangleVector.strokeWeight;
                }

                parentView.AddSubview(currengroupView);

                var constraintWidth = currengroupView.WidthAnchor.ConstraintEqualToConstant(absolute.width);
                constraintWidth.Priority = (uint)NSLayoutPriority.DefaultLow;
                constraintWidth.Active = true;
                var constraintHeight = currengroupView.HeightAnchor.ConstraintEqualToConstant(absolute.height);
                constraintHeight.Priority = (uint)NSLayoutPriority.DefaultLow;
                constraintHeight.Active = true;
                CreateConstraints(currengroupView, parentView, constraints, absolute, parentEntityFrame.absoluteBoundingBox);

            }
            else if (child.GetType() == typeof(FigmaElipse))
            {
                var elipse = ((FigmaElipse)child);
                var absolute = elipse.absoluteBoundingBox;
                var parentFrame = (IAbsoluteBoundingBox)parent;
              
                var currentElipse = new NSView() { TranslatesAutoresizingMaskIntoConstraints = false };
                currentElipse.WantsLayer = true;
                currentElipse.AlphaValue = elipse.opacity;
                currentElipse.Hidden = !child.visible;
                parentView.AddSubview(currentElipse);

                currentElipse.WidthAnchor.ConstraintEqualToConstant(absolute.width).Active = true;
                currentElipse.HeightAnchor.ConstraintEqualToConstant(absolute.height).Active = true;

                var circleLayer = new CAShapeLayer();
                var bezierPath = NSBezierPath.FromOvalInRect(new CGRect(0, 0, absolute.width, absolute.height));
                circleLayer.Path = bezierPath.ToGCPath();

                currentElipse.Layer.AddSublayer(circleLayer);

                var fills = elipse.fills.OfType<FigmaPaint>().FirstOrDefault();
                if (fills != null)
                {
                    circleLayer.FillColor = ToNSColor(fills.color).CGColor;
                }

                var strokes = elipse.strokes.FirstOrDefault();
                if (strokes != null)
                {
                    if (strokes.color != null)
                    {
                        circleLayer.BorderColor = ToNSColor(strokes.color).CGColor;
                    }
                }

                CreateConstraints(currentElipse, parentView, elipse.constraints, absolute, parentFrame.absoluteBoundingBox);

            }
            else if (child.GetType() == typeof(FigmaLine))
            {
                var figmaLine = ((FigmaLine)child);
                var absolute = figmaLine.absoluteBoundingBox;
                var parentFrame = (IAbsoluteBoundingBox)parent;

                var figmaLineView = new NSView() { TranslatesAutoresizingMaskIntoConstraints = false };
                figmaLineView.WantsLayer = true;
                figmaLineView.AlphaValue = figmaLine.opacity;
                figmaLineView.Hidden = !child.visible;
                var fills = figmaLine.fills.OfType<FigmaPaint>().FirstOrDefault();
                if (fills != null)
                {
                    figmaLineView.Layer.BackgroundColor = ToNSColor(fills.color).CGColor;
                }

                var strokes = figmaLine.strokes.FirstOrDefault();
                if (strokes != null)
                {
                    if (strokes.color != null)
                    {
                        figmaLineView.Layer.BackgroundColor = ToNSColor(strokes.color).CGColor;
                    }
                }

                parentView.AddSubview(figmaLineView);

                var lineWidth = absolute.width == 0 ? figmaLine.strokeWeight : absolute.width;

                var constraintWidth = figmaLineView.WidthAnchor.ConstraintEqualToConstant(lineWidth);
                constraintWidth.Priority = (uint)NSLayoutPriority.DefaultLow;
                constraintWidth.Active = true;

                var lineHeight = absolute.height == 0 ? figmaLine.strokeWeight : absolute.height;

                var constraintHeight = figmaLineView.HeightAnchor.ConstraintEqualToConstant(lineHeight);
                constraintHeight.Priority = (uint)NSLayoutPriority.DefaultLow;
                constraintHeight.Active = true;

                CreateConstraints(figmaLineView, parentView, figmaLine.constraints, absolute, parentFrame.absoluteBoundingBox);
            }
            else
            {
                Console.WriteLine("[{1}({2})] Not implemented: {0}", child.GetType(), child.id, child.name);
                if (child is IFigmaNodeContainer nodeContainer)
                {
                    foreach (var item in nodeContainer.children)
                    {
                        ToNSView(parent, parentView, item, figmaImageViews);
                    }
                }
            }
            return null;
        }

    }
}
