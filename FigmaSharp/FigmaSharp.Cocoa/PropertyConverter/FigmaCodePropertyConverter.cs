/* 
 * FigmaLineConverter.cs 
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

using FigmaSharp.Models;
using FigmaSharp.Services;
using AppKit;
using System;

namespace FigmaSharp.Cocoa.Converters
{
	public class FigmaCodePropertyConverter : FigmaCodePropertyConverterBase
	{
		protected virtual string GetDefaultParentName (FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			return CodeGenerationHelpers.This;
		}

		public override string ConvertToCode(string propertyName, FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			if (propertyName == CodeProperties.Frame)
			{
				System.Text.StringBuilder builder = new System.Text.StringBuilder();

				if (currentNode.Node is IAbsoluteBoundingBox absoluteBounding)
				{
					var constrainedNode = currentNode.Node as IConstraints;

					var name = currentNode.Name;
					
					//width
					var widthConstraintName = $"{name}WidthConstraint";

					var widthMaxStringValue = Math.Max(absoluteBounding.absoluteBoundingBox.Width, 1).ToDesignerString ();
					var widthConstraintStringValue = CodeGenerationHelpers.GetWidthConstraintEqualToConstant (name, widthMaxStringValue);
					builder.AppendLine($"var {widthConstraintName} = {widthConstraintStringValue};");

				
					if (constrainedNode != null && constrainedNode.constraints.IsFlexibleHorizontal)
						builder.WriteEquality(widthConstraintName, nameof(AppKit.NSLayoutConstraint.Priority), $"({typeof(int).FullName}){typeof(NSLayoutPriority)}.{nameof(NSLayoutPriority.DefaultLow)}");

					builder.WriteEquality(widthConstraintName, nameof(AppKit.NSLayoutConstraint.Active), true);

					//height
					var heightConstraintName = $"{name}HeightConstraint";

					var heightStringValue = Math.Max(absoluteBounding.absoluteBoundingBox.Height, 1).ToDesignerString();
					var heightConstraintStringValue = CodeGenerationHelpers.GetHeightConstraintEqualToConstant(name, heightStringValue);

					builder.AppendLine($"var {heightConstraintName} = {heightConstraintStringValue};");

					if (constrainedNode != null && constrainedNode.constraints.IsFlexibleVertical)
						builder.WriteEquality(heightConstraintName, nameof(AppKit.NSLayoutConstraint.Priority), $"({typeof (int).FullName}){typeof(NSLayoutPriority)}.{nameof(NSLayoutPriority.DefaultLow)}");

					builder.WriteEquality(heightConstraintName, nameof(AppKit.NSLayoutConstraint.Active), true);

					return builder.ToString();
				}
				return string.Empty;
			}
			if (propertyName == CodeProperties.AddChild)
			{
				return parentNode?.GetMethod(nameof(NSView.AddSubview), currentNode.Name);
			}
			if (propertyName == CodeProperties.Size)
			{
				if (currentNode.Node is IAbsoluteBoundingBox container)
				{
					if (currentNode.Node is FigmaLine line)
					{
						var width = container.absoluteBoundingBox.Width == 0 ? 1 : container.absoluteBoundingBox.Width;
						var height = container.absoluteBoundingBox.Height == 0 ? 1 : container.absoluteBoundingBox.Height;
						var size = typeof(CoreGraphics.CGSize).GetConstructor(new string[] { width.ToDesignerString(), height.ToDesignerString() });
						return currentNode.GetMethod(nameof(NSView.SetFrameSize), size);
					}

					var sizeConstructor = typeof(CoreGraphics.CGSize).GetConstructor(
					 container.absoluteBoundingBox.Width.ToDesignerString(),
					 container.absoluteBoundingBox.Height.ToDesignerString());
					return currentNode.GetMethod(nameof(NSView.SetFrameSize), sizeConstructor);

				}
				return string.Empty;
			}
			if (propertyName == CodeProperties.Position)
			{
				//first level has an special behaviour on positioning 
				if (currentNode.Node.Parent is FigmaCanvas)
					return string.Empty;

				if (currentNode.Node is IAbsoluteBoundingBox absoluteBounding && currentNode.Node.Parent is IAbsoluteBoundingBox parentAbsoluteBoundingBox)
				{
					var x = absoluteBounding.absoluteBoundingBox.X - parentAbsoluteBoundingBox.absoluteBoundingBox.X;

					var parentY = parentAbsoluteBoundingBox.absoluteBoundingBox.Y + parentAbsoluteBoundingBox.absoluteBoundingBox.Height;
					var actualY = absoluteBounding.absoluteBoundingBox.Y + absoluteBounding.absoluteBoundingBox.Height;
					var y = parentY - actualY;

					if (x != default || y != default)
					{
						var pointConstructor = CodeGenerationExtensions.GetConstructor(
						 typeof(CoreGraphics.CGPoint),
						 x.ToDesignerString(),
						 y.ToDesignerString()
					);
						return currentNode.GetMethod(nameof(AppKit.NSView.SetFrameOrigin), pointConstructor);
					}
				}
				return string.Empty;
			}

			if (propertyName == CodeProperties.Constraints)
			{
				if (currentNode.Node is IConstraints constrainedNode && currentNode.Node.Parent != null)
				{
					var parentNodeName = parentNode == null ?
						GetDefaultParentName(currentNode, parentNode, rendererService) :
						parentNode.Name;

					var builder = new System.Text.StringBuilder();

                    var constraints = constrainedNode.constraints;
					var absoluteBoundingBox = ((IAbsoluteBoundingBox)currentNode.Node)
						.absoluteBoundingBox;
					var absoluteBoundBoxParent = ((IAbsoluteBoundingBox)(parentNode == null ? currentNode.Node.Parent : parentNode.Node))
						.absoluteBoundingBox;

					if (constraints.horizontal.Contains("RIGHT") || constraints.horizontal == "SCALE")
					{
						var endPosition1 = absoluteBoundingBox.X + absoluteBoundingBox.Width;
						var endPosition2 = absoluteBoundBoxParent.X + absoluteBoundBoxParent.Width;
						var value = Math.Max(endPosition1, endPosition2) - Math.Min(endPosition1, endPosition2);

						var rightConstraintStringValue = CodeGenerationHelpers.GetRightConstraintEqualToAnchor (
							currentNode.Name, -value, parentNodeName);
						builder.WriteEquality(rightConstraintStringValue, nameof(NSLayoutConstraint.Active), true);
					}

					if (constraints.horizontal.Contains("LEFT"))
					{
						var value2 = absoluteBoundingBox.X - absoluteBoundBoxParent.X;
						var rightConstraintStringValue = CodeGenerationHelpers.GetLeftConstraintEqualToAnchor(
						currentNode.Name, value2, parentNodeName);
						builder.WriteEquality(rightConstraintStringValue, nameof(NSLayoutConstraint.Active), true);
					}

					if (constraints.vertical.Contains("BOTTOM") || constraints.horizontal == "SCALE")
					{
						var endPosition1 = absoluteBoundingBox.Y + absoluteBoundingBox.Height;
						var endPosition2 = absoluteBoundBoxParent.Y + absoluteBoundBoxParent.Height;
						var value2 = Math.Max(endPosition1, endPosition2) - Math.Min(endPosition1, endPosition2);

						var rightConstraintStringValue = CodeGenerationHelpers.GetBottomConstraintEqualToAnchor(
					currentNode.Name, -value2, parentNodeName);
						builder.WriteEquality(rightConstraintStringValue, nameof(NSLayoutConstraint.Active), true);
					}

					if (constraints.vertical.Contains("TOP"))
					{
						var value = absoluteBoundingBox.Y - absoluteBoundBoxParent.Y;

						var rightConstraintStringValue = CodeGenerationHelpers.GetTopConstraintEqualToAnchor(
						currentNode.Name, value, parentNodeName);
						builder.WriteEquality(rightConstraintStringValue, nameof(NSLayoutConstraint.Active), true);
					}

                    if (constraints.horizontal == "CENTER" || constraints.horizontal == "SCALE")
                    {
                        var delta = absoluteBoundingBox.X - absoluteBoundBoxParent.X - absoluteBoundBoxParent.Center.X;

						var rightConstraintStringValue = CodeGenerationHelpers.GetConstraintEqualToAnchor(
					currentNode.Name, nameof (NSView.LeftAnchor), delta, parentNodeName, nameof(NSView.CenterXAnchor));
						builder.WriteEquality(rightConstraintStringValue, nameof(NSLayoutConstraint.Active), true);
                    }

                    if (constraints.vertical == "CENTER" || constraints.vertical == "SCALE")
                    {
                        var delta = absoluteBoundingBox.Y - absoluteBoundBoxParent.Y - absoluteBoundBoxParent.Center.Y;

						var rightConstraintStringValue = CodeGenerationHelpers.GetConstraintEqualToAnchor(
				currentNode.Name, nameof(NSView.TopAnchor), delta, parentNodeName, nameof(NSView.CenterYAnchor));
						builder.WriteEquality(rightConstraintStringValue, nameof(NSLayoutConstraint.Active), true);
                    }

                    return builder.ToString();
				}
				return string.Empty;
			}


			throw new System.NotImplementedException(propertyName);
		}
	}
}
