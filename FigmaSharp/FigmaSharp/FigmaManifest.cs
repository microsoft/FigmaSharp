using System;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
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

		public void ToComment (StringBuilder builder)
		{
			string date = Date.ToString("yyyy-MM-dd HH:mm");
			string time = Date.ToString("HH:mm");

			string header =
				$"* This file was auto-generated using\n" +
				$"* FigmaSharp {ApiVersion} and Figma API {RemoteApiVersion} on {date} at {time}\n" +
				$"*\n" +
				$"* Document title:   macOS Components\n" + // TODO: Document title in manifest
				$"* Document version: {DocumentVersion}\n" +
				$"* Document URL:     {DocumentUrl}\n" +
				$"*\n" +
				$"* Changes to this file may cause incorrect behavior\n" +
				$"* and will be lost if the code is regenerated.";

			builder.AppendLine(header);
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
