namespace StereoEditor
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importleftrightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mPOToJPGStereoPairsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allMPOsInAFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allMPOsInAFolderToJPGStereoPairsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windoesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.lblLeftRightImgs = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.progBar = new System.Windows.Forms.ToolStripProgressBar();
            this.lblReady = new System.Windows.Forms.Label();
            this.lblConvFolder = new System.Windows.Forms.Label();
            this.lblOpenFiles = new System.Windows.Forms.Label();
            this.menuStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.convertToolStripMenuItem,
            this.windoesToolStripMenuItem,
            this.helpToolStripMenuItem});
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.MdiWindowListItem = this.windoesToolStripMenuItem;
            this.menuStrip.Name = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.importleftrightToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // importleftrightToolStripMenuItem
            // 
            this.importleftrightToolStripMenuItem.Name = "importleftrightToolStripMenuItem";
            resources.ApplyResources(this.importleftrightToolStripMenuItem, "importleftrightToolStripMenuItem");
            this.importleftrightToolStripMenuItem.Click += new System.EventHandler(this.importleftrightToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // convertToolStripMenuItem
            // 
            this.convertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cToolStripMenuItem,
            this.mPOToJPGStereoPairsToolStripMenuItem,
            this.allMPOsInAFolderToolStripMenuItem,
            this.allMPOsInAFolderToJPGStereoPairsToolStripMenuItem});
            this.convertToolStripMenuItem.Name = "convertToolStripMenuItem";
            resources.ApplyResources(this.convertToolStripMenuItem, "convertToolStripMenuItem");
            // 
            // cToolStripMenuItem
            // 
            this.cToolStripMenuItem.Name = "cToolStripMenuItem";
            resources.ApplyResources(this.cToolStripMenuItem, "cToolStripMenuItem");
            this.cToolStripMenuItem.Click += new System.EventHandler(this.cToolStripMenuItem_Click);
            // 
            // mPOToJPGStereoPairsToolStripMenuItem
            // 
            this.mPOToJPGStereoPairsToolStripMenuItem.Name = "mPOToJPGStereoPairsToolStripMenuItem";
            resources.ApplyResources(this.mPOToJPGStereoPairsToolStripMenuItem, "mPOToJPGStereoPairsToolStripMenuItem");
            this.mPOToJPGStereoPairsToolStripMenuItem.Click += new System.EventHandler(this.mPOToJPGStereoPairsToolStripMenuItem_Click);
            // 
            // allMPOsInAFolderToolStripMenuItem
            // 
            this.allMPOsInAFolderToolStripMenuItem.Name = "allMPOsInAFolderToolStripMenuItem";
            resources.ApplyResources(this.allMPOsInAFolderToolStripMenuItem, "allMPOsInAFolderToolStripMenuItem");
            this.allMPOsInAFolderToolStripMenuItem.Click += new System.EventHandler(this.allMPOsInAFolderToolStripMenuItem_Click);
            // 
            // allMPOsInAFolderToJPGStereoPairsToolStripMenuItem
            // 
            this.allMPOsInAFolderToJPGStereoPairsToolStripMenuItem.Name = "allMPOsInAFolderToJPGStereoPairsToolStripMenuItem";
            resources.ApplyResources(this.allMPOsInAFolderToJPGStereoPairsToolStripMenuItem, "allMPOsInAFolderToJPGStereoPairsToolStripMenuItem");
            this.allMPOsInAFolderToJPGStereoPairsToolStripMenuItem.Click += new System.EventHandler(this.allMPOsInAFolderToJPGStereoPairsToolStripMenuItem_Click);
            // 
            // windoesToolStripMenuItem
            // 
            this.windoesToolStripMenuItem.Name = "windoesToolStripMenuItem";
            resources.ApplyResources(this.windoesToolStripMenuItem, "windoesToolStripMenuItem");
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // lblLeftRightImgs
            // 
            resources.ApplyResources(this.lblLeftRightImgs, "lblLeftRightImgs");
            this.lblLeftRightImgs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLeftRightImgs.Name = "lblLeftRightImgs";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.progBar});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            resources.ApplyResources(this.lblStatus, "lblStatus");
            // 
            // progBar
            // 
            this.progBar.Name = "progBar";
            resources.ApplyResources(this.progBar, "progBar");
            // 
            // lblReady
            // 
            resources.ApplyResources(this.lblReady, "lblReady");
            this.lblReady.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblReady.Name = "lblReady";
            // 
            // lblConvFolder
            // 
            resources.ApplyResources(this.lblConvFolder, "lblConvFolder");
            this.lblConvFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblConvFolder.Name = "lblConvFolder";
            // 
            // lblOpenFiles
            // 
            resources.ApplyResources(this.lblOpenFiles, "lblOpenFiles");
            this.lblOpenFiles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblOpenFiles.Name = "lblOpenFiles";
            // 
            // frmMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblOpenFiles);
            this.Controls.Add(this.lblConvFolder);
            this.Controls.Add(this.lblReady);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.lblLeftRightImgs);
            this.Controls.Add(this.menuStrip);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "frmMain";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importleftrightToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mPOToJPGStereoPairsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allMPOsInAFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windoesToolStripMenuItem;
        private System.Windows.Forms.Label lblLeftRightImgs;
        private System.Windows.Forms.ToolStripMenuItem allMPOsInAFolderToJPGStereoPairsToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripProgressBar progBar;
        private System.Windows.Forms.Label lblReady;
        private System.Windows.Forms.Label lblConvFolder;
        private System.Windows.Forms.Label lblOpenFiles;
    }
}

