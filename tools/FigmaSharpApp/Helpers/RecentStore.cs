/* 
 * Author:
 *   Hylke Bons <hylbo@microsoft.com>
 *
 * Copyright (C) 2020 Microsoft, Corp
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


using Foundation;

namespace FigmaSharpApp
{
    class RecentStore
    {
        public static RecentStore SharedRecentStore = new RecentStore ();

        NSString mostRecentDocumentString = new NSString("RecentDocument");
        NSString recentDocumentsString = new NSString("RecentDocuments");


        public void AddRecent(string link_id, string title)
        {
            NSDictionary readonlyDict = NSUserDefaults.StandardUserDefaults.DictionaryForKey(recentDocumentsString);
            NSMutableDictionary dict = new NSMutableDictionary(readonlyDict);

            if (dict == null)
                dict = new NSMutableDictionary();

            dict.Add(new NSString(link_id), new NSString(title));

            if (!string.IsNullOrWhiteSpace(title))
                NSUserDefaults.StandardUserDefaults.SetString(title, mostRecentDocumentString);
            else
                NSUserDefaults.StandardUserDefaults.SetString(link_id, mostRecentDocumentString);

            NSUserDefaults.StandardUserDefaults.SetValueForKey(dict, new NSString(recentDocumentsString));
            NSUserDefaults.StandardUserDefaults.Synchronize();
        }


        public NSDictionary GetRecents()
        {
            return (NSDictionary) NSUserDefaults.StandardUserDefaults.ValueForKey(recentDocumentsString);
        }


        public string GetMostRecent()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(mostRecentDocumentString);
        }
    }
}
