// Copyright (c) 2004-2012 Zenfolio, Inc. All rights reserved.
//
// Permission is hereby granted,  free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction,  including without limitation the rights
// to use, copy, modify, merge,  publish,  distribute,  sublicense,  and/or sell
// copies of the Software,  and  to  permit  persons  to  whom  the Software  is 
// furnished to do so, subject to the following conditions:
// 
// The above  copyright notice  and this permission notice  shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE  IS PROVIDED "AS IS",  WITHOUT WARRANTY OF ANY KIND,  EXPRESS OR
// IMPLIED,  INCLUDING BUT NOT LIMITED  TO  THE WARRANTIES  OF  MERCHANTABILITY,
// FITNESS  FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
// AUTHORS  OR  COPYRIGHT HOLDERS  BE LIABLE FOR  ANY CLAIM,  DAMAGES  OR  OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.


//
// Convenience wrapper for generated web reference class.
//

using System;
using System.Net;
using System.Text;
using System.Security.Cryptography;

using Zenfolio.Examples.Uploader.ZfApiRef;
using System.IO;

namespace Zenfolio.Examples.Uploader
{
    /// <summary>
    /// Transparent helper inherited from generated ZfApi web service proxy class.
    /// Provides login sequence and manages authentication token.
    /// </summary>
    public class ZenfolioClient: ZfApi
    {
        private string _token;
        private string _loginName;
        private string _authority;

        public ZenfolioClient()
        {
        }

        /// <summary>
        /// Gets login name of current user
        /// </summary>
        public string LoginName
        {
            get { return _loginName; }
        }

        /// <summary>
        /// Gets current user token.
        /// </summary>
        public string Token
        {
            get { return _token; }
        }

        /// <summary>
        /// Gets Zenfolio authority i.e. scheme/host/port
        /// </summary>
        public string Authority
        {
            get 
            { 
                if (_authority == null)
                    _authority = new Uri(this.Url).GetLeftPart(UriPartial.Authority);
                return _authority; 
            }
        }

        /// <summary>
        /// Computes salted data hash
        /// </summary>
        /// <param name="data">Data to hash</param>
        /// <param name="salt">Salt</param>
        /// <returns>Computed SHA-256 hash of salt+data pair</returns>
        private static byte[] HashData(byte[] salt, byte[] data)
        {
            byte[] buffer = new byte[data.Length + salt.Length];
            salt.CopyTo(buffer, 0);
            data.CopyTo(buffer, salt.Length);
            return new SHA256Managed().ComputeHash(buffer);
        }

        /// <summary>
        /// Logs into Zenfolio API
        /// </summary>
        /// <param name="loginName">User's login name</param>
        /// <param name="password">User's password</param>
        /// <returns>True if login was successful, false otherwise.</returns>
        public bool Login(string loginName, string password)
        {
            // Get API challenge
            AuthChallenge ch = this.GetChallenge(loginName);

            // Extract and hash password bytes
            byte[] passwordHash = HashData(ch.PasswordSalt,
                                           Encoding.UTF8.GetBytes(password));

            // Compute secret proof
            byte[] proof = HashData(ch.Challenge, passwordHash);

            // Authenticate
            try
            {
                _token = this.Authenticate(ch.Challenge, proof);
                if (_token != null)
                {
                    _loginName = loginName;
                    return true;
                }
            }
            catch
            {
                // Swallow all exceptions and return false
            }
            return false;
        }

        /// <summary>
        /// Logs into Zenfolio API
        /// </summary>
        /// <param name="loginName">User's login name</param>
        /// <param name="password">User's password</param>
        /// <returns>True if login was successful, false otherwise.</returns>
        public bool LoginPlain(string loginName, string password)
        {
            try
            {
                _token = this.AuthenticatePlain(loginName, password);
                if (_token != null)
                {
                    _loginName = loginName;
                    return true;
                }
            }
            catch
            {
                // Swallow all exceptions and return false
            }
            return false;
        }

        /// <summary>
        /// Creates a WebRequest instance for the specified URI.
        /// Overriden to provide authentication header.
        /// </summary>
        /// <param name="uri">The URI to use when creating the WebRequest.</param>
        /// <returns>The WebRequest instance.</returns>
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest wr = base.GetWebRequest(uri);
            if (_token != null)
                wr.Headers.Add("X-Zenfolio-Token", _token);
            return wr;
        }
    }
}
