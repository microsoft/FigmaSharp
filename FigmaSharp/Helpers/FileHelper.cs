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
using System.Net;
using System.Threading.Tasks;
using FigmaSharp.Converters;

namespace FigmaSharp
{
    public static class FileHelper
    {
        //TODO: Change to async multithread
        public static void SaveFiles(string destinationDirectory, string format, Dictionary<string, string> remotefile)
        {
            if (!Directory.Exists(destinationDirectory))
            {
                throw new DirectoryNotFoundException(destinationDirectory);
            }
            List<Task> downloads = new List<Task>();

            foreach (var file in remotefile)
            {
                if (file.Value == null)
                {
                    continue;
                }

                var key = FigmaResourceConverter.FromResource(file.Key);
                var fileName = string.Concat(Path.GetFileName(key), format);
                var fullPath = Path.Combine(destinationDirectory, fileName);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }

                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(new Uri(file.Value), fullPath);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            };
            }
        }
    }
