using System;
using System.Collections.Generic;

using AppKit;

namespace NAMESPACE
{
    partial class MYTABLEVIEW : AppKit.NSTableView
    {
        public MyTableView()
        {
            InitializeComponent();

            Delegate = new TableDelegate();
            DataSource = new TableDataSource();
        }


        class TableDelegate : NSTableViewDelegate
        {
            // Return your column views with data here
            public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
            {
                if (tableColumn.Identifier == "Column1")
                {
                    return new NSView()
                    {
                        // Property1 = TableDataSource.Data[(int)row].Property1,
                        // Property2 = TableDataSource.Data[(int)row].Property2
                    };
                }

                if (tableColumn.Identifier == "Column2")
                {
                    return new NSView()
                    {
                        // Property3 = TableDataSource.Data[(int)row].Property3
                    };
                }

                // Etc…

                return null;
            }


            // Handle row selections here
            public override bool ShouldSelectRow(NSTableView tableView, nint row)
            {
                return true;
            }
        }


        class TableDataSource : NSTableViewDataSource
        {
            public static List<object> Data = new List<object>();

            public override nint GetRowCount(NSTableView tableView)
            {
                return Data.Count;
            }
        }
    }
}
