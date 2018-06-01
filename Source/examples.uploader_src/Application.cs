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
// Uploader application.
//

using System;
using System.Windows.Forms;
using System.IO;

using Zenfolio.Examples.Uploader.ZfApiRef;
using Zenfolio.Examples.Uploader.Properties;

namespace Zenfolio.Examples.Uploader
{
    /// <summary>
    /// Uploader application class
    /// </summary>
    public class Application
    {
        private static ZenfolioClient _client;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args) 
        {
            try
            {
                string mimeType = CheckCommandLine(args);
                if (mimeType == null)
                    return 2;

                LoginDialog dlgLogin = new LoginDialog();
                _client = new ZenfolioClient();
        
                // Try to login repeatedly
                bool loggedin = false;
                while(!loggedin)
                {
                    // Try to login
                    string login = "contact@enchantedpix.com"; // dlgLogin._txtLogin.Text;
                    string password = "Ph0t0graphy!";          // dlgLogin._txtPassword.Text;
                    loggedin = _client.Login(login, password);
                }

                //Load more detailed projection of selected gallery
                var gunghoGallery = 675105122405010023;

                PhotoSet gallery = _client.LoadPhotoSet(gunghoGallery, InformatonLevel.Level1, false);

                var images = Directory.GetFiles(Settings.Default.ImageFolderPath);

                foreach (var image in images)
                {
                    UploadDialog dlgUpload = new UploadDialog(_client, gallery, image, mimeType);
                    var dlgResult = dlgUpload.ShowDialog();
                    if (dlgResult != DialogResult.OK)
                        return 1;
                }

                return 0;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString(), "Exception");
                return 2;
            }
        }

        /// <summary>
        /// Checks if command-line parameters are valid
        /// </summary>
        /// <param name="args">Command line parameters array</param>
        /// <returns>MIME type of the image to be uploaded, null otherwise</returns>
        private static string CheckCommandLine(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                MessageBox.Show("Missing command line parameters", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            if (args.Length > 1)
            {
                MessageBox.Show("Can upload only one image at a time", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            try
            {
                if (!System.IO.File.Exists(args[0]))
                {
                    MessageBox.Show("File doesn't exist", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                FileInfo fi = new FileInfo(args[0]);
                if ((fi.Attributes & FileAttributes.Directory) != 0)
                {
                    MessageBox.Show("Cannot upload directories", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                switch (fi.Extension.ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                    case ".jpe":
                    case ".jfif":
                    case ".jfi":
                    case ".jif":
                        return "image/jpeg";

                    case ".tiff":
                    case ".tif":
                        return "image/tiff";

                    case ".png":
                        return "image/png";

                    case ".gif":
                        return "image/gif";

                    default:
                        MessageBox.Show("Unsupported file type", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                }
            }
            catch
            {
                MessageBox.Show("Can't read image file", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }	    
        }
    }
}
