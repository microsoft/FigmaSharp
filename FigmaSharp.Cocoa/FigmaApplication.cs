namespace FigmaSharp.Cocoa
{
    public class FigmaApplication
    {
        private FigmaApplication()
        {

        }

        public static void Init(string token)
        {
            //Figma initialization
            var applicationDelegate = new FigmaDelegate();
            AppContext.Current.Configuration(applicationDelegate, token);
        }

		public static void Init()
		{
			//Figma initialization
			var applicationDelegate = new FigmaDelegate();
			AppContext.Current.Configuration(applicationDelegate);
		}
	}
}
