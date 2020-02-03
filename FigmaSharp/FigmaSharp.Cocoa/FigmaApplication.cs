namespace FigmaSharp.Cocoa
{
	public class FigmaApplication
	{
		public static void Init (string token = null)
		{
			//Figma initialization
			var applicationDelegate = new FigmaDelegate ();
			if (string.IsNullOrEmpty (token))
				AppContext.Current.Configuration (applicationDelegate);
			else
				AppContext.Current.Configuration (applicationDelegate, token);
		}
	}
}
