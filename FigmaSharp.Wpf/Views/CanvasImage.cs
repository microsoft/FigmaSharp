/* 
 * FigmaRectangleVectorConverter.cs
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

using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace FigmaSharp.Wpf
{
    public class CanvasImage : Canvas, IDisposable
    {
        public Image ImageView {
            get;
            private set;
        }

        public CanvasImage ()
        {
            ImageView = new Image();
            ImageView.ClipToBounds = true;
            this.SizeChanged += CanvasImage_SizeChanged;
        }

        private void CanvasImage_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            Refresh();
        }

        internal void SetImage(ImageSource imageSource)
        {
            if (imageSource == null)
            {
                Children.Remove(ImageView);
                ImageView.Source = null;
            }
            else
            {
                if (!Children.Contains(ImageView))
                    Children.Add(ImageView);
                ImageView.Source = imageSource;

                Refresh();
            }
        }

        void Refresh ()
        {
            if (!Children.Contains(ImageView))
                return;
            SetLeft(ImageView, 0);
            SetTop(ImageView, 0);
            ImageView.Width = Width;
            ImageView.Height = Height;
        }

        public void Dispose()
        {
            this.SizeChanged -= CanvasImage_SizeChanged;
        }
    }
}
