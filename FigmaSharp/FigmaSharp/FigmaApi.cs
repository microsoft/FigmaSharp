/* 
 * FigmaApi.cs 
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

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;

using FigmaSharp.Models;

using Newtonsoft.Json;

namespace FigmaSharp
{
	public class FigmaApi
	{
		internal string Token { get; set; }

		//TODO: right now there is no way to detect changes in API
		public Version Version { get; } = new Version (1, 0, 1);

		#region Urls

		string GetFigmaFileUrl (string fileId) => string.Format ("https://api.figma.com/v1/files/{0}", fileId);
		string GetFigmaImageUrl (string fileId, params IFigmaDownloadImageNode[] imageIds) => string.Format ("https://api.figma.com/v1/images/{0}?ids={1}", fileId, string.Join (",", imageIds.Select(s => s.ResourceId).ToArray ()));
		string GetFigmaFileVersionsUrl (string fileId) => string.Format ("{0}/versions", GetFigmaFileUrl (fileId));

		#endregion

		public string GetContentFile (FigmaFileQuery figmaQuery)
		{
			var result = GetContentUrl (figmaQuery,
				(e) => {
					var queryUrl = GetFigmaFileUrl (figmaQuery.FileId);
					if (e.Version != null) {
						queryUrl += string.Format ("?version={0}", figmaQuery.Version);
					}
					return queryUrl;
				}
			);
			return result;
		}

		public string GetContentFileVersion (FigmaFileVersionQuery figmaQuery)
		{
			var result = GetContentUrl (figmaQuery,	(e) => GetFigmaFileVersionsUrl (e.FileId));
			return result;
		}

		public FigmaFileResponse GetFile (FigmaFileQuery figmaQuery)
		{
			var content = GetContentFile (figmaQuery);
			return FigmaApiHelper.GetFigmaResponseFromFileContent (content);
		}

		public FigmaFileVersionResponse GetFileVersions (FigmaFileVersionQuery figmaQuery)
		{
			var content = GetContentFileVersion (figmaQuery);
			return FigmaApiHelper.GetFigmaResponseFromFileVersionContent (content);
		}

		#region Images

		public FigmaImageResponse GetImages (string fileId, IFigmaDownloadImageNode[] resourceIds, ImageQueryFormat format = ImageQueryFormat.png, float scale = 2)
		{
			var currentIds = resourceIds.Select(s => s.ResourceId).ToArray();
			var query = new FigmaImageQuery (fileId, resourceIds);
			query.Scale = scale;
			query.Format = format;
			return GetImage (query);
		}

		public void ProcessDownloadImages (string fileId, IFigmaDownloadImageNode[] resourceIds, ImageQueryFormat format = ImageQueryFormat.png, float scale = 2)
		{
			var response = GetImages(fileId, resourceIds, format, scale);
            foreach (var image in response.images)
            {
				var resourceId = resourceIds.FirstOrDefault(s => s.ResourceId == image.Key);
				if (resourceId != null)
					resourceId.Url = image.Value;
			}
		}

		public FigmaImageResponse GetImage (FigmaImageQuery figmaQuery)
		{
			var result = GetContentUrl (figmaQuery,
				(e) => {
					var figmaImageUrl = GetFigmaImageUrl (figmaQuery.FileId, figmaQuery.Ids);
					var stringBuilder = new StringBuilder (figmaImageUrl);

					stringBuilder.Append (string.Format ("&format={0}", figmaQuery.Format));
					stringBuilder.Append (string.Format ("&scale={0}", figmaQuery.Scale));

					if (figmaQuery.Version != null) {
						stringBuilder.Append (string.Format ("&version={0}", figmaQuery.Version));
					}
					return stringBuilder.ToString ();
				}
			);

			return JsonConvert.DeserializeObject<FigmaImageResponse> (result);
		}

		string GetContentUrl <T> (T figmaQuery, Func<T, string> handler) where T : FigmaFileBaseQuery
		{
			var token = string.IsNullOrEmpty (figmaQuery.PersonalAccessToken) ?
	Token : figmaQuery.PersonalAccessToken;

			var query = handler (figmaQuery);

			var httpWebRequest = (HttpWebRequest)WebRequest.Create (query);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "GET";
			httpWebRequest.Headers["x-figma-token"] = token;

			var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse ();
			using (var streamReader = new StreamReader (httpResponse.GetResponseStream ())) {
				var result = streamReader.ReadToEnd ();
				var json = Newtonsoft.Json.Linq.JObject.Parse (result);
				return json.ToString ();
			}
		}

		#endregion
	}
}