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

using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

namespace FigmaSharp.Models
{
	public class FigmaUser
	{
		public string handle { get; set; }
        public string img_url { get; set; }
        public string id { get; set; }
    }

	public class FigmaFileVersion
    {
        public string id { get; set; }
        public DateTime created_at { get; set; }
        public string label { get; set; }
        public string description { get; set; }
        public FigmaUser user { get; set; }
        public string thumbnail_url { get; set; }
    }

	public class FigmaFileVersionResponse
	{
        public FigmaFileVersion[] versions { get; set; }
    }

    public class FigmaComponent
    {
        public string key { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class FigmaStyle
    {
        public string key { get; set; }
        public string name { get; set; }
        public string styleType { get; set; }
    }

    /// <summary>
    /// FigmaResponse contains the raw data requested from a figma.com URL.
    /// </summary>
    public class FigmaFileResponse
    {
        public FigmaDocument document { get; set; }

        public Dictionary<string, FigmaComponent> components { get; set; }
        public Dictionary<string, FigmaStyle> styles { get; set; }

        public string name { get; set; }
        public string lastModified { get; set; }
        public string version { get; set; }
        public string thumbnailUrl { get; set; }
        public int schemaVersion { get; set; }


		public void Save (string filePath)
		{
            if (File.Exists (filePath)) {
                File.Delete (filePath);
            }
            using (var file = File.CreateText (filePath)) {
				var serializer = new JsonSerializer {
					Formatting = Formatting.Indented
				};
				serializer.Serialize (file, this);
            }
        }
    }

    /// <summary>
    /// FigmaImageResponse contains a list of URLs to images requested from a figma.com URL.
    /// </summary>
    public class FigmaImageResponse
    {
        public string err { get; set; }
        public Dictionary<string, string> images { get; set; }
    }
}
