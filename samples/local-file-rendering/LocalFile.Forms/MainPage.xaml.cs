using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FigmaSharp.Forms;
using LocalFile.Shared;
using Xamarin.Forms;

namespace LocalFile.Forms
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        DocumentExample documentExample;

        public MainPage()
        {
            InitializeComponent();

            var scrollViewWrapper = new ScrollViewWrapper(ContainerPanel);
            var converters = FigmaSharp.AppContext.Current.GetFigmaConverters();
            var storyboard = new FigmaStoryboard(converters);
            documentExample = new DocumentExample(scrollViewWrapper, storyboard);
        }
    }
}
