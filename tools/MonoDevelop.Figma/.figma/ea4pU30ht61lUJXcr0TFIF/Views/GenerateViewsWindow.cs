
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppKit;
using FigmaSharp;
using FigmaSharp.Controls.Cocoa.Services;
using FigmaSharp.Services;
using MonoDevelop.Ide;
using MonoDevelop.Projects;

namespace MonoDevelop.Figma
{
    class ValueData
	{
		public readonly FigmaBundleViewBase View;
		public readonly INodeProvider fileProvider;

		public ValueData(FigmaBundleViewBase view, INodeProvider fileProvider)
		{
			this.View = view;
			this.fileProvider = fileProvider;
		}

		public bool Value { get; set; }
		public string Description => System.IO.Path.GetFileNameWithoutExtension(View.PublicCsClassFilePath);
		public string PackageName => View.Bundle.Manifest.DocumentTitle;
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
		public const string Col2 = "col2";

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

			if (tableColumn.Identifier == Col1)
			{
				var currentView = view as NSButton;

				if (currentView == null)
				{
					currentView = new NSButton();
					currentView.SetButtonType(NSButtonType.Switch);
					currentView.Activated += (s, e) =>
					{
						dataItem.Value = currentView.State == NSCellStateValue.On;
					};
				}
				currentView.State = dataItem.Value ? NSCellStateValue.On : NSCellStateValue.Off;
				currentView.Title = dataItem.Description;
				return currentView;
			}
			if (tableColumn.Identifier == Col2)
			{
				var currentView = view as NSTextField;
				if (currentView == null)
				{
					currentView = ViewHelpers.CreateLabel(dataItem.PackageName, NSFont.SystemFontOfSize (13));
				}
				return currentView;
			}
			return null;
		}
	}

	public partial class GenerateViewsWindow : AppKit.NSWindow
	{
		readonly List<ValueData> Data = new List<ValueData>();
		readonly Project project;

		async Task<List<ValueData>> FetchDataAsync()
		{
			var test = new List<ValueData>();
			//we iterate over all the bundles

			foreach (var figmaBundle in project.GetFigmaPackages())
			{
				var fileProvider = new ControlFileNodeProvider(figmaBundle.ResourcesDirectoryPath);
				await fileProvider.LoadAsync(figmaBundle.DocumentFilePath);

				var mainFigmaNodes = fileProvider.GetMainGeneratedLayers();
				foreach (var figmaNode in mainFigmaNodes)
				{
					var document = figmaBundle.GetFigmaFileView(figmaNode);
					test.Add(new ValueData(document, fileProvider));
				}
			}
			return test;
		}

		async Task<ProjectFile> CreateBundleView(FigmaBundleViewBase figmaBundleView, Project currentProject, INodeProvider fileProvider, bool translateStrings)
		{
			var bundle = figmaBundleView.Bundle;

			await fileProvider.LoadAsync(bundle.DocumentFilePath);

			var converters = FigmaControlsContext.Current.GetConverters();
			var codePropertyConverter = FigmaControlsContext.Current.GetCodePropertyConverter();
			var codeRendererService = new NativeViewCodeService(fileProvider, converters, codePropertyConverter);

			figmaBundleView.Generate(outputDirectory, codeRendererService, namesSpace: bundle.Namespace, translateStrings: translateStrings);

			var partialDesignerClassFilePath = Path.Combine(outputDirectory, figmaBundleView.PartialDesignerClassName);
			var publicCsClassFilePath = Path.Combine(outputDirectory, figmaBundleView.PublicCsClassName);

			var designerProjectFile = currentProject.AddFile(partialDesignerClassFilePath);
			var csProjectFile = currentProject.AddFile(publicCsClassFilePath);
			designerProjectFile.DependsOn = csProjectFile.FilePath;
			designerProjectFile.Metadata.SetValue(FigmaFile.FigmaPackageId, bundle.FileId);

			if (!figmaBundleView.FigmaNode.TryGetNodeCustomName(out string customName))
				customName = figmaBundleView.FigmaNode.name;

			designerProjectFile.Metadata.SetValue(FigmaFile.FigmaNodeCustomName, customName);
			return csProjectFile;
		}

		readonly string outputDirectory;
		readonly AppKit.NSTableView fileTableView;


		public GenerateViewsWindow(string outputDirectory, Project project)
		{
			InitializeComponent();

			this.project = project;
			this.outputDirectory = outputDirectory;

			fileTableView = new NSTableView();
			tableScrollView.DocumentView = fileTableView;

			const int packageWidth = 200;
			var tablePackageColumn = new NSTableColumn(OutlineViewDelegate.Col2);
			tablePackageColumn.Title = "Package";
			tablePackageColumn.Width = packageWidth;

			var tableColumn = new NSTableColumn(OutlineViewDelegate.Col1);
			tableColumn.Title = "Available Views";
			tableColumn.Width = fileTableView.Frame.Width - packageWidth;
			fileTableView.AddColumn(tableColumn);

			fileTableView.AddColumn(tablePackageColumn);

			bundleButton.Activated += CreateButton_Activated;
			cancelButton.Activated += CancelButton_Activated;
		}

		public async Task LoadAsync()
		{
			var data = await FetchDataAsync();
			Data.AddRange(data);
			fileTableView.DataSource = new OutlineViewDataSource(data);
			fileTableView.Delegate = new OutlineViewDelegate(data);
		}

		private void CancelButton_Activated(object sender, EventArgs e)
		{
			this.Close();
		}

		private async void CreateButton_Activated(object sender, EventArgs e)
		{
			IdeApp.Workbench.StatusBar.AutoPulse = true;
			IdeApp.Workbench.StatusBar.BeginProgress($"Generating views…");

			var selectedData = Data.Where(s => s.Value);
			foreach (var item in selectedData)
			{
				IdeApp.Workbench.StatusBar.ShowMessage($"Generating {item.Description}…");
				await CreateBundleView(item.View, project, item.fileProvider, translationsCheckbox.State == NSCellStateValue.On);
			}

			await IdeApp.ProjectOperations.SaveAsync(project);
			project.NeedsReload = true;

			IdeApp.Workbench.StatusBar.EndProgress();
			IdeApp.Workbench.StatusBar.AutoPulse = false;

			this.Close();
		}
	}
}
