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

namespace BasicRendering.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //var token = Environment.GetEnvironmentVariable("TOKEN");
            //var token = "158001-91ae4d14-ccd3-40a6-b8e2-9c13194a94b3";
            var token = ConfigurationManager.AppSettings.Get("FIGMATOKEN");
            Console.WriteLine(token);
            if (string.IsNullOrEmpty(token))
            {
                token = Settings.Default.TOKEN;
            }
            FigmaApplication.Init(token);
        }
    }
}
