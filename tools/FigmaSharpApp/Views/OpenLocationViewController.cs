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
using FigmaSharp.Cocoa;
using Foundation;

namespace FigmaSharpApp
{
	public partial class OpenLocationViewController : NSViewController
	{
		string keychain_token;

		string token_message;
		const string token_message_unsaved = "No token found in keychain";


		public OpenLocationViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			token_message = TokenStatusTextField.StringValue;

			if (FigmaSharp.AppContext.Current.IsApiConfigured)
				TokenTextField.StringValue = FigmaSharp.AppContext.Api.Token;
			else
				TokenStatusTextField.StringValue = token_message_unsaved;

			TokenTextField.Changed += TokenTextFieldChanged;
			LinkComboBox.Changed += LinkComboboxChanged;
			CancelButton.Activated += delegate { View.Window.Close (); };

			OpenButton.Activated += OpenButtonActivated;
		}

		void UpdateComboBox ()
		{
			LinkComboBox?.RemoveAll();

			string mostRecentDocument = RecentStore.SharedRecentStore.GetMostRecent();
			var recentDocuments = RecentStore.SharedRecentStore.GetRecents();

			if (!string.IsNullOrEmpty(mostRecentDocument))
				LinkComboBox.StringValue = mostRecentDocument;

			if (recentDocuments != null)
			{
				LinkComboBox.Completes = true;
				LinkComboBox.MaximumNumberOfLines = 12;

				foreach (NSString link_id in recentDocuments.Keys)
				{
					var title = (NSString)recentDocuments.ValueForKey(link_id);

					if (!string.IsNullOrWhiteSpace(title.ToString()))
						LinkComboBox.Add(title);
					else
						LinkComboBox.Add(link_id);
				}
			}
		}

		public override void ViewDidAppear()
		{
			base.ViewDidAppear();
			UpdateComboBox();
			OpenButton.Enabled = CheckFormIsFilled();

			if (OpenLocationWindow.Window != null)
				OpenLocationWindow.Window.Center();
		}

		void TokenTextFieldChanged (Object sender, EventArgs args)
		{
			string token_message = TokenStatusTextField.StringValue;

			if (TokenTextField.StringValue == keychain_token)
				TokenStatusTextField.StringValue = token_message;
			else
				TokenStatusTextField.StringValue = token_message_unsaved;

			OpenButton.Enabled = CheckFormIsFilled ();
		}

		void LinkComboboxChanged (object sender, EventArgs args)
		{
			LinkComboBox.StringValue = FigmaLink.TryParseID (LinkComboBox.StringValue);
			OpenButton.Enabled = CheckFormIsFilled ();
		}

		void OpenButtonActivated (Object sender, EventArgs args)
		{
			View.Window.IsVisible = false;

			//is our api key different we store it
			if (TokenTextField.StringValue != FigmaSharp.AppContext.Api.Token)
			{
				FigmaSharp.AppContext.Current.SetAccessToken(TokenTextField.StringValue);
				try
				{
					TokenStore.Current.SetToken(TokenTextField.StringValue);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			}
			PerformSegue ("OpenLocationSegue", this);
		}

		public override void PrepareForSegue (NSStoryboardSegue segue, NSObject sender)
		{
			string token = TokenTextField.StringValue.Trim ();
			string link_id = LinkComboBox.StringValue.Trim ();

			// Check if the Link ID is actually a title from the recent list
			nint selectedComboBoxIndex = Array.IndexOf(LinkComboBox.Values, new NSString(link_id));

			if (selectedComboBoxIndex < 0)
				selectedComboBoxIndex = LinkComboBox.SelectedIndex;

			if (selectedComboBoxIndex > -1)
			{
				NSDictionary recents = RecentStore.SharedRecentStore.GetRecents();
				link_id = (NSString)recents?.Keys[selectedComboBoxIndex];
			}


			var windowController = (DocumentWindowController)segue.DestinationController;
			var contentController = (DocumentViewController)windowController.ContentViewController;

			contentController.OnInitialize ();
			contentController.LoadDocument (token, link_id);
		}

		bool CheckFormIsFilled ()
		{
			return (!string.IsNullOrWhiteSpace (LinkComboBox.StringValue) &&
					!string.IsNullOrWhiteSpace (TokenTextField.StringValue));
		}

		public override NSObject RepresentedObject {
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
