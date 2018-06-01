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
// "Browse for Gallery" window.
//

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Configuration;

using Zenfolio.Examples.Uploader.ZfApiRef;

namespace Zenfolio.Examples.Uploader
{
    /// <summary>
    /// Gallery browser form
    /// </summary>
    public class BrowseDialog : System.Windows.Forms.Form
    {
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.TreeView _tvGroups;
        private System.Windows.Forms.ImageList _ilGroups;
        private ZenfolioClient _client;
        private System.Windows.Forms.Label _lblAction;
        private System.Windows.Forms.Button _btnNewGroup;
        private System.Windows.Forms.Button _btnNewGallery;
        private System.Windows.Forms.Button _btnOk;
        private System.Windows.Forms.Button _btnCancel;
        public PhotoSet SelectedGallery;

        /// <summary>
        /// Tree node types enumeration
        /// </summary>
        public enum NodeType {
            Root = 0,
            Group,
            Gallery,
            Collection,
            Photo
        }

        public BrowseDialog()
        {
            InitializeComponent();
        }

        public BrowseDialog(ZenfolioClient client)
        {
            _client = client;
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
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(BrowseDialog));
        this._tvGroups = new System.Windows.Forms.TreeView();
        this._ilGroups = new System.Windows.Forms.ImageList(this.components);
        this._lblAction = new System.Windows.Forms.Label();
        this._btnNewGroup = new System.Windows.Forms.Button();
        this._btnNewGallery = new System.Windows.Forms.Button();
        this._btnOk = new System.Windows.Forms.Button();
        this._btnCancel = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // _tvGroups
        // 
        this._tvGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
        this._tvGroups.HideSelection = false;
        this._tvGroups.HotTracking = true;
        this._tvGroups.ImageList = this._ilGroups;
        this._tvGroups.Indent = 19;
        this._tvGroups.ItemHeight = 18;
        this._tvGroups.LabelEdit = true;
        this._tvGroups.Location = new System.Drawing.Point(16, 64);
        this._tvGroups.Name = "_tvGroups";
        this._tvGroups.ShowLines = false;
        this._tvGroups.ShowRootLines = false;
        this._tvGroups.Size = new System.Drawing.Size(410, 256);
        this._tvGroups.TabIndex = 2;
        this._tvGroups.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnGroupsKeyDown);
        this._tvGroups.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnGroupsSelect);
        this._tvGroups.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.OnGroupsAfterLabelEdit);
        this._tvGroups.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.OnGroupsCollapse);
        this._tvGroups.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.OnGroupsBeforeLabelEdit);
        // 
        // _ilGroups
        // 
        this._ilGroups.ImageSize = new System.Drawing.Size(16, 16);
        this._ilGroups.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_ilGroups.ImageStream")));
        this._ilGroups.TransparentColor = System.Drawing.Color.Transparent;
        // 
        // _lblAction
        // 
        this._lblAction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
        this._lblAction.Location = new System.Drawing.Point(16, 16);
        this._lblAction.Name = "_lblAction";
        this._lblAction.Size = new System.Drawing.Size(410, 24);
        this._lblAction.TabIndex = 1;
        this._lblAction.Text = "Select a gallery";
        // 
        // _btnNewGroup
        // 
        this._btnNewGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this._btnNewGroup.Location = new System.Drawing.Point(16, 336);
        this._btnNewGroup.Name = "_btnNewGroup";
        this._btnNewGroup.Size = new System.Drawing.Size(112, 23);
        this._btnNewGroup.TabIndex = 3;
        this._btnNewGroup.Text = "Make New G&roup";
        this._btnNewGroup.Click += new System.EventHandler(this.OnNewGroup);
        // 
        // _btnNewGallery
        // 
        this._btnNewGallery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this._btnNewGallery.Location = new System.Drawing.Point(136, 336);
        this._btnNewGallery.Name = "_btnNewGallery";
        this._btnNewGallery.Size = new System.Drawing.Size(112, 23);
        this._btnNewGallery.TabIndex = 4;
        this._btnNewGallery.Text = "Make New &Gallery";
        this._btnNewGallery.Click += new System.EventHandler(this.OnNewGallery);
        // 
        // _btnOk
        // 
        this._btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
        this._btnOk.Location = new System.Drawing.Point(274, 336);
        this._btnOk.Name = "_btnOk";
        this._btnOk.TabIndex = 5;
        this._btnOk.Text = "OK";
        // 
        // _btnCancel
        // 
        this._btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this._btnCancel.Location = new System.Drawing.Point(354, 336);
        this._btnCancel.Name = "_btnCancel";
        this._btnCancel.TabIndex = 6;
        this._btnCancel.Text = "Cancel";
        // 
        // BrowseDialog
        // 
        this.AcceptButton = this._btnOk;
        this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
        this.CancelButton = this._btnCancel;
        this.ClientSize = new System.Drawing.Size(442, 373);
        this.Controls.Add(this._btnCancel);
        this.Controls.Add(this._btnOk);
        this.Controls.Add(this._btnNewGallery);
        this.Controls.Add(this._btnNewGroup);
        this.Controls.Add(this._lblAction);
        this.Controls.Add(this._tvGroups);
        this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.MinimumSize = new System.Drawing.Size(450, 400);
        this.Name = "BrowseDialog";
        this.ShowInTaskbar = false;
        this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Browse for Gallery";
        this.Load += new System.EventHandler(this.OnLoad);
        this.ResumeLayout(false);

    }
    #endregion

        /// <summary>
        /// Occurs before a form is displayed for the first time.
        /// Makes all necessary initializations.
        /// </summary>
        private void OnLoad(object sender, EventArgs e)
        {
            // Load own profile
            User user = _client.LoadPrivateProfile();

            // Load and wrap Groups hierarchy
            Group rootGroup = _client.LoadGroupHierarchy(user.LoginName);
            TreeNode rootNode = CreateGroupNode(rootGroup);

            // Fix-up the root node of the Group hierarchy
            rootNode.ImageIndex = (int)NodeType.Root;
            rootNode.SelectedImageIndex = rootNode.ImageIndex;
            rootNode.Text = user.DisplayName;

            // Keep root node always expanded
            rootNode.Expand();

            _tvGroups.Nodes.Add(rootNode);

        }

        /// <summary>
        /// Creates TreeNode representing Group element.
        /// </summary>
        /// <param name="element">Group element to wrap.</param>
        /// <returns>Constructed tree node.</returns>
        private TreeNode CreateGroupNode(GroupElement element)
        {
            TreeNode ret = new TreeNode(element.Title);
            ret.Tag = element;

            if (element is Group)
            {
                Group group = element as Group;
                ret.ImageIndex = (int)NodeType.Group;

                // Process elements recursively
                if (group.Elements != null)
                {
                    foreach(GroupElement child in group.Elements)
                        ret.Nodes.Add(CreateGroupNode(child));
                }
            }
            else 
            {
                //It's a photoset
                PhotoSet ps = element as PhotoSet ;
                ret.ImageIndex = ps.Type == PhotoSetType.Gallery ?
                    (int)NodeType.Gallery : (int)NodeType.Collection;
            }

            ret.SelectedImageIndex = ret.ImageIndex;
            return ret;
        }

        /// <summary>
        /// Called when user tries to collapse a tree node.
        /// Blocks collapsing of the root node.
        /// </summary>
        /// <param name="sender">Event sender (treeview)</param>
        /// <param name="e">Event arguments</param>
        private void OnGroupsCollapse(object sender, TreeViewCancelEventArgs e)
        {
            NodeType nt = (NodeType)e.Node.ImageIndex;
            e.Cancel = (nt == NodeType.Root);
        }

        /// <summary>
        /// Called when user selects a tree node in the tree view.
        /// Checks selected node type and enables appropriate UI elements.
        /// </summary>
        /// <param name="sender">Event sender (treeview)</param>
        /// <param name="e">Event arguments</param>
        private void OnGroupsSelect(object sender, TreeViewEventArgs e)
        {
            // Check node type and enable/disable appropriate UI
            NodeType nt = (NodeType)e.Node.ImageIndex;
            switch(nt)
            {
                case NodeType.Gallery:
                    _btnOk.Enabled = true;
                    _btnNewGroup.Enabled = false;
                    _btnNewGallery.Enabled = false;
                    SelectedGallery = e.Node.Tag as PhotoSet;
                    break;
                case NodeType.Root:
                case NodeType.Group:
                        _btnOk.Enabled = false;
                    _btnNewGroup.Enabled = true;
                    _btnNewGallery.Enabled = true;
                    break;
                default:
                    _btnOk.Enabled = false;
                    _btnNewGroup.Enabled = false;
                    _btnNewGallery.Enabled = false;
                    break;
            }
        }

        /// <summary>
        /// Called when user presses "Make New Group" button.
        /// Creates new Group under selected Group.
        /// </summary>
        /// <param name="sender">Event sender (button)</param>
        /// <param name="e">Event arguments</param>
        private void OnNewGroup(object sender, EventArgs e)
        {
            // Extract current context
            TreeNode currentNode = _tvGroups.SelectedNode;
            Group parent = (Group)currentNode.Tag;
            GroupUpdater updater = new GroupUpdater();
            updater.Title = "New Group";

            // Create Group
            Group newGroup = _client.CreateGroup(parent.Id, updater);

            // Attach new Group to the tree and start edit
            TreeNode newNode = CreateGroupNode(newGroup);
            currentNode.Nodes.Add(newNode);
            currentNode.Expand();
            newNode.BeginEdit();
        }

        /// <summary>
        /// Called when user presses "Make New Gallery" button.
        /// Creates new gallery under selected Group.
        /// </summary>
        /// <param name="sender">Event sender (button)</param>
        /// <param name="e">Event arguments</param>
        private void OnNewGallery(object sender, EventArgs e)
        {
            // Extract current context
            TreeNode currentNode = _tvGroups.SelectedNode;
            Group parent = (Group)currentNode.Tag;
            PhotoSetUpdater updater = new PhotoSetUpdater();
            updater.Title = "New Gallery";

            // Create gallery
            PhotoSet newGallery = _client.CreatePhotoSet(parent.Id, 
            PhotoSetType.Gallery, updater);

            // Attach new gallery to the tree and start edit
            TreeNode newNode = CreateGroupNode(newGallery);
            currentNode.Nodes.Add(newNode);
            currentNode.Expand();
            newNode.BeginEdit();
        }
    
        /// <summary>
        /// Called when user finishes editing tree label.
        /// Updates edited element title.
        /// </summary>
        /// <param name="sender">Event sender (treeview)</param>
        /// <param name="e">Event arguments</param>
        private void OnGroupsAfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            // Check that label is changed and valid
            if (e.Label == null || e.Label.Length == 0 || e.Label.Equals(e.Node.Text))
            {
                e.CancelEdit = true;
                return;
            }

            // Update edited element with new title
            GroupElement element = e.Node.Tag as GroupElement;
            NodeType nt = (NodeType)e.Node.ImageIndex;

            // Select appropriate updater and method
            if (nt == NodeType.Group)
            {
                GroupUpdater updater = new GroupUpdater();
                updater.Title = e.Label;
                element = _client.UpdateGroup(element.Id, updater);
            }
            else
            {
                // It could be either collection or gallery here
                PhotoSetUpdater updater = new PhotoSetUpdater();
                updater.Title = e.Label;
                element = _client.UpdatePhotoSet(element.Id, updater);
            }	
    
            // Store updated snapshot
            e.Node.Tag = element;
        }

        /// <summary>
        /// Called when user attempts editing tree label.
        /// Blocks editing for root element.
        /// </summary>
        /// <param name="sender">Event sender (treeview)</param>
        /// <param name="e">Event arguments</param>
        private void OnGroupsBeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            // do not allow to edit root node
            NodeType nt = (NodeType)e.Node.ImageIndex;
            e.CancelEdit = (nt == NodeType.Root);
        }

        /// <summary>
        /// Called when user presses a key.
        /// Intercepts F2 to start label edit to mimic shell behavior.
        /// </summary>
        /// <param name="sender">Event sender (treeview)</param>
        /// <param name="e">Event arguments</param>
        private void OnGroupsKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F2 && e.Modifiers == Keys.None)
            {
                // Try to edit current node if F2 is pressed
                _tvGroups.SelectedNode.BeginEdit();
                e.Handled = true;
            }
        }
    }
}
