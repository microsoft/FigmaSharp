using FigmaSharp.Wpf;
using LocalFile.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LocalFile.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DocumentExample documentExample;
        public MainWindow()
        {
            InitializeComponent();

            var converters = FigmaSharp.AppContext.Current.GetFigmaConverters();
            var storyboard = new FigmaStoryboard(converters);

            var scrollViewWrapper = new ScrollViewWrapper(ContainerPanel);
            documentExample = new DocumentExample(scrollViewWrapper, storyboard);
        }
    }
}
