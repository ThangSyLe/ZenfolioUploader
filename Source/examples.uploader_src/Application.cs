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
using System.Net;
using System.Web;

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
                long galleryID = 0;
                string galleryName = "";

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

                    galleryID = gallery.Id;
                    galleryName = gallery.Title;
                }
                else
                {
                    BrowseDialog dlgBrowse = new BrowseDialog(_client);
                    DialogResult dlgResult = dlgBrowse.ShowDialog();

                    galleryID = dlgBrowse.SelectedGallery.Id;
                    galleryName = dlgBrowse.SelectedGallery.Title;

                    gallery = _client.LoadPhotoSet(galleryID, InformatonLevel.Level1, true);
                    Log.Information("Loaded Gallery [{Title}]", galleryName);
                }

                Log.Debug("Found images [{ImageCount}] in Zenfolio GalleryID[{GalleryID}]", gallery.PhotoCount, gallery.Id);

                var previousImageCount = 0;
                while (true)
                {
                    try
                    {
                        var imageFolderPath = Settings.Default.ImageFolderPath + $"//{galleryName}";

                        if (!Directory.Exists(imageFolderPath))
                        {
                            Directory.CreateDirectory(imageFolderPath);
                        }

                        var imagePaths = Directory.GetFiles(imageFolderPath);

                        if (imagePaths.Count() > previousImageCount)
                        {
                            Log.Debug("Found images [{ImagesCount}] in directory[{Directory}]", imagePaths.Length, imageFolderPath);

                            var newImageFileInfos = GetNewImageFileInfos(gallery, imagePaths);

                            if (newImageFileInfos.Count() > 0)
                            {
                                UploadImages(newImageFileInfos, gallery, mimeType);

                                newImageFileInfos = GetNewImageFileInfos(gallery, imagePaths);

                                previousImageCount = imagePaths.Count();
                            }

                        }

                        Thread.Sleep(Settings.Default.WaitTimeInSec * 1000);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, ex.Message);
                        Thread.Sleep(Settings.Default.WaitTimeInSec * 1000);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static bool IsFileLocked(string filePath)
        {
            try
            {
                using (System.IO.File.Open(filePath, FileMode.Open)) { }
            }
            catch (IOException e)
            {
                var errorCode = System.Runtime.InteropServices.Marshal.GetHRForException(e) & ((1 << 16) - 1);

                return errorCode == 32 || errorCode == 33;
            }

            return false;
        }

        private static List<FileInfo> GetNewImageFileInfos(PhotoSet gallery, string[] imagePaths)
        {
            var imageFileInfos = new List<FileInfo>();
            foreach (var imagePath in imagePaths)
            {
                if (IsFileLocked(imagePath))
                {
                    Log.Debug("File [{FilePath}] is locked so it's being ignored", imagePath);
                }
                else
                {
                    var imageFileinfo = new FileInfo(imagePath);

                    if (!gallery.Photos.Any(p => p.FileName == imageFileinfo.Name))
                    {
                        imageFileInfos.Add(imageFileinfo);
                    }
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
                    var photoId = UploadProc(imageFileInfo, gallery, mimeType);

                    if (!string.IsNullOrEmpty(Settings.Default.ZenfolioCollectionID))
                    {
                        _client.CollectionAddPhoto(Convert.ToInt64(Settings.Default.ZenfolioCollectionID), Convert.ToInt64(photoId));
                        Log.Information("Added PhotoId[{PhotoId} into CollectionId(CollectionId)]", photoId, Settings.Default.ZenfolioCollectionID);
                    }
                    
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

        /// <summary>
        /// Builds upload url
        /// </summary>
        /// <returns>Url to use for image upload.</returns>
        private static string BuildUrl(FileInfo fileInfo, PhotoSet gallery)
        {
            // append query parameters that describe the file being uploaded
            // to the base upload URL
            return String.Format("{0}?filename={1}", gallery.UploadUrl,
                                 HttpUtility.UrlEncode(fileInfo.Name));
        }

        /// <summary>
        /// Uploading procedure
        /// </summary>
        public static string UploadProc(FileInfo imageFile, PhotoSet gallery, string mimeType)
        {
            // Upload the data
            BinaryReader fileReader = null;
            Stream requestStream = null;

            try
            {

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(BuildUrl(imageFile, gallery));
                req.AllowWriteStreamBuffering = false;
                req.Method = "POST";

                // put correct user token in request headers
                req.Headers.Add("X-Zenfolio-Token", _client.Token);
                req.ContentType = mimeType;
                req.ContentLength = imageFile.Length;

                // Prepare to read the file and push it into request stream
                fileReader = new BinaryReader(new FileStream(imageFile.FullName, FileMode.Open));
                requestStream = req.GetRequestStream();


                // Create a buffer for image data
                const int bufSize = 1024;
                byte[] buffer = new byte[bufSize];
                int chunkLength = 0;

                // Transfer data
                while ((chunkLength = fileReader.Read(buffer, 0, bufSize)) > 0)
                {
                    requestStream.Write(buffer, 0, chunkLength);

                    //Enter sleep state for Thread.Interrupt() to work
                    //result.AsyncWaitHandle.WaitOne();
                    //requestStream.EndWrite(result);

                    //Notify UI
                    //this.Invoke(new MethodInvoker(this.OnProgress));
                }


                // Read image ID from the response
                WebResponse response = req.GetResponse();
                TextReader responseReader = new StreamReader(response.GetResponseStream());

                string imageId = responseReader.ReadToEnd();

                return imageId;
                //TODO load photo and construct url for View button
                //_client.LoadPhoto(id);

                // Inform UI that upload finished
                //this.Invoke(new MethodInvoker(this.OnComplete));
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                fileReader.Close();
                requestStream.Close();
            }
        }
    }
}
