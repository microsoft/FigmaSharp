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
		[ManifestDescription ("File Id")]
		public string FileId { get; set; }

		[ManifestDescription ("Document Title")]
		public string DocumentTitle { get; set; }

		[ManifestDescription("Document Version")]
		public string DocumentVersion { get; set; }

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

			builder.AppendLine ($"This file was auto-generated using");
			builder.AppendLine ($"FigmaSharp {ApiVersion} and Figma API {RemoteApiVersion} on {date} at {time}");
			builder.AppendLine ();
			builder.AppendLine ($"Document title:   {DocumentTitle}");
			builder.AppendLine ($"Document version: {DocumentVersion}");
			builder.AppendLine ($"Document URL:     {FileId}");
			builder.AppendLine ();
			builder.AppendLine ($"Changes to this file may cause incorrect behavior");
			builder.AppendLine ($"and will be lost if the code is regenerated.");
		}

		public static FigmaManifest FromFilePath (string filePath)
		{
			return JsonConvert.DeserializeObject<FigmaManifest> (File.ReadAllText (filePath));
		}

		public void Save (string filePath)
		{
			File.WriteAllText (filePath, JsonConvert.SerializeObject (this, Formatting.Indented));
		}
	}
}
