using System;
using System.IO;
using FigmaSharp;
using System.Linq;
using System.Reflection;

namespace FigmaDocumentExporter.Shell
{
    class Program
    {

        static void Main(string[] args)
        {
            if (args.Length != 2 && args.Length != 3)
            {
                Console.WriteLine("Error: Invalid number of arguments");
                Console.WriteLine("dotnet FigmaFileExporter.dll {FIGMA_TOKEN} {FILE_ID} {OUTPUT_DIRECTORY:OPTIONAL}");
                return;
            }
            
            const string outputFile = "downloaded.figma";

            var token = args[0];
            var fileId = args[1];

            var currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            if (args.Length == 2)
            {
                Console.WriteLine($"Not output directory selected. Current Path: {currentDirectory}");
            }
            
            var outputDirectory = args.Length == 2 ? currentDirectory : args[2];
            
            FigmaSharp.AppContext.Current.SetAccessToken (token);

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

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
                FileHelper.SaveFiles(outputDirectory, ".png", figmaImageResponse.images);
            }
            Console.WriteLine("Process finished successfull ({0} images processed)", figmaImageIds.Length);
        }
    }
}
