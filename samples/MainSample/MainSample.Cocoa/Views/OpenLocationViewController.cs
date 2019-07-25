/* 
 * Author:
 *   Hylke Bons <hylbo@microsoft.com>
 *
 * Copyright (C) 2019 Microsoft, Corp
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

using AppKit;
using Foundation;

namespace FigmaSharp.Samples
{
    public partial class OpenLocationViewController : NSViewController
    {
        string keychain_token;

        string token_message;
        string token_message_unsaved = "This token will be saved in your keychain.";


        public OpenLocationViewController(IntPtr handle) : base(handle)
        {
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            token_message = TokenStatusTextField.StringValue;

            try {
                keychain_token = TokenStore.SharedTokenStore.GetToken();
                TokenTextField.StringValue = keychain_token;

            } catch {
                TokenStatusTextField.StringValue = token_message_unsaved;
                Console.WriteLine("No token found in keychain.");
            }

            TokenTextField.Changed += TokenTextFieldChanged;
            LinkComboBox.Changed += LinkComboboxChanged;
            CancelButton.Activated += delegate { View.Window.Close(); };

            PopulateLinkComboBox();

            OpenButton.Activated += OpenButtonActivated;
            OpenButton.Enabled = CheckFormIsFilled();
        }


        void PopulateLinkComboBox ()
        {
            string[] recents = GetRecents();

            if (recents != null) {
                foreach (string recent_item in GetRecents())
                    LinkComboBox.Add(new NSString(recent_item));
            }

            LinkComboBox.StringValue = "" + NSUserDefaults.StandardUserDefaults.StringForKey(LAST_DOCUMENT_KEY);
        }


        const string RECENT_DOCUMENTS_KEY = "recent_documents";
        const string LAST_DOCUMENT_KEY = "last_document";

        string[] GetRecents ()
        {
            return NSUserDefaults.StandardUserDefaults.StringArrayForKey(RECENT_DOCUMENTS_KEY);
        }


        void AddRecent (string link_id)
        {
            List<string> recent_documents = new List<string>();
            var list = NSUserDefaults.StandardUserDefaults.StringArrayForKey(RECENT_DOCUMENTS_KEY);

            if (list != null)
                recent_documents.AddRange(list);

            if (!recent_documents.Contains(link_id)) {
                recent_documents.Add(link_id);

                NSUserDefaults.StandardUserDefaults.SetValueForKey(NSArray.FromStrings(recent_documents.ToArray()), new NSString(RECENT_DOCUMENTS_KEY));
            }

            PopulateLinkComboBox();
            NSUserDefaults.StandardUserDefaults.Synchronize();
        }


        void TokenTextFieldChanged(Object sender, EventArgs args)
        {
            string token_message = TokenStatusTextField.StringValue;

            if (TokenTextField.StringValue == keychain_token)
                TokenStatusTextField.StringValue = token_message;
            else
                TokenStatusTextField.StringValue = token_message_unsaved;

            OpenButton.Enabled = CheckFormIsFilled();
        }


        void LinkComboboxChanged(object sender, EventArgs args)
        {
            LinkComboBox.StringValue = FigmaLink.TryParseID(LinkComboBox.StringValue);
            OpenButton.Enabled = CheckFormIsFilled();
        }


        void OpenButtonActivated(Object sender, EventArgs args)
        {
            View.Window.IsVisible = false;
            PerformSegue("OpenLocationSegue", this);
        }


        public override void PrepareForSegue(NSStoryboardSegue segue, NSObject sender)
        {
            string token = TokenTextField.StringValue.Trim();
            TokenStore.SharedTokenStore.SetToken(token);

            string link_id = LinkComboBox.StringValue.Trim();

            NSUserDefaults.StandardUserDefaults.SetValueForKey(new NSString(link_id), new NSString(LAST_DOCUMENT_KEY));
            AddRecent(link_id);

            var document_window_controller = (DocumentWindowController) segue.DestinationController;
            document_window_controller.LoadDocument(token, link_id);

            //document_window_controller.Window.WillClose += delegate {
            //    View.Window.IsVisible = true;
            //};
        }


        bool CheckFormIsFilled()
        {
            return (!string.IsNullOrWhiteSpace(LinkComboBox.StringValue) &&
                    !string.IsNullOrWhiteSpace(TokenTextField.StringValue));
        }


        public override NSObject RepresentedObject
        {
            get {
                return base.RepresentedObject;
            }

            set {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }
    }
}
