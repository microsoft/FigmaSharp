using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FigmaSharp;

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
            FigmaApplication.Init(Environment.GetEnvironmentVariable("TOKEN"));
            Application.EnableVisualStyles ();
            Application.SetCompatibleTextRenderingDefault (false);
            Application.Run (new Form1 ());
        }
    }
}
