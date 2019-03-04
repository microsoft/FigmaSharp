using System;
using System.Text;
using MapKit;

namespace FigmaSharp.NativeControls
{
	public class MapViewConverter : CustomViewConverter
	{
		public override bool CanConvert (FigmaNode currentNode)
		{
			return ContainsType (currentNode, "mapView");
		}

		public override IViewWrapper ConvertTo (FigmaNode currentNode, ProcessedNode parent)
		{
			var view = new MapKit.MKMapView ();
			view.Configure (currentNode);
			var keyValues = GetKeyValues (currentNode);
			foreach (var key in keyValues) {
				if (key.Key == "type") {
					continue;
				} 

			}
			return new ViewWrapper (view);
		}

		public override string ConvertToCode (FigmaNode currentNode, ProcessedNode parent)
		{
			StringBuilder builder = new StringBuilder ();
			var name = "mapView";
			builder.AppendLine ($"var {name} = new {nameof (MapKit)}.{nameof (MKMapView)}();");
			builder.Configure (name, currentNode);
			return builder.ToString ();
		}
	}
}
