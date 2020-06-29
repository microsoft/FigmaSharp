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
using System.Text;
using Foundation;
using Security;

namespace FigmaSharp.Cocoa
{
	public class TokenStore
	{
		public static TokenStore Current = new TokenStore ();

		const string SERVICE = "FigmaSharp";
		const string ACCOUNT = "Figma API Token";

		public void SetToken (string token)
		{
			var record = FetchRecord(ACCOUNT, out var searchRecord);

			if (record == null)
			{
				record = new SecRecord(SecKind.InternetPassword)
				{
					Service = SERVICE,
					Label = SERVICE,
					Account = ACCOUNT,
					ValueData = NSData.FromString(token)
				};

				SecKeyChain.Add(record);
				return;
			}

			record.ValueData = NSData.FromString(token);
			SecKeyChain.Update(searchRecord, record);
		}

		public string GetToken ()
		{
			GetPassword(ACCOUNT, out var password);
			return password;
		}

		/// <summary>
		/// Gets the password from the OSX keychain
		/// </summary>
		/// <returns>
		/// Password is present in the keychain
		/// </returns>
		/// <param name='username'>
		/// The username
		/// </param>
		/// <param name='password'>
		/// The stored password
		/// </param>
		bool GetPassword(string username, out string password)
		{
			SecRecord searchRecord;
			var record = FetchRecord(username, out searchRecord);
			if (record == null)
			{
				password = string.Empty;
				return false;
			}
			password = NSString.FromData(record.ValueData, NSStringEncoding.UTF8);
			return true;
		}

		/// <summary>
		/// Fetchs the record from the keychain
		/// </summary>
		/// <returns>
		/// The record or NULL
		/// </returns>
		/// <param name='username'>
		/// Username of record to fetch
		/// </param>
		/// <param name='searchRecord'>
		/// The search record used to fetch the returned record
		/// </param>
		SecRecord FetchRecord(string username, out SecRecord searchRecord)
		{
			searchRecord = new SecRecord(SecKind.InternetPassword)
			{
				Service = SERVICE,
				Account = username
			};

			SecStatusCode code;
			var data = SecKeyChain.QueryAsRecord(searchRecord, out code);

			if (code == SecStatusCode.Success)
				return data;
			else
				return null;
		}
	}
}
