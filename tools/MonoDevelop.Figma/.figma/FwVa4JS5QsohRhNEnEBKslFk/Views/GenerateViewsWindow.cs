
using System;
using System.Collections.Generic;
using System.Data;
using AppKit;
using System.Linq;
using MonoDevelop.Ide;
using MonoDevelop.Projects;
using FigmaSharp;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;
using FigmaSharp.Services;
using MonoDevelop.Core;
using System.Threading.Tasks;
using System.IO;

namespace MonoDevelop.Figma.Packages
{
    class ValueData 
    {
		public readonly FigmaBundleViewBase View;
        public readonly IFigmaFileProvider fileProvider;

        public ValueData(FigmaBundleViewBase view,  IFigmaFileProvider fileProvider)
		{
			this.View = view;
            this.fileProvider = fileProvider;
        }

		public bool Value { get; set; }
        public string Description => System.IO.Path.GetFileNameWithoutExtension(View.PublicCsClassFilePath);
    }

    class OutlineViewDataSource : NSTableViewDataSource
    {
        readonly List<ValueData> data;

        public OutlineViewDataSource(List<ValueData> data)
        {
            this.data = data;
        }

        public override nint GetRowCount(NSTableView tableView)
        {
            return data.Count;
        }
    }

    class OutlineViewDelegate : NSTableViewDelegate
    {
        readonly List<ValueData> data;
        public const string Col1 = "col1";

        public OutlineViewDelegate(List<ValueData> data)
        {
            this.data = data;
        }

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            if (row >= data.Count)
                return null;

            var view = tableView.MakeView(tableColumn.Identifier, this);
            var dataItem = data[(int)row];

            if (tableColumn.Identifier == Col1) {
                CurrentView currentView = view as CurrentView;

                if (currentView == null) {
                    currentView = new CurrentView();
                    currentView.ViewButton.Activated += (s, e) => {
                        dataItem.Value = currentView.ViewButton.State == NSCellStateValue.On;
                    };
                }
                currentView.ViewButton.State = dataItem.Value ? NSCellStateValue.On : NSCellStateValue.Off;
                currentView.ViewButton.Title = dataItem.Description;
                return currentView;
            }
            return null;
        }
    }

    public partial class GenerateViewsWindow : AppKit.NSWindow
	{
        readonly List<ValueData> Data = new List<ValueData>();
        readonly Project project;

        async Task<List<ValueData>> FetchDataAsync ()
        { 
            var test = new List<ValueData>();
  
            var figmaFolder = project.GetFigmaFolder();
            foreach (var figmaProject in System.IO.Directory.GetDirectories(figmaFolder)) {
                var figmaBundle = FigmaBundle.FromDirectoryPath(figmaProject);
                var fileProvider = new FigmaLocalFileProvider(figmaBundle.ResourcesDirectoryPath);
                await fileProvider.LoadAsync(figmaBundle.DocumentFilePath);
                figmaBundle.LoadRemoteMainLayers(fileProvider);

                foreach (var view in figmaBundle.Views) {
                    test.Add (new ValueData(view, fileProvider));
                }
            }
            return test;
        }

        void CreateBundleView (FigmaBundleViewBase figmaBundleView, Project currentProject, IFigmaFileProvider fileProvider)
		{
            var bundle = figmaBundleView.Bundle;
		
			fileProvider.Load(bundle.DocumentFilePath);

			var converters = NativeControlsContext.Current.GetConverters();
			var codePropertyConverter = NativeControlsContext.Current.GetCodePropertyConverter();
			var codeRendererService = new NativeViewCodeService (fileProvider, converters, codePropertyConverter);

			figmaBundleView.Generate (outputDirectory, codeRendererService);

            var partialDesignerClassFilePath = Path.Combine(outputDirectory, figmaBundleView.PartialDesignerClassName);
            var publicCsClassFilePath = Path.Combine(outputDirectory, figmaBundleView.PublicCsClassName);

            var designerProjectFile = currentProject.AddFile(partialDesignerClassFilePath);
			var csProjectFile = currentProject.AddFile(publicCsClassFilePath);
			designerProjectFile.DependsOn = csProjectFile.FilePath;
		
        }

        readonly string outputDirectory;

        public GenerateViewsWindow (string outputDirectory, Project project)
		{
			InitializeComponent ();

            this.project = project;
            this.outputDirectory = outputDirectory;

            var tableColumn = new NSTableColumn(OutlineViewDelegate.Col1);
            tableColumn.Title = "Available Views";
            tableColumn.Width = MyTable.Frame.Width;
            MyTable.AddColumn(tableColumn);

            createButton.Activated += CreateButton_Activated;
			cancelButton.Activated += CancelButton_Activated;
        }

        public async Task LoadAsync ()
		{
            var data = await FetchDataAsync();
            Data.AddRange(data);
            MyTable.DataSource = new OutlineViewDataSource(data);
            MyTable.Delegate = new OutlineViewDelegate(data);
        }

		private void CancelButton_Activated(object sender, EventArgs e)
		{
            this.Close();
        }

		private async void CreateButton_Activated(object sender, EventArgs e)
        {
            var selectedData = Data.Where(s => s.Value);
			foreach (var item in selectedData) {
                CreateBundleView(item.View, project, item.fileProvider);
            }

            await IdeApp.ProjectOperations.SaveAsync(project);
            project.NeedsReload = true;
        }
	}
}
