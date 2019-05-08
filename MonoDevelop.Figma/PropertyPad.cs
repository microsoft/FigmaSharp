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

    class PropertyContentPad
    {
        public PropertyGrid Control;

        public PropertyContentPad()
        {
            Control = new PropertyGrid();
            Control.ShowAll();
        }

        public void Initialize()
        {

        }
    }

    class PropertyPad : PadContent, ICommandDelegator
    {
        static PropertyContentPad instance;
        public static PropertyContentPad Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PropertyContentPad();
                }
                return instance;
            }
        }

        IPadWindow container;

        internal object CommandRouteOrigin { get; set; }

        public PropertyPad()
        {
            Instance.Initialize();
        }

        protected override void Initialize(IPadWindow container)
        {
            base.Initialize(container);
            this.container = container;
        }

        internal IPadWindow PadWindow
        {
            get { return container; }
        }

        #region AbstractPadContent implementations

        public override Control Control
        {
            get { return instance.Control; }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion

        #region ICommandDelegatorRouter implementation

        object ICommandDelegator.GetDelegatedCommandTarget()
        {
            // Route the save command to the object for which we are inspecting the properties,
            // so pressing the Save shortcut when doing changes in the property pad will save
            // the document we are changing
            if (IdeApp.CommandService.CurrentCommand == IdeApp.CommandService.GetCommand(FileCommands.Save))
                return CommandRouteOrigin;
            else
                return null;
        }

        #endregion

        //Grid consumers must call this when they lose focus!
        public void BlankPad()
        {
            instance.Control.CurrentObject = null;
            CommandRouteOrigin = null;
        }

        void ClearToolbar()
        {
            if (container != null)
            {
                var toolbar = container.GetToolbar(DockPositionType.Top);
                foreach (var w in toolbar.Children)
                    toolbar.Remove(w);
            }
        }

        internal static void Initialize(FigmaDesignerSession session)
        {
            throw new NotImplementedException();
        }
    }
}