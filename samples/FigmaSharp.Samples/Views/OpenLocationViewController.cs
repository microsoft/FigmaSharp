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

using AppKit;
using Foundation;

namespace FigmaSharp.Samples
{
    public partial class OpenLocationViewController : NSViewController
    {
        public static event LinkOpenedHandler LinkOpened = delegate { };
        public delegate void LinkOpenedHandler(string link, string token);


        public OpenLocationViewController(IntPtr handle) : base(handle)
        {
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();


            LinkComboBox.Changed += delegate {
                LinkComboBox.StringValue = FigmaLink.TryParseID(LinkComboBox.StringValue);
                OpenButton.Enabled = CheckFormIsFilled();
            };


            string token = "";
            string token_message = TokenStatusTextField.StringValue;
            string token_message_unsaved = "This token will be saved in your keychain.";

            TokenTextField.Changed += delegate {
                OpenButton.Enabled = CheckFormIsFilled();

                if (TokenTextField.StringValue == token)
                    TokenStatusTextField.StringValue = token_message;
                else
                    TokenStatusTextField.StringValue = token_message_unsaved;
            };


            try {
                token = TokenStore.SharedTokenStore.GetToken();
                TokenTextField.StringValue = token;

            } catch {
                TokenStatusTextField.StringValue = token_message_unsaved;
                Console.WriteLine("No token found in keychain.");
            }


            CancelButton.Activated += delegate {
                View.Window.Close();
            };

            OpenButton.Activated += delegate {
                View.Window.Close();

                token = TokenTextField.StringValue.Trim();
                TokenStore.SharedTokenStore.SetToken(token);

                string link = LinkComboBox.StringValue.Trim();
                LinkOpened(link, token);
            };


            OpenButton.Enabled = CheckFormIsFilled();
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
