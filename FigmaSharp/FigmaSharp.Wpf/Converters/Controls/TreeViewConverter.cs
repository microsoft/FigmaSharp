using System;
using System.Linq;

using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Extensions;
using FigmaSharp.Views;
using FigmaSharp.Services;
using FigmaSharp.Controls;
using FigmaSharp.Views.Wpf;
using System.Windows.Controls;
using System.Windows.Automation;
using System.Windows;

namespace FigmaSharp.Wpf.Converters
{
    class TreeViewConverter: FrameConverterBase
    {
       
        public override Type GetControlType(FigmaNode currentNode) => typeof(IButton);

        public override bool CanConvert(FigmaNode currentNode)
        {
            currentNode.TryGetNativeControlType(out var controlType);
            return controlType == FigmaControlType.TreeView;
        }

        public TreeViewItem getTreeViewItems(FigmaNode figmaNode)
        {
            var treeViewItem = new TreeViewItem();

            if (figmaNode.GetType() == typeof(FigmaText) && figmaNode.name == ComponentString.ITEM_TITLE)
            {
                FigmaText text = (FigmaText)figmaNode;
                treeViewItem.Header = text.characters;
            }
            else if (figmaNode.GetType() == typeof(FigmaGroup))
            {
                FigmaGroup group = (FigmaGroup)figmaNode;
                treeViewItem.Header = group.name;
                foreach (FigmaNode node in group.children)
                {
                    TreeViewItem treeItemChild = getTreeViewItems(node);
                    if (treeItemChild != null)
                    {
                        treeViewItem.Items.Add(treeItemChild);
                    }
                }
            }
            else
            {
                return null;
            }

            return treeViewItem;
        }
        public override IView ConvertToView(FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
        {
            var treeView = new TreeView();


            var frame = (FigmaFrame)currentNode;
            frame.TryGetNativeControlType(out var controlType);
            frame.TryGetNativeControlVariant(out var controlVariant);

            FigmaGroup items = frame.children
                .OfType<FigmaGroup>()
                .FirstOrDefault(s => s.visible && s.name == ComponentString.ITEMS);

            treeView.Configure(frame);
            treeView.ConfigureAutomationProperties(frame);
            treeView.ConfigureTooltip(frame);
            treeView.ConfigureTabIndex(frame);
            treeView.configureAlignment(parent);

            if (items != null)
            {
                foreach (FigmaNode node in items.children)
                {
                    var treeViewChild = getTreeViewItems(node);
                    if(treeViewChild != null)
                    {
                        treeView.Items.Add(getTreeViewItems(node));
                    }
                    
                }
            }      

            var wrapper = new View(treeView);
            return wrapper;
        }

        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            return string.Empty;
        }
    }
}
