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

using FigmaSharp.Models;

namespace FigmaSharp
{
    public static class FigmaApiHelper
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
                    if (stream == null)
                    {
                        Console.WriteLine("Resource '{0}' not found in assembly '{1}'", resource, assembly.FullName);
                        return null;
                    }
                    using (TextReader tr = new StreamReader(stream))
                    {
                        return tr.ReadToEnd();
                    };
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Cannot read resource '{0}' in assembly '{1}'", resource, ex);
                return null;
            }
        }


        public static FigmaResponse GetFigmaDialogFromUrlFile (string urlFile)
       {
         var figmaContent = GetFigmaFileContent (urlFile, AppContext.Current.Token);
         return GetFigmaResponseFromContent(figmaContent);
       }


       public static FigmaResponse GetFigmaDialogFromFilePath (string file)
       {
         var figmaContent = File.ReadAllText (file);
         return GetFigmaResponseFromContent(figmaContent);
       }


       public static FigmaResponse GetFigmaResponseFromContent (string figmaContent)
       {
         return JsonConvert.DeserializeObject<FigmaResponse> (figmaContent, new FigmaResponseConverter ());
       }


        public static void SetFigmaResponseFromContent(FigmaResponse figmaResponse, string filePath)
        {
            var data = JsonConvert.SerializeObject (figmaResponse);
            if (File.Exists (filePath)) {
                File.Delete(filePath);
            }
            File.WriteAllText(filePath, data);
        }

        public static FigmaImageResponse GetFigmaImages(string fileId, IEnumerable<string> ids, ImageQueryFormat format = ImageQueryFormat.png, float scale = 2)
        {
            var query = new FigmaImageQuery(AppContext.Current.Token, fileId, ids);
			query.Scale = scale;
			query.Format = format;
			return GetFigmaImage(query);
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
            return GetFigmaFileContent (file, AppContext.Current.Token);
       }

        public static string GetFigmaFileContent (string file, string personal_access_token)
        {
			return GetUrlContent($"https://api.figma.com/v1/files/{file}", personal_access_token);
        }

		public static string GetUrlContent(string url)
		{
			try
			{
				var wc = new System.Net.WebClient();
				byte[] raw = wc.DownloadData(url);

				string webData = System.Text.Encoding.UTF8.GetString(raw);
				return webData;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			return string.Empty;
		}

		public static string GetUrlContent(string url, string personal_access_token)
		{
			var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "GET";
			httpWebRequest.Headers["x-figma-token"] = personal_access_token;

			var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
			{
				return streamReader.ReadToEnd();
			}
		}
	}
}
