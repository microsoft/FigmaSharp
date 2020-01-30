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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using FigmaSharp.Models;
using Newtonsoft.Json;
using System.Linq;

namespace FigmaSharp
{
	public class FigmaApi
	{
		internal string Token { get; set; }

		//TODO: right now there is no way to detect changes in API
		public string Version { get; } = "1.0";


		#region Urls

		string GetFigmaFileUrl (string fileId) => string.Format (FigmaFileUrl, fileId);
		string GetFigmaImageUrl (string fileId, params string[] imageIds)
		{
			return string.Format (FigmaImageUrl, fileId, string.Join (",", imageIds));
		}

		const string FigmaFileUrl = "https://api.figma.com/v1/files/{0}";
		const string FigmaImageUrl = "https://api.figma.com/v1/images/{0}?ids={1}";

		#endregion

		#region File

		#endregion

		public string GetContentFile (FigmaFileQuery figmaQuery)
		{
			var queryUrl = GetFigmaFileUrl (figmaQuery.FileId);
			var token = string.IsNullOrEmpty (figmaQuery.PersonalAccessToken) ?
				Token : figmaQuery.PersonalAccessToken;

			if (figmaQuery.Version != null) {
				queryUrl += string.Format ("&version={0}", figmaQuery.Version);
			}

			var httpWebRequest = (HttpWebRequest)WebRequest.Create (queryUrl);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "GET";
			httpWebRequest.Headers["x-figma-token"] = token;
			
			var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse ();
			using (var streamReader = new StreamReader (httpResponse.GetResponseStream ())) {
				return streamReader.ReadToEnd ();
			}
		}

		public FigmaResponse GetFile (FigmaFileQuery figmaFileQuery)
		{
			var content = GetContentFile (figmaFileQuery);
			return FigmaApiHelper.GetFigmaResponseFromContent (content);
		}

		#region Images

		public FigmaImageResponse GetImages (string fileId, string[] ids, ImageQueryFormat format = ImageQueryFormat.png, float scale = 2)
		{
			var query = new FigmaImageQuery (fileId, ids);
			query.Scale = scale;
			query.Format = format;
			return GetImage (query);
		}

		public FigmaImageResponse GetImage (FigmaImageQuery figmaQuery)
		{
			var figmaImageUrl = GetFigmaImageUrl (figmaQuery.FileId, figmaQuery.Ids);
			var stringBuilder = new StringBuilder (figmaImageUrl);
			if (figmaQuery.Scale != default) {
				stringBuilder.Append (string.Format ("&scale={0}", figmaQuery.Scale));
			}
			if (figmaQuery.Format != default) {
				stringBuilder.Append (string.Format ("&format={0}", figmaQuery.Format));
			}
			if (figmaQuery.Version != null) {
				stringBuilder.Append (string.Format ("&version={0}", figmaQuery.Version));
			}

			var httpWebRequest = (HttpWebRequest)WebRequest.Create (stringBuilder.ToString ());
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "GET";
			httpWebRequest.Headers["x-figma-token"] = figmaQuery.PersonalAccessToken;

			try {
				var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse ();
				using (var streamReader = new StreamReader (httpResponse.GetResponseStream ())) {
					var result = streamReader.ReadToEnd ();
					return JsonConvert.DeserializeObject<FigmaImageResponse> (result);
				}
			} catch (Exception ex) {
				Console.WriteLine (ex);
			}
			return null;
		}

		#endregion
	}
}