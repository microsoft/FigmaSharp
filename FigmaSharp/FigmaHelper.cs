/* 
 * FigmaQueryHelper.cs - Helper methods to query Figma API
 * 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *
 * Copyright (C) 2018 Microsoft, Corp
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FigmaSharp
{
    public class FigmaImageResponse
    {
        public string err { get; set; }
        public Dictionary<string, string> images { get; set; }
    }

    public class FigmaImageQuery
    {
        public FigmaImageQuery(string document, string[] ids) : this (FigmaEnvirontment.Token, document, ids)
        {
           
        }

        public FigmaImageQuery(string personalAccessToken, string document, string[] ids)
        {
            Document = document;
            Ids = ids;
            PersonalAccessToken = personalAccessToken;
        }

        public string Document { get; set; }
        public string[] Ids { get; set; }
        public string PersonalAccessToken { get; set; }
        public string Scale { get; set; }
        public string Format { get; set; }
        public string Version { get; set; }
    }

    public static class FigmaHelper
    {
        public static string GetManifestResource (Assembly assembly, string resource)
        {
            if (assembly == null)
            {
                assembly = Assembly.GetEntryAssembly();
            }
            try
            {
                //TODO: not safe
                var fullResourceName = string.Format("{0}.{1}", assembly.GetName().Name, resource);
                var resources = assembly.GetManifestResourceNames();
                using (var stream = assembly.GetManifestResourceStream(fullResourceName))
                {
                    using (TextReader tr = new StreamReader(stream))
                    {
                        return tr.ReadToEnd();
                    };
                }
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public static IFigmaDocumentContainer GetFigmaDialogFromUrlFile (string urlFile, string viewName = null, string nodeName = null)
		{
			var figmaContent = GetFigmaFileContent (urlFile, FigmaEnvirontment.Token);
			return GetFigmaDialogFromContent (figmaContent, viewName, nodeName);
		}

		public static IFigmaDocumentContainer GetFigmaDialogFromFilePath (string file, string viewName = null, string nodeName = null)
		{
			var figmaContent = File.ReadAllText (file);
			return GetFigmaDialogFromContent (figmaContent, viewName, nodeName);
		}

		public static IFigmaDocumentContainer GetFigmaDialogFromContent (string figmaContent, string viewName = null, string nodeName = null)
		{
			var figmaResponse = JsonConvert.DeserializeObject<FigmaResponse> (figmaContent, new FigmaResponseConverter ());
			return GetFigmaDialogFromResponse (figmaResponse, viewName, nodeName);
		}

		static IFigmaDocumentContainer GetFigmaDialogFromResponse (FigmaResponse figmaResponse, string viewName = null, string nodeName = null)
		{
			var resultNodes = new List<FigmaNode> ();

			FigmaNode[] figmaNodes = figmaResponse.document.children;

			if (!string.IsNullOrEmpty (nodeName)) {
				figmaNodes.Recursively (nodeName, resultNodes);
				var figmaFrame = (FigmaFrameEntity)resultNodes.FirstOrDefault ();
				if (figmaFrame == null) {
					return null;
				}
				figmaNodes = figmaFrame.children;
				resultNodes.Clear ();
			}

			if (string.IsNullOrEmpty (viewName)) {
				return figmaNodes.FirstOrDefault () as IFigmaDocumentContainer;
			}

			figmaNodes.Recursively (viewName, resultNodes);
			if (resultNodes.Count == 0) {
				return null;
			}

			return resultNodes.FirstOrDefault () as IFigmaDocumentContainer;
		}


        //TODO: Change to async multithread
        public static async Task SaveFilesAsync(string destinationDirectory, string format, params string[] remotefile)
        {
            if (!Directory.Exists(destinationDirectory))
            {
                throw new DirectoryNotFoundException(destinationDirectory);
            }
            List<Task> downloads = new List<Task>();
            foreach (var file in remotefile)
            {
                var task = Task.Run(() =>
                {
                    var fileName = string.Concat (Path.GetFileName (file), format);
                    var fullPath = Path.Combine(destinationDirectory, fileName);

                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }

                    try
                    {
                        using (WebClient client = new WebClient())
                        {
                            client.DownloadFile(new Uri(file), fullPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                });
                downloads.Add(task);
            }
            await Task.WhenAll(downloads);
        }

        public static FigmaImageResponse GetFigmaImages(string fileId, string[] ids)
        {
            var query = new FigmaImageQuery(FigmaEnvirontment.Token, fileId, ids);
            return FigmaHelper.GetFigmaImage(query);
        }

        public static FigmaImageResponse GetFigmaImage (FigmaImageQuery figmaQuery)
        {
            var id = string.Join(",", figmaQuery.Ids);

            var stringBuilder = new StringBuilder($"https://api.figma.com/v1/images/{figmaQuery.Document}?ids={id}");
            if (figmaQuery.Scale != null)
            {
                stringBuilder.Append(string.Format("&scale={0}", figmaQuery.Scale));
            }
            if (figmaQuery.Format != null)
            {
                stringBuilder.Append(string.Format("&format={0}", figmaQuery.Format));
            }
            if (figmaQuery.Version != null)
            {
                stringBuilder.Append(string.Format("&version={0}", figmaQuery.Version));
            }

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(stringBuilder.ToString ());
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers["x-figma-token"] = figmaQuery.PersonalAccessToken;

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return JsonConvert.DeserializeObject<FigmaImageResponse>(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

		public static string GetFigmaFileContent (string file)
		{
			return GetFigmaFileContent (file, FigmaEnvirontment.Token);
		}

		public static string GetFigmaFileContent (string file, string personal_access_token)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create($"https://api.figma.com/v1/files/{file}");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers["x-figma-token"] = personal_access_token;

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                   return streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }
    }
}
