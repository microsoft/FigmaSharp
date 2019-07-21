/* 
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
using AppKit;
using LiteForms;

namespace LiteForms.Cocoa
{
	public class Window : IWindow, IDisposable
	{
		public event EventHandler Closing;

		NSWindow window;

		public virtual OnKeyDown ()
		{

		}

		public string Title
		{
			get => window.Title;
			set => window.Title = value;
		}

		public Window(Rectangle rectangle) : this(new NSWindow(rectangle.ToCGRect(), NSWindowStyle.Titled | NSWindowStyle.Resizable | NSWindowStyle.Closable, NSBackingStore.Buffered, false))
		{

		}

		public Window(NSWindow window)
		{
			this.window = window;

			this.window.WillClose += Window_WillClose;
		}

		private void Window_WillClose(object sender, EventArgs e)
		{
			Closing?.Invoke(this, EventArgs.Empty);
		}

		IView content;
		public IView Content
		{
			get => content;
			set
			{
				content = value;
				this.window.ContentView = content.NativeObject as NSView;
			}
		}

		public object NativeObject => window;

		public void AddChild(IWindow window)
		{
			//To implement
		}

		public void RemoveChild(IWindow window)
		{
			//To implement
		}

		public void ShowDialog ()
		{
			//To implement
		}

		public void Show ()
		{
			window.MakeKeyAndOrderFront(null);
		}

		public void Dispose()
		{
			this.window.WillClose -= Window_WillClose;
		}
	}
}

