// Authors:
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

using Microsoft.Maui.Controls;

using FigmaSharp.Models;
using FigmaSharp.Views.Maui;
using Microsoft.Maui;
using FigmaSharp.Maui;

namespace FigmaSharp.Maui
{
    public static class ViewConfigureExtensions
    {
        public static void Configure(this FigmaSharp.Views.Maui.View view, FigmaFrame child)
        {
            var nativeView = view.NativeObject as Microsoft.Maui.Controls.View;

            Configure(nativeView, child);
            nativeView.Opacity = child.opacity;
            nativeView.BackgroundColor = child.backgroundColor.
                MixOpacity(child.opacity).ToMauiColor();
        }

        public static void Configure(this Microsoft.Maui.Controls.View view, FigmaNode child)
        {
            view.IsVisible = child.visible;
        }

        public static void Configure(this Microsoft.Maui.Controls.Label label, FigmaText text)
        {
            Configure(label, (FigmaNode)text);

            label.HorizontalTextAlignment = text.style.textAlignHorizontal == "CENTER" ? TextAlignment.Center : text.style.textAlignHorizontal == "LEFT" ? TextAlignment.Start : TextAlignment.End;
            label.Opacity = text.opacity;
            label.VerticalTextAlignment = text.style.textAlignVertical == "CENTER" ? TextAlignment.Center : text.style.textAlignHorizontal == "TOP" ? TextAlignment.Start : TextAlignment.End;

            if (text.HasFills)
                label.TextColor = text.fills[0].color.ToMauiColor();
        }
    }
}
