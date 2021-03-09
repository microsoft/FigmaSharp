using System;
using System.Collections.Generic;

using AppKit;
using Foundation;

namespace NAMESPACE
{
    partial class MYOUTLINEVIEW : AppKit.NSOutlineView
    {
        public MyOutlineView()
        {
            InitializeComponent();

            Delegate = new OutlineDelegate();
            DataSource = new OutlineDataSource();
        }


        class OutlineDelegate : NSOutlineViewDelegate
        {
            // Return your column views with data here
            public override NSView GetViewForItem(NSOutlineView outlineView, NSTableColumn tableColumn, nint row)
            {
                if (tableColumn.Identifier == "Column1")
                {
                    return new NSView()
                    {
                        // Property1 = OutlineDataSource.Data[(int)row].Property1,
                        // Property2 = OutlineDataSource.Data[(int)row].Property2
                    };
                }

                if (tableColumn.Identifier == "Column2")
                {
                    return new NSView()
                    {
                        // Property3 = OutlineDataSource.Data[(int)row].Property3
                    };
                }

                // Etc…

                return null;
            }


            // Handle row selections here
            public override bool ShouldSelectRow(NSOutlineView outlineView, nint row)
            {
                return true;
            }
        }


        class OutlineDataSource : NSOutlineViewDataSource
        {
            public static List<object> Data = new List<object>();


            public override nint GetChildrenCount(NSOutlineView outlineView, NSObject item)
            {
                return 0;
            }

            public override NSObject GetChild(NSOutlineView outlineView, nint childIndex, NSObject item)
            {

                return new NSObject();
            }

            public override bool ItemExpandable(NSOutlineView outlineView, NSObject item)
            {
                return false;
            }
        }
    }
}
