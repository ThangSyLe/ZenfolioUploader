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
// Login dialog. Asks for username and password.
//

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Zenfolio.Examples.Uploader
{
    /// <summary>
    /// Login dialog to retrieve username and password
    /// </summary>
    public class LoginDialog : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Label _lblLogin;
        internal System.Windows.Forms.TextBox _txtLogin;
        private System.Windows.Forms.Label _lblPassword;
        internal System.Windows.Forms.TextBox _txtPassword;
        private System.Windows.Forms.Button _btnOk;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.PictureBox _icon;

	    /// <summary>
	    /// Required designer variable.
	    /// </summary>
	    private System.ComponentModel.Container components = null;

	    public LoginDialog()
	    {
	        //
	        // Required for Windows Form Designer support
	        //
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
	    System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(LoginDialog));
	    this._lblLogin = new System.Windows.Forms.Label();
	    this._txtLogin = new System.Windows.Forms.TextBox();
	    this._lblPassword = new System.Windows.Forms.Label();
	    this._txtPassword = new System.Windows.Forms.TextBox();
	    this._btnOk = new System.Windows.Forms.Button();
	    this._btnCancel = new System.Windows.Forms.Button();
	    this._icon = new System.Windows.Forms.PictureBox();
	    this.SuspendLayout();
	    // 
	    // _lblLogin
	    // 
	    this._lblLogin.Location = new System.Drawing.Point(88, 16);
	    this._lblLogin.Name = "_lblLogin";
	    this._lblLogin.Size = new System.Drawing.Size(72, 23);
	    this._lblLogin.TabIndex = 0;
	    this._lblLogin.Text = "&Login Name:";
	    this._lblLogin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
	    // 
	    // _txtLogin
	    // 
	    this._txtLogin.Location = new System.Drawing.Point(168, 16);
	    this._txtLogin.Name = "_txtLogin";
	    this._txtLogin.Size = new System.Drawing.Size(128, 20);
	    this._txtLogin.TabIndex = 1;
	    this._txtLogin.Text = "";
	    // 
	    // _lblPassword
	    // 
	    this._lblPassword.Location = new System.Drawing.Point(96, 56);
	    this._lblPassword.Name = "_lblPassword";
	    this._lblPassword.Size = new System.Drawing.Size(64, 23);
	    this._lblPassword.TabIndex = 2;
	    this._lblPassword.Text = "&Password:";
	    this._lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
	    // 
	    // _txtPassword
	    // 
	    this._txtPassword.Location = new System.Drawing.Point(168, 56);
	    this._txtPassword.Name = "_txtPassword";
	    this._txtPassword.PasswordChar = '*';
	    this._txtPassword.Size = new System.Drawing.Size(128, 20);
	    this._txtPassword.TabIndex = 3;
	    this._txtPassword.Text = "";
	    // 
	    // _btnOk
	    // 
	    this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
	    this._btnOk.Location = new System.Drawing.Point(136, 96);
	    this._btnOk.Name = "_btnOk";
	    this._btnOk.TabIndex = 4;
	    this._btnOk.Text = "&OK";
	    // 
	    // _btnCancel
	    // 
	    this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
	    this._btnCancel.Location = new System.Drawing.Point(224, 96);
	    this._btnCancel.Name = "_btnCancel";
	    this._btnCancel.TabIndex = 5;
	    this._btnCancel.Text = "&Cancel";
	    // 
	    // _icon
	    // 
	    this._icon.Image = ((System.Drawing.Image)(resources.GetObject("_icon.Image")));
	    this._icon.Location = new System.Drawing.Point(16, 16);
	    this._icon.Name = "_icon";
	    this._icon.Size = new System.Drawing.Size(64, 64);
	    this._icon.TabIndex = 6;
	    this._icon.TabStop = false;
	    // 
	    // LoginDialog
	    // 
	    this.AcceptButton = this._btnOk;
	    this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
	    this.CancelButton = this._btnCancel;
	    this.ClientSize = new System.Drawing.Size(312, 133);
	    this.Controls.Add(this._icon);
	    this.Controls.Add(this._btnCancel);
	    this.Controls.Add(this._btnOk);
	    this.Controls.Add(this._txtPassword);
	    this.Controls.Add(this._lblPassword);
	    this.Controls.Add(this._txtLogin);
	    this.Controls.Add(this._lblLogin);
	    this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
	    this.MaximizeBox = false;
	    this.MinimizeBox = false;
	    this.Name = "LoginDialog";
	    this.ShowInTaskbar = false;
	    this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
	    this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
	    this.Text = "Login";
	    this.TopMost = true;
	    this.ResumeLayout(false);

	}
	#endregion

    }
}
