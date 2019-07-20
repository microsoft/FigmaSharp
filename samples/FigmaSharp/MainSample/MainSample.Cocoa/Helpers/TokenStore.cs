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

using Security;

namespace FigmaSharp.Samples
{
    class TokenStore
    {
        public static TokenStore SharedTokenStore = new TokenStore ();

        const string SERVICE = "FigmaSharp";
        const string ACCOUNT = "Figma API Token";


        public void SetToken (string token)
        {
            byte[] password_bytes = Encoding.UTF8.GetBytes (token);

            SecStatusCode result = SecKeyChain.AddGenericPassword (
                SERVICE, ACCOUNT, password_bytes);

            if (result == SecStatusCode.DuplicateItem)
            {
                // TODO: Replace the token
                // SecKeyChain.Remove ();
                // StoreToken (token);
                return;
            }

            if (result != SecStatusCode.Success)
                throw new Exception ("Could not store token in Keychain");
        }


        public string GetToken ()
        {
            byte[] password_bytes;

            SecStatusCode result = SecKeyChain.FindGenericPassword (
                SERVICE, ACCOUNT, out password_bytes);

            if (result != SecStatusCode.Success)
                throw new Exception ("Could not find token in Keychain");

            return Encoding.UTF8.GetString (password_bytes);
        }
    }
}
