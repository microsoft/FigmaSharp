//
// Author:
//   netonjm <josmed@microsoft.com>
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
//
using System;
using System.Collections.Generic;
using AppKit;
using FigmaSharp.Models;
using System.Linq;

namespace FigmaSharp.Cocoa
{
	public class FigmaVersionMenu
	{
		public event EventHandler<FigmaFileVersion> VersionSelected;

		(NSMenuItem menu, FigmaFileVersion version) current_item;

		public NSMenuItem CurrentMenu => current_item.menu;
		public FigmaFileVersion CurrentVersion => current_item.version;

		List <(NSMenuItem menu, FigmaFileVersion version)> named_version_items = new List<(NSMenuItem, FigmaFileVersion)>();
		List<(NSMenuItem menu, FigmaFileVersion version)> other_version_items = new List<(NSMenuItem, FigmaFileVersion)>();

		public FigmaVersionMenu ()
		{
			current_item = (CreateMenuItem("Current"), null);
		}

		public void Clear (NSMenu submenu = null)
		{
			named_version_items.Clear();
			other_version_items.Clear();

			if (submenu != null) {
				submenu.RemoveAllItems();
				submenu.AddItem(new NSMenuItem("No Version History") { Enabled = false });
				submenu.Update();
			}
		}

		public void AddItem (FigmaFileVersion version)
		{
			if (!string.IsNullOrEmpty(version.label)) {
				named_version_items.Add((CreateMenuItem (version.label, version), version));
			} else {
				other_version_items.Add((CreateMenuItem(version.created_at.ToString("g"), version), version));
			}
		}

		NSMenuItem CreateMenuItem (string label, FigmaFileVersion version = null)
		{
			var menuItem = new NSMenuItem(label);
			menuItem.Activated += (s,e) => {
				ResetStates();
				((NSMenuItem)s).State = NSCellStateValue.On;
				VersionSelected?.Invoke(this, version);
			};
			return menuItem;
		}

		void ResetStates()
		{
			current_item.menu.State = NSCellStateValue.Off;

			foreach (var labeled_item in named_version_items)
				labeled_item.menu.State = NSCellStateValue.Off;

			foreach (var labeled_item in other_version_items)
				labeled_item.menu.State = NSCellStateValue.Off;
		}

		public void Generate (NSMenu menu)
		{
			menu.RemoveAllItems();

			menu.AddItem(current_item.menu);

			if (named_version_items.Count > 0) {
				menu.AddItem(NSMenuItem.SeparatorItem);
				menu.AddItem(new NSMenuItem("Labeled") { Enabled = false });
				foreach (var item in named_version_items)
					menu.AddItem(item.menu);
			}

			var otherVersionItems = other_version_items.Skip(1);
			if (otherVersionItems.Count () > 0) {
				menu.AddItem(NSMenuItem.SeparatorItem);
				menu.AddItem(new NSMenuItem("Autosaved") { Enabled = false });

				foreach (var item in otherVersionItems) // First item is "Current"
					menu.AddItem(item.menu);
			} 

			menu.Update();
		}

		internal NSMenuItem GetMenuItem (FigmaFileVersion fileVersion)
		{
			if (fileVersion == other_version_items.FirstOrDefault ().version)
				return current_item.menu;
			var version = named_version_items.FirstOrDefault(s => s.version.id == fileVersion.id).menu;
			if (version != null)
				return version;
			version = other_version_items.FirstOrDefault(s => s.version.id == fileVersion.id).menu;
			return version;
		}

		public FigmaFileVersion GetFileVersion (NSMenuItem menu)
		{
			if (current_item.menu == menu)
				return current_item.version;
			var version = named_version_items.FirstOrDefault(s => s.menu == menu).version;
			if (version != null)
				return version;
			version = other_version_items.FirstOrDefault(s => s.menu == menu).version;
			return version;
		}
	}
}
