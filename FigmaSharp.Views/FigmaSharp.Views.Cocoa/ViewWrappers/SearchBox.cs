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
using FigmaSharp.Views.Native.Cocoa;

namespace FigmaSharp.Views.Cocoa
{
	public class SearchBox : View, ISearchBox
	{
		FNSSearchField searchField;
		public event EventHandler Changed;

		public SearchBox () : this (new FNSSearchField ())
		{

		}

		public SearchBox (FNSSearchField searchField) : base (searchField)
		{
			this.searchField = searchField;
			this.searchField.TranslatesAutoresizingMaskIntoConstraints = false;
			this.searchField.Changed += TextField_Changed;
		}

		private void TextField_Changed (object sender, EventArgs e)
		{
			Changed?.Invoke (this, EventArgs.Empty);
		}

		public string Text {
			get => searchField.StringValue;
			set {
				searchField.StringValue = value ?? "";
			}
		}

		public string PlaceHolderString {
			get => searchField.PlaceholderString;
			set {
				searchField.PlaceholderString = value ?? "";
			}
		}

		public Color ForegroundColor {
			get => searchField.TextColor.ToColor ();
			set => searchField.TextColor = value.ToNSColor ();
		}

		public override void Dispose ()
		{
			searchField.Changed -= TextField_Changed;
			base.Dispose ();
		}
	}
}
