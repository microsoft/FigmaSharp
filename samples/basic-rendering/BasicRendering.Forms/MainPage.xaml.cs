using System.ComponentModel;
using ExampleFigma;
using FigmaSharp;
using Xamarin.Forms;

namespace BasicRendering.Forms
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        const string FigmaFileName = "INSERT YOUR FIGMA FILE IDENTIFIER HERE";

        public MainPage()
        {
            InitializeComponent();

            var scrollViewWrapper = new ScrollViewWrapper(ContainerPanel);
            var manager = new ExampleViewManager(scrollViewWrapper, FigmaFileName);
            manager.Initialize();
        }
    }
}