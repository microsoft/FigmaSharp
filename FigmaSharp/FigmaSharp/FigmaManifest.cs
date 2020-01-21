using System;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using FigmaSharp.Models;
using System.Threading.Tasks;

namespace FigmaSharp
{
	[System.AttributeUsage (System.AttributeTargets.Property)]
	public class ManifestDescription : System.Attribute
	{
		public string Description { get; }
		public ManifestDescription (string description)
		{
			this.Description = description;
		}
	}

	public class FigmaBundle
	{
		public FigmaManifest Manifest { get; set; }

		public string DirectoryPath { get; private set; }

		public string Name => DirectoryPath == null ? null : Path.GetFileName (DirectoryPath);

		internal const string FigmaBundleDirectoryExtension = ".figmabundle";
		//const string FigmaBundlesDirectoryName = ".bundles";
		internal const string FigmaDirectoryName = ".figma";
		internal const string ManifestFileName = "manifest.json";
		internal const string DocumentFileName = "document.figma";
		internal const string ResourcesDirectoryName = "Resources";
		internal const string ImageFormat = ".png";

		public void Load (string bundleDirectoryPath)
		{
			if (!Directory.Exists (bundleDirectoryPath)) {
				throw new DirectoryNotFoundException ("directory doesn't exists");
			}

			var manifestFullPath = Path.Combine (bundleDirectoryPath, ManifestFileName);
			if (!File.Exists (manifestFullPath)) {
				throw new FileNotFoundException ("manifest doesn't exists");
			}

			DirectoryPath = bundleDirectoryPath;

			try {
				Manifest = FigmaManifest.FromFilePath (manifestFullPath);
			} catch (Exception ex) {
				throw new FileLoadException ("error reading manifest file", ex);
			}
		}

		public void Save ()
		{
			if (DirectoryPath == null) {
				return;
			}

			if (!Directory.Exists (DirectoryPath)) {
				Directory.CreateDirectory (DirectoryPath);
			}

			if (Manifest != null) {
				Manifest.Save (Path.Combine (DirectoryPath, ManifestFileName));
			}
		}

		public static FigmaBundle Create (string fileId, string directoryPath)
		{
			var bundle = new FigmaBundle () {
				DirectoryPath = directoryPath
			};
			bundle.Manifest = new FigmaManifest () {
				ApiVersion = AppContext.Current.Version,
				Date = DateTime.Now,
				DocumentUrl = fileId
			};

			return bundle;
		}

		public static FigmaBundle FromDirectoryPath (string fullPath)
		{
			try {
				var bundle = new FigmaBundle ();
				bundle.Load (fullPath);
				return bundle;
			} catch (Exception ex) {
				Console.WriteLine (ex);
				//not a bundle
				return null;
			}
		}

		public string FileId => Manifest.DocumentUrl;

		public string DocumentFilePath => Path.Combine (DirectoryPath, DocumentFileName);

		internal static FigmaResponse GenerateDocumentOutputFile (string fileId, string directoryPath)
		{
			var documentFilePath = Path.Combine (directoryPath, DocumentFileName);
			if (File.Exists (documentFilePath))
				File.Delete (documentFilePath);
			//generate also Document.figma
			var content = FigmaApiHelper.GetFigmaFileContent (fileId);
		
			File.WriteAllText (documentFilePath, content);
			var figmaResponse = FigmaApiHelper.GetFigmaResponseFromContent (content);
			return figmaResponse;
		}

		internal static void GenerateOutputResourceFiles (string fileId, FigmaResponse figmaResponse, string resourcesDirectoryPath)
		{
			var mainNode = figmaResponse.document.children.FirstOrDefault ();

			var figmaImageIds = mainNode.OfTypeImage ()
				.Select (s => s.id)
				.ToArray ();

			if (figmaImageIds.Length > 0) {
				
				if (!Directory.Exists (resourcesDirectoryPath)) {
					Directory.CreateDirectory (resourcesDirectoryPath);
				}

				var figmaImageResponse = FigmaApiHelper.GetFigmaImages (fileId, figmaImageIds);
				FileHelper.SaveFiles (resourcesDirectoryPath, ImageFormat, figmaImageResponse.images);
			}
		}

		internal void GenerateLocalDocument (bool includeImages)
		{
			if (string.IsNullOrEmpty (FileId)) {
				throw new InvalidOperationException ("id not set");
			}

			//get image resources
			var figmaResponse = GenerateDocumentOutputFile (Manifest.DocumentUrl, DirectoryPath);

			if (!includeImages)
				return;

			var resourcesDirectoryPath = Path.Combine (DirectoryPath, ResourcesDirectoryName);
			GenerateOutputResourceFiles (FileId, figmaResponse, resourcesDirectoryPath);
		}
	}

	public class FigmaManifest
	{
		[ManifestDescription ("Document Url")]
		public string DocumentUrl { get; set; }

		[ManifestDescription ("Document Version")]
		public float DocumentVersion { get; set; }

		[ManifestDescription ("Conversion Date")]
		public DateTime Date { get; set; }

		[ManifestDescription ("Remote Api Version")]
		public string RemoteApiVersion { get; set; }

		[ManifestDescription ("FigmaSharp Api Version")]
		public string ApiVersion { get; set; }

		ManifestDescription GetManifestDescription (string propertyName)
		{
			var attribute = this.GetType ().GetProperty (propertyName).GetCustomAttributes (true).OfType<ManifestDescription> ().FirstOrDefault ();
			return attribute;
		}

		public void ToComment (StringBuilder builder, bool singleComment = false)
		{
			string comment = singleComment ? "//" : "*";

			if (!string.IsNullOrEmpty (DocumentUrl))
				builder.AppendLine ($"{comment} {GetManifestDescription (nameof (DocumentUrl)).Description}: {DocumentUrl}");

			if (!string.IsNullOrEmpty (ApiVersion))
				builder.AppendLine ($"{comment} {GetManifestDescription (nameof (DocumentVersion)).Description}: {DocumentVersion}");

			if (Date != default)
				builder.AppendLine ($"{comment} {GetManifestDescription (nameof (Date)).Description}: {Date.ToString ("MM/dd/yyyy HH:mm:ss")}");

			if (!string.IsNullOrEmpty (RemoteApiVersion))
				builder.AppendLine ($"{comment} {GetManifestDescription (nameof (RemoteApiVersion)).Description}: {RemoteApiVersion}");

			builder.AppendLine ($"{comment} {GetManifestDescription (nameof (ApiVersion)).Description}: {ApiVersion}");
		}

		public static FigmaManifest FromFilePath (string filePath)
		{
			return JsonConvert.DeserializeObject<FigmaManifest> (File.ReadAllText (filePath));
		}

		public void Save (string filePath)
		{
			File.WriteAllText (filePath, JsonConvert.SerializeObject (this));
		}
	}
}
