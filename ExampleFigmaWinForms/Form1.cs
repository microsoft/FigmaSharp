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
        public Form1()
        {
            InitializeComponent ();
            var panel = new GroupBox() { Text = "ddd", Left = 10, Top = 10 };
            panel.Width = this.Width - 40;
            panel.Height = this.Height - 40;
            this.Controls.Add(panel);
            panel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            var label = new Label() { Text = "dsaddsasdsdadsa", Left = 400, Top = 320 };
            panel.Controls.Add(label);
            System.Windows.Forms.ScrollBars eee = new ScrollBars();
            panel.Invalidate();
            this.AutoScroll = true;
        }
    }
}
