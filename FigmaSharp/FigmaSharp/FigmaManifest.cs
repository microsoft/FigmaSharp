using System;
using System.Text;
using System.Linq;

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
		public string ApiVersion {
			get {
				return System.Diagnostics.FileVersionInfo.GetVersionInfo (this.GetType ().Assembly.Location).FileVersion;
			}
		}

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
