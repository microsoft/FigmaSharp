/* 
 * Author:
 *   Hylke Bons <hylbo@microsoft.com>
 *
 * Copyright (C) 2019 Microsoft, Corp
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
using System.Linq;
using System.Collections.Generic;

using AppKit;
using Foundation;
using ObjCRuntime;

using FigmaSharp.Models;

namespace FigmaSharpApp
{
	class MenuVersionItem : NSMenuItem
	{
		public FigmaFileVersion Version { get; set; }

		public MenuVersionItem()
		{
		}

		public MenuVersionItem(string title) : base(title)
		{
		}

		public MenuVersionItem(NSCoder coder) : base(coder)
		{
		}

		public MenuVersionItem(string title, EventHandler handler) : base(title, handler)
		{
		}

		public MenuVersionItem(string title, string charCode) : base(title, charCode)
		{
		}

		public MenuVersionItem(string title, string charCode, EventHandler handler) : base(title, charCode, handler)
		{
		}

		public MenuVersionItem(string title, Selector selectorAction, string charCode) : base(title, selectorAction, charCode)
		{
		}

		public MenuVersionItem(string title, string charCode, EventHandler handler, Func<NSMenuItem, bool> validator) : base(title, charCode, handler, validator)
		{
		}

		protected MenuVersionItem(NSObjectFlag t) : base(t)
		{
		}

		protected internal MenuVersionItem(IntPtr handle) : base(handle)
		{
		}
	}

	class VersionMenu
	{
		NSMenuItem current_item = new NSMenuItem ("Current");
		List<MenuVersionItem> named_version_items = new List<MenuVersionItem> ();
		List<MenuVersionItem> other_version_items = new List<MenuVersionItem> ();

		public event EventHandler<FigmaFileVersion> VersionSelected;

		public VersionMenu ()
		{
			current_item.State = NSCellStateValue.On;
			current_item.Activated += delegate {
				ResetStates ();

				current_item.State = NSCellStateValue.On;
				VersionSelected?.Invoke (this, null);
			};
		}

		public void Reset ()
		{
			named_version_items = new List<MenuVersionItem> ();
			other_version_items = new List<MenuVersionItem> ();
		}

		void ResetStates ()
		{
			current_item.State = NSCellStateValue.Off;

			foreach (NSMenuItem labeled_item in named_version_items)
				labeled_item.State = NSCellStateValue.Off;

			foreach (NSMenuItem labeled_item in other_version_items)
				labeled_item.State = NSCellStateValue.Off;
		}

		internal void AddItem (FigmaFileVersion version)
		{
			var item = new MenuVersionItem() { Version = version };

			item.Activated += delegate {
				ResetStates();

				item.State = NSCellStateValue.On;
				VersionSelected?.Invoke (this, version);
			};

			if (!string.IsNullOrEmpty(version.label))
			{
				item.Title = version.label;
				named_version_items.Add(item);

			}
			else
			{
				item.Title = version.created_at.ToString("g");
				other_version_items.Add(item);
			}
		}

		public void UseAsVersionsMenu ()
		{
			NSMenu menu = NSApplication.SharedApplication.MainMenu.ItemWithTitle ("Versions").Submenu;
			menu.RemoveAllItems ();

			menu.AddItem (current_item);
			menu.AddItem (NSMenuItem.SeparatorItem);
			menu.AddItem (new NSMenuItem ("Labeled") { Enabled = false });

			foreach (NSMenuItem item in named_version_items)
				menu.AddItem (item);

			menu.AddItem (NSMenuItem.SeparatorItem);
			menu.AddItem (new NSMenuItem ("Autosaved") { Enabled = false });

			foreach (NSMenuItem item in other_version_items.Skip(1)) // First item is "Current"
				menu.AddItem (item);

			menu.Update ();
		}
	}
}
