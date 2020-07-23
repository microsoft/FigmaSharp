using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using FigmaSharp.Services;

namespace FigmaSharpApp.Translation
{
    public interface IAppTranslationService : ITranslationService
    {
        string[] GetTranlationLangs();
        string OutputLanguage { get; }
        string InputLanguage { get; }
        void SetInputLanguageLanguage(string lang);
        void SetOutputLanguageLanguage(string lang);
    }

	public class GoogleTranslationService : IAppTranslationService
    {
        GoogleTranslator translator;
        public GoogleTranslationService()
		{
            translator = new GoogleTranslator ();
        }

        public string[] GetTranlationLangs ()
        {
            return GoogleTranslator.Languages.ToArray();
        }

        public string OutputLanguage { get; private set; } = "French";

        public string InputLanguage { get; private set; } = "English";

        public void SetOutputLanguageLanguage (string lang)
        {
            if (!GoogleTranslator.Languages.Any(s => s == lang))
                throw new Exception("lang doesn't exists");
            OutputLanguage = lang;
        }

        public void SetInputLanguageLanguage(string lang)
        {
            if (!GoogleTranslator.Languages.Any(s => s == lang))
                throw new Exception("lang doesn't exists");
            InputLanguage = lang;
        }

        public string GetTranslatedStringText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            var newText = translator.Translate(text, InputLanguage, OutputLanguage);
            return newText;
        }
    }

    /// <summary>
    /// Translates text using Google's online language tools.
    /// </summary>
    public class GoogleTranslator
    {
        #region Properties

        /// <summary>
        /// Gets the supported languages.
        /// </summary>
        public static IEnumerable<string> Languages
        {
            get
            {
                GoogleTranslator.EnsureInitialized();
                return GoogleTranslator._languageModeMap.Keys.OrderBy(p => p);
            }
        }

        /// <summary>
        /// Gets the time taken to perform the translation.
        /// </summary>
        public TimeSpan TranslationTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the url used to speak the translation.
        /// </summary>
        /// <value>The url used to speak the translation.</value>
        public string TranslationSpeechUrl
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        public Exception Error
        {
            get;
            private set;
        }

        #endregion

        #region Public methods

        string UrlEncode(string url)
        {
            String s = System.Net.WebUtility.UrlEncode(url);
            s = Regex.Replace(s, "(%[0-9A-F]{2})", c => c.Value.ToLowerInvariant());
            return s;
        }

        /// <summary>
        /// Translates the specified source text.
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="sourceLanguage">The source language.</param>
        /// <param name="targetLanguage">The target language.</param>
        /// <returns>The translation.</returns>
        public string Translate
            (string sourceText,
             string sourceLanguage,
             string targetLanguage)
        {
            // Initialize
            this.Error = null;
            this.TranslationSpeechUrl = null;
            this.TranslationTime = TimeSpan.Zero;
            DateTime tmStart = DateTime.Now;
            string translation = string.Empty;

            try
            {
                // Download translation
                string url = string.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
                                            GoogleTranslator.LanguageEnumToIdentifier(sourceLanguage),
                                            GoogleTranslator.LanguageEnumToIdentifier(targetLanguage),
                                            UrlEncode(sourceText));
                string outputFile = Path.GetTempFileName();
                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("user-agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 13_3_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148 [FBAN/FBIOS;FBDV/iPhone11,8;FBMD/iPhone;FBSN/iOS;FBSV/13.3.1;FBSS/2;FBID/phone;FBLC/en_US;FBOP/5;FBCR/]");
                    wc.DownloadFile(url, outputFile);
                }

                // Get translated text
                if (File.Exists(outputFile))
                {

                    // Get phrase collection
                    string text = File.ReadAllText(outputFile);
                    int index = text.IndexOf(string.Format(",,\"{0}\"", GoogleTranslator.LanguageEnumToIdentifier(sourceLanguage)));
                    if (index == -1)
                    {
                        // Translation of single word
                        int startQuote = text.IndexOf('\"');
                        if (startQuote != -1)
                        {
                            int endQuote = text.IndexOf('\"', startQuote + 1);
                            if (endQuote != -1)
                            {
                                translation = text.Substring(startQuote + 1, endQuote - startQuote - 1);
                            }
                        }
                    }
                    else
                    {
                        // Translation of phrase
                        text = text.Substring(0, index);
                        text = text.Replace("],[", ",");
                        text = text.Replace("]", string.Empty);
                        text = text.Replace("[", string.Empty);
                        text = text.Replace("\",\"", "\"");

                        // Get translated phrases
                        string[] phrases = text.Split(new[] { '\"' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; (i < phrases.Count()); i += 2)
                        {
                            string translatedPhrase = phrases[i];
                            if (translatedPhrase.StartsWith(",,"))
                            {
                                i--;
                                continue;
                            }
                            translation += translatedPhrase + "  ";
                        }
                    }

                    // Fix up translation
                    translation = translation.Trim();
                    translation = translation.Replace(" ?", "?");
                    translation = translation.Replace(" !", "!");
                    translation = translation.Replace(" ,", ",");
                    translation = translation.Replace(" .", ".");
                    translation = translation.Replace(" ;", ";");

                    // And translation speech URL
                    this.TranslationSpeechUrl = string.Format("https://translate.googleapis.com/translate_tts?ie=UTF-8&q={0}&tl={1}&total=1&idx=0&textlen={2}&client=gtx",
                                                               UrlEncode(translation), GoogleTranslator.LanguageEnumToIdentifier(targetLanguage), translation.Length);
                }
            }
            catch (Exception ex)
            {
                this.Error = ex;
            }

            // Return result
            this.TranslationTime = DateTime.Now - tmStart;
            return translation;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Converts a language to its identifier.
        /// </summary>
        /// <param name="language">The language."</param>
        /// <returns>The identifier or <see cref="string.Empty"/> if none.</returns>
        private static string LanguageEnumToIdentifier
            (string language)
        {
            string mode = string.Empty;
            GoogleTranslator.EnsureInitialized();
            GoogleTranslator._languageModeMap.TryGetValue(language, out mode);
            return mode;
        }

        /// <summary>
        /// Ensures the translator has been initialized.
        /// </summary>
        private static void EnsureInitialized()
        {
            if (GoogleTranslator._languageModeMap == null)
            {
                GoogleTranslator._languageModeMap = new Dictionary<string, string>();
                GoogleTranslator._languageModeMap.Add("Afrikaans", "af");
                GoogleTranslator._languageModeMap.Add("Albanian", "sq");
                GoogleTranslator._languageModeMap.Add("Arabic", "ar");
                GoogleTranslator._languageModeMap.Add("Armenian", "hy");
                GoogleTranslator._languageModeMap.Add("Azerbaijani", "az");
                GoogleTranslator._languageModeMap.Add("Basque", "eu");
                GoogleTranslator._languageModeMap.Add("Belarusian", "be");
                GoogleTranslator._languageModeMap.Add("Bengali", "bn");
                GoogleTranslator._languageModeMap.Add("Bulgarian", "bg");
                GoogleTranslator._languageModeMap.Add("Catalan", "ca");
                GoogleTranslator._languageModeMap.Add("Chinese", "zh-CN");
                GoogleTranslator._languageModeMap.Add("Croatian", "hr");
                GoogleTranslator._languageModeMap.Add("Czech", "cs");
                GoogleTranslator._languageModeMap.Add("Danish", "da");
                GoogleTranslator._languageModeMap.Add("Dutch", "nl");
                GoogleTranslator._languageModeMap.Add("English", "en");
                GoogleTranslator._languageModeMap.Add("Esperanto", "eo");
                GoogleTranslator._languageModeMap.Add("Estonian", "et");
                GoogleTranslator._languageModeMap.Add("Filipino", "tl");
                GoogleTranslator._languageModeMap.Add("Finnish", "fi");
                GoogleTranslator._languageModeMap.Add("French", "fr");
                GoogleTranslator._languageModeMap.Add("Galician", "gl");
                GoogleTranslator._languageModeMap.Add("German", "de");
                GoogleTranslator._languageModeMap.Add("Georgian", "ka");
                GoogleTranslator._languageModeMap.Add("Greek", "el");
                GoogleTranslator._languageModeMap.Add("Haitian Creole", "ht");
                GoogleTranslator._languageModeMap.Add("Hebrew", "iw");
                GoogleTranslator._languageModeMap.Add("Hindi", "hi");
                GoogleTranslator._languageModeMap.Add("Hungarian", "hu");
                GoogleTranslator._languageModeMap.Add("Icelandic", "is");
                GoogleTranslator._languageModeMap.Add("Indonesian", "id");
                GoogleTranslator._languageModeMap.Add("Irish", "ga");
                GoogleTranslator._languageModeMap.Add("Italian", "it");
                GoogleTranslator._languageModeMap.Add("Japanese", "ja");
                GoogleTranslator._languageModeMap.Add("Korean", "ko");
                GoogleTranslator._languageModeMap.Add("Lao", "lo");
                GoogleTranslator._languageModeMap.Add("Latin", "la");
                GoogleTranslator._languageModeMap.Add("Latvian", "lv");
                GoogleTranslator._languageModeMap.Add("Lithuanian", "lt");
                GoogleTranslator._languageModeMap.Add("Macedonian", "mk");
                GoogleTranslator._languageModeMap.Add("Malay", "ms");
                GoogleTranslator._languageModeMap.Add("Maltese", "mt");
                GoogleTranslator._languageModeMap.Add("Norwegian", "no");
                GoogleTranslator._languageModeMap.Add("Persian", "fa");
                GoogleTranslator._languageModeMap.Add("Polish", "pl");
                GoogleTranslator._languageModeMap.Add("Portuguese", "pt");
                GoogleTranslator._languageModeMap.Add("Romanian", "ro");
                GoogleTranslator._languageModeMap.Add("Russian", "ru");
                GoogleTranslator._languageModeMap.Add("Serbian", "sr");
                GoogleTranslator._languageModeMap.Add("Slovak", "sk");
                GoogleTranslator._languageModeMap.Add("Slovenian", "sl");
                GoogleTranslator._languageModeMap.Add("Spanish", "es");
                GoogleTranslator._languageModeMap.Add("Swahili", "sw");
                GoogleTranslator._languageModeMap.Add("Swedish", "sv");
                GoogleTranslator._languageModeMap.Add("Tamil", "ta");
                GoogleTranslator._languageModeMap.Add("Telugu", "te");
                GoogleTranslator._languageModeMap.Add("Thai", "th");
                GoogleTranslator._languageModeMap.Add("Turkish", "tr");
                GoogleTranslator._languageModeMap.Add("Ukrainian", "uk");
                GoogleTranslator._languageModeMap.Add("Urdu", "ur");
                GoogleTranslator._languageModeMap.Add("Vietnamese", "vi");
                GoogleTranslator._languageModeMap.Add("Welsh", "cy");
                GoogleTranslator._languageModeMap.Add("Yiddish", "yi");
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// The language to translation mode map.
        /// </summary>
        private static Dictionary<string, string> _languageModeMap;

        #endregion
    }
}
