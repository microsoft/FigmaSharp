using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExampleFigmaWinForms.Properties;
using FigmaSharp;
using FigmaSharp.WinForms;

namespace ExampleFigmaWinForms
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var token = Environment.GetEnvironmentVariable ("TOKEN");
            if (string.IsNullOrEmpty(token)) {
                token = Settings.Default.TOKEN;
            }
            FigmaApplication.Init(token);
            Application.EnableVisualStyles ();
            Application.SetCompatibleTextRenderingDefault (false);
            Application.Run (new Form1 ());
        }
    }
}
