using FigmaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FigmaDocumentExporter.WinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent ();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DownloadAction (null);
        }

        const string outputFile = "downloaded.figma";
        async  void DownloadAction(object sender)
        {
            var outputDirectory = outputPathTextField.Text;
            if (!Directory.Exists (outputDirectory)) {
                Directory.CreateDirectory (outputDirectory);
            }

            var fileId = documentTextField.Text;

            var content = FigmaApiHelper.GetFigmaFileContent (fileId);

            var outputFilePath = Path.Combine (outputDirectory, outputFile);
            if (File.Exists (outputFilePath)) {
                File.Delete (outputFilePath);
            }
            File.WriteAllText (outputFilePath, content);

            var mainNode = FigmaApiHelper.GetFigmaDialogFromContent (content) as FigmaNode;
            var figmaModelImages = mainNode.OfTypeImage ().ToArray ();
            var figmaImageIds = figmaModelImages.Select (s => s.ID).ToArray ();
          
            if (figmaImageIds.Length > 0) {
                var figmaResponse = FigmaApiHelper.GetFigmaImages (fileId, figmaImageIds);
                var urls = figmaResponse.images.Select (s => s.Value).ToArray ();
                await FileHelper.SaveFilesAsync (outputDirectory, ".png", urls);

                MessageBox.Show ("Process finished correctly");
            } else {
                MessageBox.Show ("No images detected in the Figma File");
            }
        }
    }
}
