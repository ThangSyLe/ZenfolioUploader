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
using Serilog;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

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
        static void Main(string[] args) 
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();

            try
            {
                Log.Verbose("Main() Started");
                Log.Debug("args[{Args}]", JsonConvert.SerializeObject(args));

                Log.Information("----------- Application Started ----------- ");

                string mimeType = "image/jpeg";

                bool loggedin = false;
                _client = new ZenfolioClient();


                while (!loggedin)
                {
                    Log.Debug("Attempting to log in as [{Login}]", Settings.Default.ZenfolioUserName);
                    loggedin = _client.Login(Settings.Default.ZenfolioUserName, Settings.Default.ZenfolioPassword);
                }

                //Load more detailed projection of selected gallery
                PhotoSet gallery = new PhotoSet();
                if (!string.IsNullOrEmpty(Settings.Default.ZenfolioGalleryID))
                {
                    gallery = _client.LoadPhotoSet(Convert.ToInt64(Settings.Default.ZenfolioGalleryID), InformatonLevel.Level1, true);
                }
                else
                {
                    BrowseDialog dlgBrowse = new BrowseDialog(_client);
                    DialogResult dlgResult = dlgBrowse.ShowDialog();

                    gallery = _client.LoadPhotoSet(dlgBrowse.SelectedGallery.Id, InformatonLevel.Level1, true);
                    Log.Information("Loaded Gallery [{Title}]", gallery.Title);
                }
                
                Log.Debug("Found images [{ImageCount}] in Zenfolio GalleryID[{GalleryID}]", gallery.PhotoCount, gallery.Id);
                
                while (true)
                {
                    try
                    {
                        var imagePaths = Directory.GetFiles(Settings.Default.ImageFolderPath);
                        Log.Debug("Found images [{ImagesCount}] in directory[{Directory}]", imagePaths.Length, Settings.Default.ImageFolderPath);

                        var newImageFileInfos = GetNewImageFileInfos(gallery, imagePaths);

                        if (newImageFileInfos.Count() > 0)
                        {
                            UploadImages(newImageFileInfos, gallery, mimeType);
                        }

                        imagePaths = Directory.GetFiles(Settings.Default.ImageFolderPath);
                        newImageFileInfos = GetNewImageFileInfos(gallery, imagePaths);

                        Thread.Sleep(Settings.Default.WaitTimeInSec * 1000);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, ex.Message);
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(e, e.Message);
            }
        }

        private static List<FileInfo> GetNewImageFileInfos(PhotoSet gallery, string[] imagePaths)
        {
            var imageFileInfos = new List<FileInfo>();
            foreach (var imagePath in imagePaths)
            {
                var imageFileinfo = new FileInfo(imagePath);

                if (!gallery.Photos.Any(p => p.FileName == imageFileinfo.Name))
                {
                    imageFileInfos.Add(imageFileinfo);
                }
            }

            return imageFileInfos;
        }

        private static void UploadImages(List<FileInfo> imageFileInfos, PhotoSet gallery, string mimeType)
        {
            Log.Debug("New images to upload count[{ImagesCount}]", imageFileInfos.Count());

            foreach (var imageFileInfo in imageFileInfos)
            {
                Log.Debug("Attempting to upload file[{ImagePath}]", imageFileInfo.Name);

                if (!gallery.Photos.Any(photos => imageFileInfo.Name == photos.FileName))
                {
                    UploadDialog dlgUpload = new UploadDialog(_client, gallery, imageFileInfo.FullName, mimeType);

                    var dlgResult = dlgUpload.ShowDialog();

                    var galleryPhotos = gallery.Photos.ToList();

                    galleryPhotos.Add(new Photo
                    {
                        FileName = imageFileInfo.Name
                    });

                    gallery.Photos = galleryPhotos.ToArray();

                    Log.Information("Uploaded file[{ImageFileName}] successfully", imageFileInfo.Name);
                }
                else
                {
                    Log.Debug("File[{FileName}] already uploaded", imageFileInfo.Name);
                }
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
