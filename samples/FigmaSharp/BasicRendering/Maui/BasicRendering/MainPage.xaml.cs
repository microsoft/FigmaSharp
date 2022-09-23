using System.ComponentModel;
using Xamarin.Forms;

namespace BasicRendering
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : RemoteContentPage
    {
        public MainPage()
        {
            InitializeFigmaComponent();
            LoadDocument("ZIKtloPwUhTRodLY6Pb0Ek");
            RenderByName("MainScreen");
        }
    }
}
