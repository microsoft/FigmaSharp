/* 
 * CustomTextFieldConverter.cs
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

using System.Linq;
using System.Text;

using AppKit;

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.NativeControls.Cocoa
{
	public class TextViewConverter : FigmaNativeControlConverter
	{
		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var value) && (value == NativeControlType.TextView);
		}

		protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			var figmaInstance = (FigmaFrameEntity)currentNode;

			figmaInstance.TryGetNativeControlType(out var controlType);
			ITextView view = new TextView();
			var scrollView = (NSScrollView)view.NativeObject;

			scrollView.Configure(currentNode);

			var textView = new NSTextView(new CoreGraphics.CGRect(0,0,scrollView.ContentSize.Width, scrollView.ContentSize.Height));
			textView.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize);
			textView.AutoresizingMask = NSViewResizingMask.WidthSizable;
			textView.TextContainer.Size = new CoreGraphics.CGSize( scrollView.ContentSize.Width, float.MaxValue);

			var texts = figmaInstance.children.OfType<FigmaText>();
			var text = texts.FirstOrDefault(s => s.name == "lbl" && s.visible);

			if (text != null) {
				textView.Value = text.characters;

				// TODO: text styling
				// tv.TextStorage.Append(new Foundation.NSAttributedString(""), null);
			}

			scrollView.BorderType = NSBorderType.LineBorder;
			scrollView.HasHorizontalScroller = false;
			scrollView.HasVerticalScroller = true;
			scrollView.DocumentView = textView;

			figmaInstance.TryGetNativeControlComponentType(out var controlComponentType);
			switch (controlComponentType)
			{
				case NativeControlComponentType.TextViewSmall:
				case NativeControlComponentType.TextViewSmallDark:
					textView.Font = NSFont.SystemFontOfSize(NSFont.SmallSystemFontSize);
					break;
			}

			return view;
		}

		protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			var instance = (FigmaFrameEntity)currentNode.Node;
			var name = currentNode.Name;

			var builder = new StringBuilder();

			if (rendererService.NeedsRenderConstructor (currentNode, parentNode))
				builder.WriteConstructor (name, typeof (NSScrollView));

			builder.Configure (instance, name);

			// TODO: textView.TextContainer.Size = new CoreGraphics.CGSize(scrollView.ContentSize.Width, float.MaxValue);

			string textViewName = name + "TextView";
			builder.WriteConstructor(textViewName, typeof(NSTextView));

			builder.WriteEquality(textViewName,
			                      nameof(NSTextView.Frame),
			                      string.Format("new {0} ({1}, {2}, {3}, {4})",
			                          typeof(CoreGraphics.CGRect), 0, 0, name + ".ContentSize.Width", name + ".ContentSize.Height"));

			builder.WriteEquality(textViewName, nameof(NSTextView.AutoresizingMask), NSViewResizingMask.WidthSizable.GetFullName());
			// TODO: builder.WriteEquality(textViewName, nameof(NSTextView.Font), string.Format("{0} ({1})", NSFont.SystemFontOfSize.GetType(), typeof(NSFont.SystemFontSize)));

			var texts = instance.children.OfType<FigmaText> ();
			var figmaTextNode = texts.FirstOrDefault (s => s.name == "lbl" && s.visible);

			if (figmaTextNode != null) {
				var text = figmaTextNode.characters ?? string.Empty;
				builder.WriteEquality (name, nameof (NSTextField.StringValue), text, true);
				//builder.Configure (figmaTextNode, name);
			}

			builder.WriteEquality(name, nameof(NSScrollView.BorderType), NSBorderType.LineBorder.GetFullName());
			builder.WriteEquality(name, nameof(NSScrollView.HasHorizontalRuler), false);
			builder.WriteEquality(name, nameof(NSScrollView.HasVerticalScroller), false);
			builder.WriteEquality(name, nameof(NSScrollView.DocumentView), textViewName);

			instance.TryGetNativeControlComponentType(out var controlComponentType);
			switch (controlComponentType)
			{
				case NativeControlComponentType.TextViewSmall:
				case NativeControlComponentType.TextViewSmallDark:
					// TODO: textView.Font = NSFont.SystemFontOfSize(NSFont.SmallSystemFontSize);
					break;
			}

			return builder;
		}
	}
}
