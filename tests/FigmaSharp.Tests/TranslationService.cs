// Authors:
//   jmedrano <josmed@microsoft.com>
//
// Copyright (C) 2020 Microsoft, Corp
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using FigmaSharp.Services;
using FigmaSharp.Controls.Cocoa.Services;
using FigmaSharp.Controls.Cocoa;
using FigmaSharp.Converters;
using System.Text;
using System.Net;
using NUnit.Framework;
using System.IO;
using System.Text.RegularExpressions;

namespace FigmaSharp.Tests
{
    public class TestBase
    {
        protected NodeConverter[] Converters = new NodeConverter[]
        {
            new FigmaSharp.Controls.Cocoa.Converters.TextFieldConverter()
        };

        public NativeViewCodeService GetTestNativeViewCodeService (INodeProvider provider, PropertyConfigure.CodePropertyConfigureBase codePropertyConfigure = null, ITranslationService translationService = null)
        {
            return new NativeViewCodeService(provider, Converters, codePropertyConfigure, translationService);
        }
    }

    [TestFixture]
    public class TranslationService : TestBase
    {
        public static class MyMagicalService
        {
            public static string GetTranslation (string text)
            {
                if (text == "hello")
                    return "hola";
                if (text == "goodbye")
                    return "adios";
                return text;
            }
        }

        class MyCustomTranslationService : ITranslationService
        {
            public string GetTranslatedStringText(string text)
            {
                return string.Format("{0}.{1} (\"{2}\")", typeof(MyMagicalService), nameof(MyMagicalService.GetTranslation), text);
            }
        }

        public TranslationService()
        {

        }

        string UrlEncode(string url)
        {
            String s = System.Net.WebUtility.UrlEncode(url);
            s = Regex.Replace(s, "(%[0-9A-F]{2})", c => c.Value.ToLowerInvariant());
            return s;
        }

        const string ERRORSTRINGSTART = "<font color=red>";
        const string ERRORSTRINGEND = "</font>";

        public string BabelFish(string translationmode = "en_es", string sourcedata = "hello")
        {
            try
            {
                // validate and remove trailing spaces
                if (translationmode == null || translationmode.Length == 0) throw new ArgumentNullException("translationmode");
                if (sourcedata == null || translationmode.Length == 0) throw new ArgumentNullException("sourcedata");
                translationmode = translationmode.Trim();
                sourcedata = sourcedata.Trim();
                // check for valid translationmodes
                bool validtranslationmode = false;
                for (int i = 0; i < VALIDTRANSLATIONMODES.Length; i++)
                {
                    if (VALIDTRANSLATIONMODES[i] == translationmode)
                    {
                        validtranslationmode = true;
                        break;
                    }
                }
                if (!validtranslationmode) return ERRORSTRINGSTART + "The translationmode specified was not a valid translation translationmode" + ERRORSTRINGEND;
                Uri uri = new Uri(BABELFISHURL);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Referer = BABELFISHREFERER;
                // Encode all the sourcedata 
                string postsourcedata;
                postsourcedata = "lp=" + translationmode + "&tt=urltext&intl=1&doit=done&urltext=" + UrlEncode(sourcedata);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postsourcedata.Length;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
                Stream writeStream = request.GetRequestStream();
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] bytes = encoding.GetBytes(postsourcedata);
                writeStream.Write(bytes, 0, bytes.Length);
                writeStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8);
                string page = readStream.ReadToEnd();
                Regex reg = new Regex(@"<div style=padding:10px;>((?:.|\n)*?)</div>", RegexOptions.IgnoreCase);
                MatchCollection matches = reg.Matches(page);
                if (matches.Count != 1 || matches[0].Groups.Count != 2)
                {
                    return ERRORSTRINGSTART + "The HTML returned from Babelfish appears to have changed. Please check for an updated regular expression" + ERRORSTRINGEND;
                }
                return matches[0].Groups[1].Value;
            }
            catch (ArgumentNullException ex)
            {
                return ERRORSTRINGSTART + ex.Message + ERRORSTRINGEND;
            }
            catch (ArgumentException ex)
            {
                return ERRORSTRINGSTART + ex.Message + ERRORSTRINGEND;
            }
            catch (WebException)
            {
                return ERRORSTRINGSTART + "There was a problem connecting to the Babelfish server" + ERRORSTRINGEND;
            }
            catch (System.Security.SecurityException)
            {
                return ERRORSTRINGSTART + "Error: you do not have permission to make HTTP connections. Please check your assembly's permission settings" + ERRORSTRINGEND;
            }
            catch (Exception ex)
            {
                return ERRORSTRINGSTART + "An unspecified error occured: " + ex.Message + ERRORSTRINGEND;
            }
        }

        readonly string[] VALIDTRANSLATIONMODES = new string[] { "en_zh", "en_fr", "en_de", "en_it", "en_ja", "en_ko", "en_pt", "en_es", "zh_en", "fr_en", "fr_de", "de_en", "de_fr", "it_en", "ja_en", "ko_en", "pt_en", "ru_en", "es_en" };

        const string BABELFISHURL = "http://babelfish.altavista.com/babelfish/tr";
        const string BABELFISHREFERER = "http://babelfish.altavista.com/";

        [Test]
        public void test ()
        {
            string fromCulture = "en";
            string toCulture = "es";
            string translationMode = string.Concat(fromCulture, "_", toCulture);

            string url = String.Format("http://babelfish.yahoo.com/translate_txt?lp={0}&tt=urltext&intl=1&doit=done&urltext={1}", translationMode, UrlEncode("hello"));
            WebClient webClient = new WebClient();
            webClient.Encoding = System.Text.Encoding.Default;
            string page = webClient.DownloadString(url);

            int start = page.IndexOf("<div style=\"padding:0.6em;\">") + "<div style=\"padding:0.6em;\">".Length;
            int finish = page.IndexOf("</div>", start);
            string retVal = page.Substring(start, finish - start);

            var nodeProvider = new RemoteNodeProvider();
            var translationCodeService = new MyCustomTranslationService();

            nodeProvider.Load("QzEgq2772k2eeMF2sVNc3kEY");
            var codeService = new NativeViewCodeService(nodeProvider, Converters, translationService: translationCodeService);


        }
    }
}
