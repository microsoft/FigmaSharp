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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FigmaSharpApp.Translation
{
    /// <summary>
    /// The C# classes that represents the JSON returned by the Translator.
    /// </summary>
    public class TranslationResult
    {
        public DetectedLanguage DetectedLanguage { get; set; }
        public TextResult SourceText { get; set; }
        public Translation[] Translations { get; set; }
    }

    public class DetectedLanguage
    {
        public string Language { get; set; }
        public float Score { get; set; }
    }

    public class TextResult
    {
        public string Text { get; set; }
        public string Script { get; set; }
    }

    public class Translation
    {
        public string Text { get; set; }
        public TextResult Transliteration { get; set; }
        public string To { get; set; }
        public Alignment Alignment { get; set; }
        public SentenceLength SentLen { get; set; }
    }

    public class Alignment
    {
        public string Proj { get; set; }
    }

    public class SentenceLength
    {
        public int[] SrcSentLen { get; set; }
        public int[] TransSentLen { get; set; }
    }

    public class AzureTranslatorService : IAppTranslationService
    {
        string subscriptionKey;
        string traceId;
        public AzureTranslatorService ()
		{
            this.subscriptionKey = Environment.GetEnvironmentVariable("AZURE_TOKEN");
            this.traceId = Environment.GetEnvironmentVariable("AZURE_TRACEID") ?? "A14C9DB9-0DED-48D7-8BBE-C517A1A8DBB0";
        }

        const string uriRequest = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&to={0}&from={1}";

        public string[] GetTranlationLangs()
        {
            return new string[]
            {
                "es"
            };
        }

        public string OutputLanguage { get; private set; } = "es";

        public string InputLanguage { get; private set; } = "en";

        string route = "https://dev.microsofttranslator.com/translate?api-version=3.0&from={0}&to={1}";

        public async Task<string> TranslateText(string text)
        {
            if (string.IsNullOrEmpty(subscriptionKey))
                return text;

            try
            {
                var body = new object[] { new { Text = text } };
                var requestBody = JsonConvert.SerializeObject(body);
                //var endpoint = "https://dev.microsofttranslator.com/";

                using (var client = new HttpClient())
                using (HttpRequestMessage request = new HttpRequestMessage())
                {
                    // Build the request.
                    // Set the method to Post.
                    request.Method = HttpMethod.Post;
                    // Construct the URI and add headers.
                    request.RequestUri = new Uri(string.Format(route, InputLanguage, OutputLanguage));
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                    request.Headers.Add("X-ClientTraceID", traceId);
                    request.Headers.Add("Host", "dev.microsofttranslator.com");

                    // Send the request and get response.
                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                    // Read response as a string.
                    string result = await response.Content.ReadAsStringAsync();
                    // Deserialize the response using the classes created earlier.
                    TranslationResult[] deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);
                    // Iterate over the deserialized results.

                    return deserializedOutput[0].Translations[0].Text;
                }
            }
            catch (Exception)
            {
                return text;
            }
        }

        public void SetOutputLanguageLanguage(string lang)
        {
            OutputLanguage = lang;
        }

        public string GetTranslatedStringText(string text)
        {
            return TranslateText(text).Result;
        }

        public void SetInputLanguageLanguage(string lang)
        {
            InputLanguage = lang;
        }
    }
}
