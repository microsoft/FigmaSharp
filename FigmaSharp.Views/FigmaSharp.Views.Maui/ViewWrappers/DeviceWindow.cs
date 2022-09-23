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

using System;
using FigmaSharp.Maui;
using Microsoft.Maui.Controls;

namespace FigmaSharp.Views.Maui
{
	public class DeviceWindow : IDeviceWindow, IDisposable
	{
		public event EventHandler<Key> KeyDown;
		public event EventHandler Closing;
		public event EventHandler Resize;

		ContentPage window;

		public DeviceWindow () : this (new ContentPage ())
		{

		}

		public DeviceWindow (ContentPage window)
		{
			this.window = window;
			Content = new FigmaSharp.Views.Maui.View ();
		}

		protected virtual void OnKeyDownPressed (object sender, Key args)
		{
			KeyDown?.Invoke (this, args);
		}

		public Size Size {
			get => new Size ((float)window.Width, (float)window.Height);
		}

		IView content;
		public IView Content {
			get => content;
			set {
				content = value;
				this.window.Content = content.NativeObject as Microsoft.Maui.Controls.View;
			}
		}

		public object NativeObject => window;

		public Color BackgroundColor {
			get => window.BackgroundColor.ToFigmaColor ();
			set {
				window.BackgroundColor = value.ToMauiColor ();
			}
		}

		public void AddChild (IWindow window)
		{
			//To implement
		}

		public void RemoveChild (IWindow window)
		{
			//To implement
		}

		public void Dispose ()
		{

		}
	}
}

