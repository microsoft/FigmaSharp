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

using FigmaSharp.Views.Native.Cocoa;
using System;

namespace FigmaSharp.Views.Cocoa
{
	public class ComboBox : View, IComboBox
	{
		public event EventHandler SelectionIndexChanged;

		FNSPopUpButton combo;

		public ComboBox () : this (new FNSPopUpButton ())
		{

		}

		public ComboBox (FNSPopUpButton textField) : base (textField)
		{
			this.combo = textField;
            this.combo.TranslatesAutoresizingMaskIntoConstraints = false;
			this.combo.Activated += Combo_Activated;
		}

		public string SelectedItem {
			get => combo.SelectedItem?.Title;
			set {
				var index = GetIndex (value);
				if (index == -1) {
					return;
				}
				SelectedIndex = index;
			}
		}

		public int GetIndex (string title)
		{
			var itm = combo.ItemWithTitle (title);
			if (itm == null) {
				return -1;
			}
			return (int)combo.IndexOfItem (itm);
		}

		public int SelectedIndex {
			get => (int)combo.IndexOfSelectedItem;
			set {
				if (value == SelectedIndex)
					return;

				combo.SelectItem (value);
				SelectionIndexChanged?.Invoke (this, EventArgs.Empty);
			}
		}

		private void Combo_Activated (object sender, EventArgs e)
		{
			SelectionIndexChanged?.Invoke (this, EventArgs.Empty);
		}

		public void AddItem (string Text)
		{
			this.combo.AddItem (Text);
		}

		public void RemoveItem (string Text)
		{
			this.combo.RemoveItem (Text);
		}

		public void ClearItems ()
		{
			combo.RemoveAllItems ();
		}

		public override void Dispose ()
		{
			this.combo.Activated -= Combo_Activated;
			base.Dispose ();
		}
	}
}
