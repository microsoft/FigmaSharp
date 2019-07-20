using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace LocalFile.Forms
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : FigmaAssemblyResourceContentPage
    {
        public MainPage()
        {
            InitializeFigmaComponent();

            LoadDocument ("MainPage.figma");
            RenderByName ("Log-In Page");
        }
    }
}
