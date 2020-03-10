/*
*/
using System;
using FigmaSharp.Models;
using AppKit;
using Foundation;
using System.Collections.Generic;
using System.Linq;

namespace MonoDevelop.Figma.FigmaBundles
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
		(string name, FigmaFileVersion version) current_item = ("Current", null);

		List<(string name, FigmaFileVersion version)> named_version_items = new List<(string, FigmaFileVersion)>();
		List<(string name, FigmaFileVersion version)> other_version_items = new List<(string, FigmaFileVersion)>();

		public event EventHandler<FigmaFileVersion> VersionSelected;

		NSPopUpButton menu;

		public VersionMenu(NSPopUpButton view)
		{
			this.menu = view;
		}

		public void Reset()
		{
			named_version_items.Clear();
			other_version_items.Clear();
		}

		internal void AddItem(FigmaFileVersion version)
		{

			if (!string.IsNullOrEmpty(version.label))
			{
				named_version_items.Add((version.label, version));
			}
			else
			{
				named_version_items.Add((version.created_at.ToString("g"), version));
			}
		}

		public void GeneratePopup()
		{
			menu.RemoveAllItems();

			menu.AddItem(current_item.name);
			menu.AddItem("-");
			//menu.AddItem (new NSMenuItem ("Labeled") { Enabled = false });

			foreach (var item in named_version_items)
				menu.AddItem(item.name);

			menu.AddItem("-");

			foreach (var item in other_version_items.Skip(1)) // First item is "Current"
				menu.AddItem(item.name);
		}
	}
}
