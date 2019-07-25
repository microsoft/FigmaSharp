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
using System.Collections.Generic;

using AppKit;

namespace FigmaSharp.Samples
{
    class VersionMenu
    {
        NSMenuItem current_item = new NSMenuItem("Current");

        List<NSMenuItem> named_version_items = new List<NSMenuItem>();
        List<NSMenuItem> other_version_items = new List<NSMenuItem>();

        public event VersionSelectedHandler VersionSelected = delegate { };
        public delegate void VersionSelectedHandler(string id);


        public VersionMenu()
        {
            current_item.State = NSCellStateValue.On;
            current_item.Activated += delegate {
                ResetStates();

                current_item.State = NSCellStateValue.On;
                VersionSelected("Current");
            };
        }


        public void Reset()
        {
            named_version_items = new List<NSMenuItem>();
            other_version_items = new List<NSMenuItem>();
        }


        void ResetStates()
        {
            current_item.State = NSCellStateValue.Off;

            foreach (NSMenuItem labeled_item in named_version_items)
                labeled_item.State = NSCellStateValue.Off;

            foreach (NSMenuItem labeled_item in other_version_items)
                labeled_item.State = NSCellStateValue.Off;
        }


        public void AddItem(string id, string name, DateTime timestamp)
        {
            var item = new NSMenuItem();

            item.Activated += delegate {
                ResetStates();

                item.State = NSCellStateValue.On;
                VersionSelected (id);
            };

            if (!string.IsNullOrEmpty(name)) {
                item.Title = name;
                named_version_items.Add(item);

            } else {
                item.Title = timestamp.ToString("r");
                other_version_items.Add(item);
            }
        }


        public void AddItem(string id, DateTime timestamp)
        {
            AddItem(id, null, timestamp);
        }


        public void UseAsVersionsMenu()
        {
            NSMenu menu = NSApplication.SharedApplication.MainMenu.ItemWithTitle("Versions").Submenu;
            menu.RemoveAllItems();

            menu.AddItem(current_item);
            menu.AddItem(NSMenuItem.SeparatorItem);

            if (named_version_items.Count == 0 && other_version_items.Count == 0) {
                menu.AddItem(new NSMenuItem("No Version History") { Enabled = false });
            
            } else {
                menu.AddItem(new NSMenuItem("Labels") { Enabled = false });

                foreach (NSMenuItem item in named_version_items)
                    menu.AddItem(item);

                menu.AddItem(NSMenuItem.SeparatorItem);
                menu.AddItem(new NSMenuItem("Autosaves") { Enabled = false });

                foreach (NSMenuItem item in other_version_items)
                    menu.AddItem(item);
            }

            menu.Update();
        }
    }
}
