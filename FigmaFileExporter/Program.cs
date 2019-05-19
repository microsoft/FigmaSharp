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
            Console.WriteLine($"FIGMA FILE EXPORTER");
            Console.WriteLine($"===================");

            const string outputFile = "downloaded.figma";

            if (args.Length == 0)
            {
                Console.WriteLine($"Error. NO PARAMETERS DEFINED");
                Console.WriteLine($"");
                Console.WriteLine($"dotnet FigmaFileExporter.dll [document_id] {{output_directory}} {{figma_api}}");
                Console.WriteLine($"");
                return;
            }

            var fileId = args[0];

            string outputDirectory = null;
            if (args.Length > 1)
            {
                outputDirectory = args[1];
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }
            }

            if (outputDirectory == null)
            {
                outputDirectory = Directory.GetCurrentDirectory();
            }

            Console.WriteLine($"Default Directory: {outputDirectory}");

            string token = null;
            if (args.Length > 2)
            {
                token = args[2];
            }

            if (token == null)
            {
                token = Environment.GetEnvironmentVariable("FIGMA_TOKEN");
            }

            Console.WriteLine($"TOKEN: {outputDirectory}");

            FigmaSharp.AppContext.Current.SetAccessToken(token);

            Console.WriteLine($"Downloading content from file : {fileId}");
            var content = FigmaApiHelper.GetFigmaFileContent(fileId);

            var outputFilePath = Path.Combine(outputDirectory, outputFile);
            if (File.Exists(outputFilePath))
            {
                File.Delete(outputFilePath);
            }
            Console.WriteLine($"Writing content into '{outputFilePath}'");
            File.WriteAllText(outputFilePath, content);
            Console.WriteLine($"DONE.");

            var figmaResponse = FigmaApiHelper.GetFigmaResponseFromContent(content);
            var mainNode = figmaResponse.document.children.FirstOrDefault();
            var figmaModelImages = mainNode.OfTypeImage().ToArray();
            var figmaImageIds = figmaModelImages.Select(s => s.id).ToArray();

            if (figmaImageIds.Length > 0)
            {
                File.WriteAllText(outputFilePath, content);
                var figmaImageResponse = FigmaApiHelper.GetFigmaImages(fileId, figmaImageIds);
                FileHelper.SaveFiles(outputDirectory, ".png", figmaImageResponse.images);
            }
            Console.WriteLine("Process finished successfull ({0} images processed)", figmaImageIds.Length);
        }
    }
}
