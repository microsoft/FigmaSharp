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
        public MainPage()
        {
            InitializeComponent();

            var scrollViewWrapper = new ScrollViewWrapper(ContainerPanel);
            var manager = new ExampleViewManager(scrollViewWrapper);
            manager.Initialize();
        }
    }
}