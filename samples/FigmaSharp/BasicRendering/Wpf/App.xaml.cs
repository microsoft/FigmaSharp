using System.Configuration;
using System.Collections.Specialized;

using BasicRendering.Wpf.Properties;
using FigmaSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using Application = System.Windows.Application;

namespace BasicRendering.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //var configForm = new ConfigForm();
            //configForm.ShowDialog();
            //// Determine if the OK button was clicked on the dialog box.
            //if (configForm.DialogResult == DialogResult.OK)
            //{
            //    // Display a message box indicating that the OK button was clicked.
            //    MessageBox.Show("The OK button on the form was clicked.");
            //    // Optional: Call the Dispose method when you are finished with the dialog box.
            //    configForm.Dispose();
            //}
            //else
            //{
            //    // Display a message box indicating that the Cancel button was clicked.
            //    MessageBox.Show("The Cancel button on the form was clicked.");
            //    // Optional: Call the Dispose method when you are finished with the dialog box.
            //    Console.WriteLine(configForm.textBox1.Text);
            //    configForm.Dispose();
            //}

            var token = ConfigurationManager.AppSettings.Get("FIGMATOKEN");
            if (string.IsNullOrEmpty(token))
            {
                token = Settings.Default.TOKEN;
            }
            FigmaApplication.Init(token);
        }
    }
}
