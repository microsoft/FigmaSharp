// Authors:
//   Jose Medrano <josmed@microsoft.com>
//   Hylke Bons <hylbo@microsoft.com>
//
// Copyright (C) 2020 Microsoft, Corp
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

using System;
using System.Linq;
using System.Text;
using AppKit;
using FigmaSharp.Cocoa;
using FigmaSharp.Cocoa.CodeGeneration;
using FigmaSharp.Cocoa.Helpers;
using FigmaSharp.Controls.Cocoa.Helpers;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Controls.Cocoa.Converters
{
	public class TextViewConverter : CocoaConverter
	{
		public override Type GetControlType(FigmaNode currentNode) => typeof(NSTextView);

		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var controlType) &&
				controlType == FigmaControlType.TextView;
		}

		protected override IView OnConvertToView(FigmaNode currentNode, ViewNode parentNode, ViewRenderService rendererService)
		{
			var frame = (FigmaFrame)currentNode;
			frame.TryGetNativeControlType(out var controlType);

			var scrollView = new NSScrollView();

			var textView = new NSTextView(
                new CoreGraphics.CGRect(0, 0, scrollView.ContentSize.Width, scrollView.ContentSize.Height));

			textView.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize);
			textView.AutoresizingMask = NSViewResizingMask.WidthSizable;
			textView.TextContainer.Size = new CoreGraphics.CGSize( scrollView.ContentSize.Width, float.MaxValue);

			var texts = frame.children.OfType<FigmaText>();
			FigmaText text = texts.FirstOrDefault(s => s.name == "lbl" && s.visible);

			if (text != null) {
				textView.Value = text.characters;

				// TODO: text styling
				// textView.TextStorage.Append(new Foundation.NSAttributedString(""), null);
			}

			scrollView.BorderType = NSBorderType.LineBorder;
			scrollView.HasHorizontalScroller = false;
			scrollView.HasVerticalScroller = true;
			scrollView.DocumentView = textView;

			frame.TryGetNativeControlVariant(out var controlVariant);

            if (controlVariant == NativeControlVariant.Small)
				textView.Font = NSFont.SystemFontOfSize(NSFont.SmallSystemFontSize);

			return new View(scrollView);
		}

		protected override StringBuilder OnConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
		{
			var code = new StringBuilder();

			// TODO: Get name from generator
			var name = currentNode.Name + "ScrollView";
			string textViewName = currentNode.Name;

			currentNode.Name = name;

			var frame = (FigmaFrame)currentNode.Node;
			frame.TryGetNativeControlVariant(out var controlVariant);

			code.WriteConstructor (name, typeof (NSScrollView));
			code.WritePropertyEquality(name, nameof(NSScrollView.BorderType), NSBorderType.LineBorder.GetFullName());
			code.WritePropertyEquality(name, nameof(NSScrollView.HasHorizontalRuler), false);
			code.WritePropertyEquality(name, nameof(NSScrollView.HasVerticalScroller), true);

			code.AppendLine();

			if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
				code.WriteConstructor(textViewName, typeof(NSTextView), rendererService.NodeRendersVar(currentNode, parentNode));

			code.WritePropertyEquality(textViewName,
			    nameof(NSTextView.Frame),
			    string.Format("new {0} ({1}, {2}, {3}, {4})",
			    typeof(CoreGraphics.CGRect), 0, 0, name + ".ContentSize.Width", name + ".ContentSize.Height"));

			code.WritePropertyEquality(textViewName, nameof(NSTextView.AutoresizingMask), NSViewResizingMask.WidthSizable.GetFullName());

			// TODO: Use CocoaConverter methods
			switch (controlVariant)
			{
				case NativeControlVariant.Regular:
					code.WritePropertyEquality(textViewName, nameof(NSTextView.Font), Members.Font.SystemFontOfSize(Members.Font.SystemFontSize));
					break;
				case NativeControlVariant.Small:
					code.WritePropertyEquality(textViewName, nameof(NSTextView.Font), Members.Font.SystemFontOfSize(Members.Font.SmallSystemFontSize));
					break;
			}

			var texts = frame.children.OfType<FigmaText> ();
			FigmaText text = texts.FirstOrDefault (s => s.name == ComponentString.TEXT && s.visible);

			if (text != null)
			{
				var stringLabel = rendererService.TranslationService.GetTranslatedText(text.characters, rendererService);
				code.WritePropertyEquality (textViewName, nameof (NSTextView.Value), stringLabel, inQuotes: !rendererService.Options.TranslateLabels);
			}

			code.AppendLine();
			code.WritePropertyEquality(name, nameof(NSScrollView.DocumentView), textViewName);

			return code;
		}
	}
}
