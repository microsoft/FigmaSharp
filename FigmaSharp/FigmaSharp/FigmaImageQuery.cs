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

namespace FigmaSharp
{
	public enum ImageQueryFormat
	{
		png,
		jpg,
		svg,
		pdf
	}

    public abstract class FigmaFileBaseQuery
    {
        public string FileId { get; private set; }
        public string PersonalAccessToken { get; private set; }

        public FigmaFileBaseQuery (string fileId, string personalAccessToken = null)
        {
            FileId = fileId;
            PersonalAccessToken = personalAccessToken;
        }
    }

    public class FigmaFileVersionQuery : FigmaFileBaseQuery
    {
        public FigmaFileVersionQuery (string fileId, string personalAccessToken = null) : base (fileId, personalAccessToken)
        {
        }
    }

    public class FigmaFileQuery : FigmaFileBaseQuery
    {
        public string Version { get; private set; }

        public FigmaFileQuery (string fileId, string version = null, string personalAccessToken = null) : base (fileId, personalAccessToken)
        {
            Version = version;
        }
    }

    public class FigmaImageQuery : FigmaFileBaseQuery
    {
        public FigmaImageQuery(string fileId, string[] ids, string personalAccessToken = null) : base (fileId, personalAccessToken)
        {
            Ids = ids;
        }

		/// <summary>
		/// A comma separated list of node IDs to render
		/// </summary>
		public string[] Ids { get; set; }

		/// <summary>
		/// A number between 0.01 and 4, the image scaling factor
		/// </summary>
		public float Scale { get; set; } = 1;

		/// <summary>
		/// A string enum for the image output format, can be jpg, png, svg, or pdf
		/// </summary>
		public ImageQueryFormat Format { get; set; }

		/// <summary>
		/// A specific version ID to use. Omitting this will use the current version of the file
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		/// Whether to include id attributes for all SVG elements. Default: false.
		/// </summary>
		public bool SvgIncludeId { get; set; }

		/// <summary>
		/// Whether to simplify inside/outside strokes and use stroke attribute if possible instead of mask. Default: true.
		/// </summary>
		public bool SvgSimplifyStroke { get; set; } = true;
	}
}
