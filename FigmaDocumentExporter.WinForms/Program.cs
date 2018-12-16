using FigmaDocumentExporter.WinForms.Properties;
using FigmaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FigmaDocumentExporter.WinForms
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            FigmaApplication.Init (Settings.Default.TOKEN);
            Application.EnableVisualStyles ();
            Application.SetCompatibleTextRenderingDefault (false);
            Application.Run (new Form1 ());
        }
    }
}
