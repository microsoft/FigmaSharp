// Authors:
//   Hylke Bons <hylbo@microsoft.com>
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

using System.Linq;
using FigmaSharp.Models;

namespace FigmaSharp.Controls.Cocoa
{
    internal static class AccessibilityNodeExtensions
    {
        internal const string a11yLabel = "label";
        internal const string a11yHelp = "help";
        internal const string a11yRole = "role";

        internal const string a11yRoleGroup = "group";

        internal const string a11yNodeName = "a11y";

        public static bool IsA11Group(this FigmaNode node)
        {
            var a11node = node.GetA11Node();
            if (a11node != null && a11node.TryGetChildPropertyValue(a11yRole, out var value) && value == a11yRoleGroup)
                return true;
            return false;
        }

        public static bool IsA11Enabled(this FigmaNode node)
        {
            return GetA11Node(node)?.visible ?? false;
        }

        public static FigmaNode GetA11Node(this FigmaNode node)
        {
            return (node as IFigmaNodeContainer)?.children?.FirstOrDefault(s => s.GetNodeTypeName () == a11yNodeName);
        }

        public static bool TrySearchA11Label(this FigmaNode node, out string label)
        {
            var a11node = node.GetA11Node();
            if (a11node != null && a11node.TryGetChildPropertyValue(a11yLabel, out label))
                return true;
            label = null;
            return false;
        }

        public static bool TrySearchA11Help(this FigmaNode node, out string label)
        {
            var a11node = node.GetA11Node();
            if (a11node != null && a11node.TryGetChildPropertyValue(a11yHelp, out label))
            {
                return true;
            }
            label = null;
            return false;
        }
    }
}
