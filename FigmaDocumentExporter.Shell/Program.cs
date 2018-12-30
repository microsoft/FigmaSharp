using System;
using System.IO;
using FigmaSharp;
using System.Linq;

namespace FigmaDocumentExporter.Shell
{
    class Program
    {

        static void Main(string[] args)
        {
            const string outputFile = "downloaded.figma";

            FigmaSharp.AppContext.Current.SetAccessToken (Environment.GetEnvironmentVariable("TOKEN"));

            var outputDirectory = args[0];
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var fileId = args[1];

            var content = FigmaApiHelper.GetFigmaFileContent(fileId);

            var outputFilePath = Path.Combine(outputDirectory, outputFile);
            if (File.Exists(outputFilePath))
            {
                File.Delete(outputFilePath);
            }
            File.WriteAllText(outputFilePath, content);

            var figmaResponse = FigmaApiHelper.GetFigmaResponseFromContent(content);
            var mainNode = figmaResponse.document.children.FirstOrDefault();
            var figmaModelImages = mainNode.OfTypeImage().ToArray();
            var figmaImageIds = figmaModelImages.Select(s => s.id).ToArray();

            if (figmaImageIds.Length > 0)
            {
                var figmaImageResponse = FigmaApiHelper.GetFigmaImages(fileId, figmaImageIds);
                var urls = figmaImageResponse.images.Where (s => !string.IsNullOrEmpty (s.Value))
                    .Select(s => s.Value)
                    .ToArray();

                FileHelper.SaveFiles(outputDirectory, ".png", urls);
            }
            Console.WriteLine("Process finished successfull ({0} images processed)", figmaImageIds.Length);
        }
    }
}
