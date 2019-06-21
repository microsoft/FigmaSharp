using System.ComponentModel;
using ExampleFigma;
using FigmaSharp;
using FigmaSharp.Forms;
using Xamarin.Forms;

namespace BasicRendering.Forms
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        class ContentView
        {

        }

        public MainPage()
        {
            InitializeComponent();

            var content = new ViewWrapper (new AbsoluteLayout ());
            var scrollViewWrapper = new ScrollViewWrapper(ContainerPanel);
            var manager = new ExampleViewManager(scrollViewWrapper, content);
            manager.Initialize();
        }
    }
}