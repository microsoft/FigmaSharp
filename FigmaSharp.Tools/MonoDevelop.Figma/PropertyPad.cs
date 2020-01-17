using System;
using System.Collections;
using FigmaSharp;
using FigmaSharp.Designer;
using MonoDevelop.Components;
using MonoDevelop.Components.Commands;
using MonoDevelop.Components.Docking;
using MonoDevelop.Components.PropertyGrid;
using MonoDevelop.Core.Serialization;
using MonoDevelop.DesignerSupport;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Commands;
using MonoDevelop.Ide.Gui;
using FigmaSharp.Models;

namespace MonoDevelop.Figma
{
    public class FigmaPropertyProvider : IPropertyProvider
    {
        public bool SupportsObject(object obj)
        {
            return obj is FigmaNode;
        }

        public object CreateProvider(object obj)
        {
            return obj;
        }
    }
}