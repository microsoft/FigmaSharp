using System;
using System.Text;
using MapKit;

namespace FigmaSharp.NativeControls.Cocoa
{
	public class MapViewConverter : CustomViewConverter
	{
		public override bool CanConvert (FigmaNode currentNode)
		{
			return currentNode.name == "mapView" && currentNode is IFigmaDocumentContainer;
		}

		public override IViewWrapper ConvertTo (FigmaNode currentNode, ProcessedNode parent)
		{
			var mapview = new MapKit.MKMapView ();
			mapview.Configure (currentNode);
			return new ViewWrapper (mapview);
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
