// Authors:
//   Jose Medrano <josmed@microsoft.com>
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
using System;
using System.Collections.Generic;
using System.Linq;
using FigmaSharp.Models;
using FigmaSharp.Services;

namespace FigmaSharp.Controls.Cocoa
{
	public static class NodeExtensions
	{
        public static bool IsDialogParentContainer(this FigmaNode figmaNode, FigmaControlType controlType)
        {
            return figmaNode is IFigmaNodeContainer container && container.children
                .OfType<FigmaInstance>()
                .Any(s => s.IsWindowOfType(controlType));
        }

        public static bool IsDialog(this FigmaNode figmaNode)
        {
            if (TryGetNativeControlType(figmaNode, out var value) &&
                (value == FigmaControlType.WindowPanel || value == FigmaControlType.WindowSheet || value == FigmaControlType.Window))
            {
                return true;
            }
            return false;
        }

        public static FigmaInstance GetDialogInstanceFromParentContainer(this FigmaNode figmaNode)
        {
            if (!(figmaNode is IFigmaNodeContainer cont))
                return null;
            var dialog = cont.children.OfType<FigmaInstance>()
                .FirstOrDefault(s => s.IsDialog());
            return dialog;
        }

        public static bool IsComponentContainer(this FigmaNode node)
        {
            return (node is FigmaInstance || node is FigmaComponentEntity) && node.name.Contains("!container");
        }

        public static bool IsWindowOfType(this FigmaNode figmaNode, FigmaControlType controlType)
        {
            if (figmaNode.TryGetNativeControlType(out var value) && value == controlType)
            {
                return true;
            }
            return false;
        }

        public static bool TryGetNativeControlVariant(this FigmaNode node, out NativeControlVariant nativeControlVariant)
        {
            nativeControlVariant = NativeControlVariant.NotDefined;
            if (node is FigmaComponentEntity)
            {
                nativeControlVariant = GetNativeControlVariant(node.name);
                return nativeControlVariant != NativeControlVariant.NotDefined;
            }

            if (node is FigmaInstance figmaInstance && figmaInstance.Component != null)
            {
                nativeControlVariant = figmaInstance.Component.ToNativeControlVariant();
                return nativeControlVariant != NativeControlVariant.NotDefined;
            }

            return false;
        }

        public static bool TryGetNativeControlType(this FigmaNode node, out FigmaControlType nativeControlType)
        {
            nativeControlType = FigmaControlType.NotDefined;
            if (node is FigmaComponentEntity)
            {
                nativeControlType = GetNativeControlType(node.name);
                return nativeControlType != FigmaControlType.NotDefined;
            }

            if (node is FigmaInstance figmaInstance && figmaInstance.Component != null)
            {
                nativeControlType = figmaInstance.Component.ToNativeControlType();
                return nativeControlType != FigmaControlType.NotDefined;
            }

            return false;
        }

        public static NativeControlVariant ToNativeControlVariant(this FigmaComponent figmaComponent)
        {
            return GetNativeControlVariant(figmaComponent.name);
        }

        public static FigmaControlType ToNativeControlType(this FigmaComponent figmaComponent)
        {
            return GetNativeControlType(figmaComponent.name);
        }

        static FigmaControlType GetNativeControlType(string name)
        {
            var found = ControlTypeService.GetByName(name);
            if (found.Equals(default))
            {
                Console.WriteLine("Component Key not found: {0}", name);
                return FigmaControlType.NotDefined;
            }
            return found.nativeControlType;
        }

        static NativeControlVariant GetNativeControlVariant(string name)
        {
            var found = ControlTypeService.GetByName(name);
            if (found.Equals(default))
            {
                Console.WriteLine("Component Key not found: {0}", name);
                return NativeControlVariant.NotDefined;
            }
            return found.nativeControlVariant;
        }

        public static bool IsWindowContent(this FigmaNode node)
        {
            return (node.Parent?.IsDialogParentContainer() ?? false) && node.IsNodeWindowContent();
        }

        public static bool IsParentMainContainer(this FigmaNode node)
        {
            if (node is FigmaFrame FigmaFrame && FigmaFrame.Parent is FigmaCanvas)
            {
                return true;
            };
            return false;
        }

        public static bool IsParentMainContainerContent(this FigmaNode node)
        {
            return (node.Parent?.IsParentMainContainer() ?? false) && node.IsNodeWindowContent();
        }

        public static bool TryGetInstanceDialogParentContainer(this FigmaNode figmaNode, INodeProvider provider, out FigmaInstance instanceDialog)
        {
            if (figmaNode is IFigmaNodeContainer container)
            {
                foreach (var item in container.children)
                {
                    if (item is FigmaInstance figmaInstance && provider.TryGetMainInstance(figmaInstance, out instanceDialog))
                    {
                        return true;
                    }
                }

            }
            instanceDialog = null;
            return false;
        }

        public static bool IsInstanceContent(this FigmaNode node, INodeProvider provider, out FigmaInstance instanceDialog)
        {
            if (node.Parent != null && TryGetInstanceDialogParentContainer(node.Parent, provider, out instanceDialog) && node.IsNodeWindowContent())
            {
                return true;
            }
            instanceDialog = null;
            return false;
        }

        public static bool IsComponentContent(this FigmaNode node, INodeProvider provider, out FigmaComponentEntity instanceDialog)
        {
            if (node.Parent != null && TryGetComponentDialogParentContainer(node.Parent, provider, out instanceDialog) && node.IsNodeWindowContent())
            {
                return true;
            }
            instanceDialog = null;
            return false;
        }

        public static bool TryGetComponentDialogParentContainer(this FigmaNode figmaNode, INodeProvider provider, out FigmaComponentEntity instanceDialog)
        {
            if (figmaNode is IFigmaNodeContainer container)
            {
                foreach (var item in container.children)
                {
                    if (item is FigmaInstance figmaInstance && provider.TryGetMainComponent (figmaInstance, out instanceDialog))
                    {
                        return true;
                    }
                }

            }
            instanceDialog = null;
            return false;
        }

        public static FigmaNode GetWindowContent(this FigmaNode node)
        {
            if (node is IFigmaNodeContainer nodeContainer)
            {
                if (node.IsDialogParentContainer())
                {
                    var content = nodeContainer.children.FirstOrDefault(s => s.IsWindowContent());
                    return content;
                }
            }
            return null;
        }

        public static bool IsMainDocumentView(this FigmaNode node)
        {
            return node.Parent is FigmaCanvas && !node.IsDialogParentContainer();
        }

        public static bool IsNodeWindowContent(this FigmaNode node)
        {
            return node.GetNodeTypeName() == "content";
        }

        public static bool IsNodeWindowMasterContent(this FigmaNode node)
        {
            return node.GetNodeTypeName() == "mastercontent";
        }

        public static bool HasChildren(this FigmaNode node)
        {
            return node is IFigmaNodeContainer;
        }

        public static bool IsDialogParentContainer(this FigmaNode figmaNode)
        {
            return figmaNode is IFigmaNodeContainer container
                && container.children.Any(s => s.IsDialog());
        }

        /// <summary>
		/// Gets current FigmaNode children WITHOUT the FigmaInstance
		/// </summary>
		/// <param name="figmaNode"></param>
		/// <param name="func"></param>
		/// <param name="reverseChildren"></param>
		/// <returns></returns>
        public static IEnumerable<FigmaNode> GetChildren(this FigmaNode figmaNode, Func<FigmaNode, bool> func = null, bool reverseChildren = false)
        {
            if ((figmaNode.GetWindowContent() ?? figmaNode) is IFigmaNodeContainer container)
            {

                var figmaInstance = figmaNode.GetDialogInstanceFromParentContainer();
                IEnumerable<FigmaNode> children = container.children;
                if (reverseChildren)
                    children = children.Reverse();

                foreach (var item in children)
                {
                    if (item == figmaInstance)
                        continue;

                    if (func == null || (func != null && func.Invoke(item)))
                        yield return item;
                }
            }
        }

        public static FigmaNode FirstChild(this FigmaNode figmaNode, Func<FigmaNode, bool> func = null)
        {
            var item = figmaNode.GetChildren(s => func?.Invoke(s) ?? true);
            return item.FirstOrDefault();
        }

        public static FigmaNode Options(this FigmaNode figmaNode)
        {
            if (figmaNode == null)
                return null;

            return figmaNode.FirstChild(s => s.name == "!options");
        }

        //public static NativeControlComponentType ToControlType (this FigmaInstance figmaInstance)
        //{
        //    if (figmaInstance.Component != null) {
        //        var found = data.FirstOrDefault (s => s.name == figmaInstance.Component.key || s.name == figmaInstance.Component.name);
        //        if (!found.Equals (default)) {
        //            return found.nativeControlComponentType;
        //        }

        //        Console.WriteLine ("Component Key not found: {0} - {1}", figmaInstance.Component.key, figmaInstance.Component.name);
        //        //throw new KeyNotFoundException(figmaInstance.Component.key);
        //    }
        //    return NativeControlComponentType.NotDefined;
        //}
    }
}
