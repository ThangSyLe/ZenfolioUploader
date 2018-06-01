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
// Upload dialog.
//

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;

using Zenfolio.Examples.Uploader.ZfApiRef;

namespace Zenfolio.Examples.Uploader
{
    /// <summary>
    /// Uploading dialog
    /// </summary>
    public class UploadDialog : System.Windows.Forms.Form
    {
        private Thread _uploadThread;
        private ZenfolioClient _client;
        private PhotoSet _gallery;
        private FileInfo _fileInfo;
        private string _mimeType;
        private int _chunkNumber = 0;
        private int _totalChunks;
        private DateTime _startTime;

        private System.Windows.Forms.Label _lblUploading;
        private System.Windows.Forms.CheckBox _checkCloseOnComplete;
        private System.Windows.Forms.Button _btnView;
        private System.Windows.Forms.Button _btnViewGallery;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.ProgressBar _progress;
        private System.Windows.Forms.Label _lblFileName;
        private System.Windows.Forms.Label _lblDestination;
        private System.Windows.Forms.Label _lblTransferRate;
        private System.Windows.Forms.Label _lblStatic1;
        private System.Windows.Forms.Label _lblStatic2;

        /// <summary>
        /// Delegate for handling exceptions from upload thread
        /// </summary>
        private delegate void ExceptionHandler(Exception e);

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Creates form instance. For form designer only.
        /// </summary>
        public UploadDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Creates form instance.
        /// </summary>
        /// <param name="client">Zenfolio client to use for operations.</param>
        /// <param name="gallery">Destination gallery for uploading.</param>
        /// <param name="filePath">Path of the image to be uploaded.</param>
        /// <param name="mimeType">MIME type of the image.</param>
        public UploadDialog(
            ZenfolioClient client, 
            PhotoSet gallery, 
            string filePath, 
            string mimeType
            )
        {
            _client = client;
            _gallery = gallery;
            _fileInfo = new FileInfo(filePath);
            _totalChunks = (int) _fileInfo.Length / 1024;

            _mimeType = mimeType;
            InitializeComponent();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(UploadDialog));
        this._lblUploading = new System.Windows.Forms.Label();
        this._progress = new System.Windows.Forms.ProgressBar();
        this._lblStatic1 = new System.Windows.Forms.Label();
        this._lblStatic2 = new System.Windows.Forms.Label();
        this._checkCloseOnComplete = new System.Windows.Forms.CheckBox();
        this._btnView = new System.Windows.Forms.Button();
        this._btnViewGallery = new System.Windows.Forms.Button();
        this._btnCancel = new System.Windows.Forms.Button();
        this._lblFileName = new System.Windows.Forms.Label();
        this._lblDestination = new System.Windows.Forms.Label();
        this._lblTransferRate = new System.Windows.Forms.Label();
        this.SuspendLayout();
        // 
        // _lblUploading
        // 
        this._lblUploading.Location = new System.Drawing.Point(8, 32);
        this._lblUploading.Name = "_lblUploading";
        this._lblUploading.Size = new System.Drawing.Size(344, 16);
        this._lblUploading.TabIndex = 0;
        this._lblUploading.Text = "Uploading:";
        // 
        // _progress
        // 
        this._progress.Location = new System.Drawing.Point(8, 64);
        this._progress.Name = "_progress";
        this._progress.Size = new System.Drawing.Size(344, 16);
        this._progress.TabIndex = 1;
        // 
        // _lblStatic1
        // 
        this._lblStatic1.Location = new System.Drawing.Point(8, 112);
        this._lblStatic1.Name = "_lblStatic1";
        this._lblStatic1.Size = new System.Drawing.Size(64, 16);
        this._lblStatic1.TabIndex = 2;
        this._lblStatic1.Text = "Upload to:";
        // 
        // _lblStatic2
        // 
        this._lblStatic2.Location = new System.Drawing.Point(8, 128);
        this._lblStatic2.Name = "_lblStatic2";
        this._lblStatic2.Size = new System.Drawing.Size(72, 16);
        this._lblStatic2.TabIndex = 4;
        this._lblStatic2.Text = "Transfer rate:";
        // 
        // _checkCloseOnComplete
        // 
        this._checkCloseOnComplete.Location = new System.Drawing.Point(8, 152);
        this._checkCloseOnComplete.Name = "_checkCloseOnComplete";
        this._checkCloseOnComplete.Size = new System.Drawing.Size(264, 16);
        this._checkCloseOnComplete.TabIndex = 6;
        this._checkCloseOnComplete.Text = "&Close this dialog box when upload completes";
        // 
        // _btnView
        // 
        this._btnView.Enabled = false;
        this._btnView.Location = new System.Drawing.Point(96, 200);
        this._btnView.Name = "_btnView";
        this._btnView.Size = new System.Drawing.Size(80, 23);
        this._btnView.TabIndex = 7;
        this._btnView.Text = "&View";
        this._btnView.Click += new System.EventHandler(this.OnView);
        // 
        // _btnViewGallery
        // 
        this._btnViewGallery.Enabled = false;
        this._btnViewGallery.Location = new System.Drawing.Point(184, 200);
        this._btnViewGallery.Name = "_btnViewGallery";
        this._btnViewGallery.Size = new System.Drawing.Size(80, 23);
        this._btnViewGallery.TabIndex = 8;
        this._btnViewGallery.Text = "View &Gallery";
        this._btnViewGallery.Click += new System.EventHandler(this.OnViewGallery);
        // 
        // _btnCancel
        // 
        this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this._btnCancel.Location = new System.Drawing.Point(272, 200);
        this._btnCancel.Name = "_btnCancel";
        this._btnCancel.Size = new System.Drawing.Size(80, 23);
        this._btnCancel.TabIndex = 9;
        this._btnCancel.Text = "Cancel";
        this._btnCancel.Click += new System.EventHandler(this.OnCancel);
        // 
        // _lblFileName
        // 
        this._lblFileName.Location = new System.Drawing.Point(8, 48);
        this._lblFileName.Name = "_lblFileName";
        this._lblFileName.Size = new System.Drawing.Size(344, 16);
        this._lblFileName.TabIndex = 10;
        // 
        // _lblDestination
        // 
        this._lblDestination.Location = new System.Drawing.Point(80, 112);
        this._lblDestination.Name = "_lblDestination";
        this._lblDestination.Size = new System.Drawing.Size(272, 16);
        this._lblDestination.TabIndex = 3;
        // 
        // _lblTransferRate
        // 
        this._lblTransferRate.Location = new System.Drawing.Point(80, 128);
        this._lblTransferRate.Name = "_lblTransferRate";
        this._lblTransferRate.Size = new System.Drawing.Size(272, 16);
        this._lblTransferRate.TabIndex = 5;
        // 
        // UploadDialog
        // 
        this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
        this.CancelButton = this._btnCancel;
        this.ClientSize = new System.Drawing.Size(360, 237);
        this.Controls.Add(this._lblTransferRate);
        this.Controls.Add(this._lblDestination);
        this.Controls.Add(this._lblFileName);
        this.Controls.Add(this._btnCancel);
        this.Controls.Add(this._btnViewGallery);
        this.Controls.Add(this._btnView);
        this.Controls.Add(this._checkCloseOnComplete);
        this.Controls.Add(this._lblStatic2);
        this.Controls.Add(this._lblStatic1);
        this.Controls.Add(this._progress);
        this.Controls.Add(this._lblUploading);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        this.MaximizeBox = false;
        this.Name = "UploadDialog";
        this.Text = "Connecting...";
        this.Load += new System.EventHandler(this.OnLoad);
        this.ResumeLayout(false);

    }
    #endregion

        /// <summary>
        /// Builds upload url
        /// </summary>
        /// <returns>Url to use for image upload.</returns>
        private string BuildUrl(FileInfo fileInfo)
        {
            // append query parameters that describe the file being uploaded
            // to the base upload URL
            return String.Format("{0}?filename={1}", _gallery.UploadUrl,
                                 HttpUtility.UrlEncode(fileInfo.Name));
        }

        /// <summary>
        /// Uploading procedure
        /// </summary>
        public void UploadProc() 
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(BuildUrl(_fileInfo));
            req.AllowWriteStreamBuffering = false;
            req.Method = "POST";
        
            // put correct user token in request headers
            req.Headers.Add("X-Zenfolio-Token", _client.Token);
            req.ContentType = _mimeType;
            req.ContentLength = _fileInfo.Length;

            // Prepare to read the file and push it into request stream
            BinaryReader fileReader = new BinaryReader(
            new FileStream(_fileInfo.FullName, FileMode.Open));
            Stream requestStream = req.GetRequestStream();

            // Upload the data
            try
            {
                // Create a buffer for image data
                const int bufSize = 1024;
                byte[] buffer = new byte[bufSize];
                int chunkLength = 0;

                // Transfer data
                while ((chunkLength = fileReader.Read(buffer, 0, bufSize)) > 0)
                {
                    IAsyncResult result = requestStream
                        .BeginWrite(buffer, 0, chunkLength, null, null);

                    //Enter sleep state for Thread.Interrupt() to work
                    result.AsyncWaitHandle.WaitOne();
                    requestStream.EndWrite(result);

                    //Notify UI
                    this.Invoke(new MethodInvoker(this.OnProgress));
                }
            }
            catch (ThreadInterruptedException)
            {
                // User aborted
                return;
            }
            finally
            {
                // Make sure that streams are closed
                try
                {
                    fileReader.Close();
                    requestStream.Close();
                }
                catch
                {
                }
            }

            try
            {
                // Read image ID from the response
                WebResponse response = req.GetResponse();
                TextReader responseReader = new StreamReader(response.GetResponseStream());

                string imageId = responseReader.ReadToEnd();

                //TODO load photo and construct url for View button
                //_client.LoadPhoto(id);

                // Inform UI that upload finished
                this.Invoke(new MethodInvoker(this.OnComplete));
            }
            catch (Exception e)
            {
                this.Invoke(new ExceptionHandler(this.OnError), new object[]{e});
            }
        }

        /// <summary>
        /// Called when form loaded.
        /// Performs initialization and strarts upload thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoad(object sender, EventArgs e)
        {
            _lblFileName.Text = _fileInfo.Name;
            _lblDestination.Text = _gallery.Title;

            _uploadThread = new Thread(new ThreadStart(this.UploadProc));
            _uploadThread.Start();
            _startTime = DateTime.Now;
        }

        /// <summary>
        /// Called when user presses Cancel button.
        /// Interrupts uploading thread and closes dialog.
        /// </summary>
        /// <param name="sender">Event sender (button)</param>
        /// <param name="e">Event arguments</param>
        private void OnCancel(object sender, EventArgs e)
        {
            _uploadThread.Interrupt();
        }

        /// <summary>
        /// Called when user presses View button.
        /// Opens uploaded image in default browser.
        /// </summary>
        /// <param name="sender">Event sender (button)</param>
        /// <param name="e">Event arguments</param>
        private void OnView(object sender, EventArgs e)
        {
            //TODO: Compute picture url and show in browser
        }

        /// <summary>
        /// Called when user presses View Gallery button.
        /// Opens gallery which has been used to upload the image.
        /// </summary>
        /// <param name="sender">Event sender (button)</param>
        /// <param name="e">Event arguments</param>
        private void OnViewGallery(object sender, EventArgs e)
        {
            // Compute gallery url
            string url = String.Format("{0}/{1}/p{2}/edit", 
                _client.Authority, _client.LoginName, _gallery.Id);

            // Open it in default browser
            Process.Start(url);
        }

        /// <summary>
        /// Called by the uploader thread to report end of operation.
        /// </summary>
        private void OnComplete()
        {
            //Upload is complete now, update UI
            _btnView.Enabled = true;
            _btnViewGallery.Enabled = true;
            _btnCancel.Enabled = false;

            if (_checkCloseOnComplete.Checked)
            {
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        /// <summary>
        /// Called by the uploader thread to report an error.
        /// It shows an error message and closes the dialog.
        /// </summary>
        /// <param name="e">Exception that occurred in uploading thread.</param>
        private void OnError(Exception e)
        {
            _btnCancel.Enabled = false;

            //Display error message
            MessageBox.Show(e.ToString(),"Exception", 
            MessageBoxButtons.OK, MessageBoxIcon.Error);

            //Close dialog
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Called by the uploader thread to report progress.
        /// It's called once per 1024 bytes of data transferred.
        /// </summary>
        private void OnProgress()
        {
            //Calculate progress
            _chunkNumber += 1;
            int percentComplete = _chunkNumber * 100 / _totalChunks;
            string caption = String.Format("{0}% of {1} Completed", 
            percentComplete, _fileInfo.Name);
            TimeSpan elapsed = DateTime.Now - _startTime;
        
            double rate = _chunkNumber * 1000000L / elapsed.TotalMilliseconds;
            string unit = "B";
            if(rate > 1000)
            {
                rate /= 1000;
                unit = "KB";
            }
            if(rate > 1000)
            {
                rate /= 1000;
                unit = "MB";
            }

            // Update progress
            _lblTransferRate.Text = String.Format("{0:#} {1}/Sec", rate, unit);
            _progress.Value = percentComplete;
            this.Text = caption;

        }
    }
}
