using ExampleFigma;
using ExampleFigmaWinForms.Properties;
using FigmaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExampleFigmaWinForms
{
    public partial class Form1 : Form
    {
        ExampleViewManager manager;
        public Form1()
        {
            InitializeComponent ();
            var fileName = Environment.GetEnvironmentVariable ("FILE");
            if (string.IsNullOrEmpty (fileName)) {
                fileName = Settings.Default.FILE;
            }
            var scrollViewWrapper = new ScrollViewWrapper (ContainerPanel);

            manager = new ExampleViewManager (scrollViewWrapper, fileName);
            manager.Initialize ();
        }

        private void ContainerPanel_Scroll(object sender, ScrollEventArgs e)
        {
            ContainerPanel.Refresh ();
            ContainerPanel.Invalidate ();
        }
    }
}
