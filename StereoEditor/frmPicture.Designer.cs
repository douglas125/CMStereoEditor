namespace StereoEditor
{
    partial class frmPicture
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPicture));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnPrevious = new System.Windows.Forms.ToolStripButton();
            this.btnNext = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnShowAdvOptions = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnParalax = new System.Windows.Forms.ToolStripButton();
            this.btnFullScreen = new System.Windows.Forms.ToolStripButton();
            this.btnSwitch = new System.Windows.Forms.ToolStripButton();
            this.separator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCrop = new System.Windows.Forms.ToolStripButton();
            this.btnFilter = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.lblWantSave = new System.Windows.Forms.Label();
            this.lblCantCrop = new System.Windows.Forms.Label();
            this.lblFilterError = new System.Windows.Forms.Label();
            this.lblReLoadFilters = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.AccessibleDescription = null;
            this.toolStrip.AccessibleName = null;
            resources.ApplyResources(this.toolStrip, "toolStrip");
            this.toolStrip.BackgroundImage = null;
            this.toolStrip.Font = null;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOpen,
            this.btnPrevious,
            this.btnNext,
            this.btnSave,
            this.toolStripSeparator1,
            this.btnShowAdvOptions,
            this.toolStripSeparator3,
            this.btnParalax,
            this.btnFullScreen,
            this.btnSwitch,
            this.separator1,
            this.btnCrop,
            this.btnFilter});
            this.toolStrip.Name = "toolStrip";
            this.toolTip.SetToolTip(this.toolStrip, resources.GetString("toolStrip.ToolTip"));
            // 
            // btnOpen
            // 
            this.btnOpen.AccessibleDescription = null;
            this.btnOpen.AccessibleName = null;
            resources.ApplyResources(this.btnOpen, "btnOpen");
            this.btnOpen.BackgroundImage = null;
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.AccessibleDescription = null;
            this.btnPrevious.AccessibleName = null;
            resources.ApplyResources(this.btnPrevious, "btnPrevious");
            this.btnPrevious.BackgroundImage = null;
            this.btnPrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // btnNext
            // 
            this.btnNext.AccessibleDescription = null;
            this.btnNext.AccessibleName = null;
            resources.ApplyResources(this.btnNext, "btnNext");
            this.btnNext.BackgroundImage = null;
            this.btnNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNext.Name = "btnNext";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnSave
            // 
            this.btnSave.AccessibleDescription = null;
            this.btnSave.AccessibleName = null;
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.BackgroundImage = null;
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Name = "btnSave";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.AccessibleDescription = null;
            this.toolStripSeparator1.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // btnShowAdvOptions
            // 
            this.btnShowAdvOptions.AccessibleDescription = null;
            this.btnShowAdvOptions.AccessibleName = null;
            resources.ApplyResources(this.btnShowAdvOptions, "btnShowAdvOptions");
            this.btnShowAdvOptions.BackgroundImage = null;
            this.btnShowAdvOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnShowAdvOptions.Name = "btnShowAdvOptions";
            this.btnShowAdvOptions.Click += new System.EventHandler(this.btnShowAdvOptions_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.AccessibleDescription = null;
            this.toolStripSeparator3.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // btnParalax
            // 
            this.btnParalax.AccessibleDescription = null;
            this.btnParalax.AccessibleName = null;
            resources.ApplyResources(this.btnParalax, "btnParalax");
            this.btnParalax.BackgroundImage = null;
            this.btnParalax.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnParalax.Name = "btnParalax";
            this.btnParalax.Click += new System.EventHandler(this.btnParalax_Click);
            // 
            // btnFullScreen
            // 
            this.btnFullScreen.AccessibleDescription = null;
            this.btnFullScreen.AccessibleName = null;
            resources.ApplyResources(this.btnFullScreen, "btnFullScreen");
            this.btnFullScreen.BackgroundImage = null;
            this.btnFullScreen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFullScreen.Name = "btnFullScreen";
            this.btnFullScreen.Click += new System.EventHandler(this.btnFullScreen_Click);
            // 
            // btnSwitch
            // 
            this.btnSwitch.AccessibleDescription = null;
            this.btnSwitch.AccessibleName = null;
            resources.ApplyResources(this.btnSwitch, "btnSwitch");
            this.btnSwitch.BackgroundImage = null;
            this.btnSwitch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSwitch.Name = "btnSwitch";
            this.btnSwitch.Click += new System.EventHandler(this.btnSwitch_Click);
            // 
            // separator1
            // 
            this.separator1.AccessibleDescription = null;
            this.separator1.AccessibleName = null;
            resources.ApplyResources(this.separator1, "separator1");
            this.separator1.Name = "separator1";
            // 
            // btnCrop
            // 
            this.btnCrop.AccessibleDescription = null;
            this.btnCrop.AccessibleName = null;
            resources.ApplyResources(this.btnCrop, "btnCrop");
            this.btnCrop.BackgroundImage = null;
            this.btnCrop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCrop.Name = "btnCrop";
            this.btnCrop.Click += new System.EventHandler(this.btnCrop_Click);
            // 
            // btnFilter
            // 
            this.btnFilter.AccessibleDescription = null;
            this.btnFilter.AccessibleName = null;
            resources.ApplyResources(this.btnFilter, "btnFilter");
            this.btnFilter.BackgroundImage = null;
            this.btnFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.DropDownOpening += new System.EventHandler(this.btnFilter_DropDownOpening);
            this.btnFilter.DoubleClick += new System.EventHandler(this.btnFilter_DoubleClick);
            this.btnFilter.DropDownOpened += new System.EventHandler(this.btnFilter_DropDownOpened);
            this.btnFilter.DropDownClosed += new System.EventHandler(this.btnFilter_DropDownClosed);
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // lblWantSave
            // 
            this.lblWantSave.AccessibleDescription = null;
            this.lblWantSave.AccessibleName = null;
            resources.ApplyResources(this.lblWantSave, "lblWantSave");
            this.lblWantSave.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblWantSave.Font = null;
            this.lblWantSave.Name = "lblWantSave";
            this.toolTip.SetToolTip(this.lblWantSave, resources.GetString("lblWantSave.ToolTip"));
            // 
            // lblCantCrop
            // 
            this.lblCantCrop.AccessibleDescription = null;
            this.lblCantCrop.AccessibleName = null;
            resources.ApplyResources(this.lblCantCrop, "lblCantCrop");
            this.lblCantCrop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCantCrop.Font = null;
            this.lblCantCrop.Name = "lblCantCrop";
            this.toolTip.SetToolTip(this.lblCantCrop, resources.GetString("lblCantCrop.ToolTip"));
            // 
            // lblFilterError
            // 
            this.lblFilterError.AccessibleDescription = null;
            this.lblFilterError.AccessibleName = null;
            resources.ApplyResources(this.lblFilterError, "lblFilterError");
            this.lblFilterError.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFilterError.Font = null;
            this.lblFilterError.Name = "lblFilterError";
            this.toolTip.SetToolTip(this.lblFilterError, resources.GetString("lblFilterError.ToolTip"));
            // 
            // lblReLoadFilters
            // 
            this.lblReLoadFilters.AccessibleDescription = null;
            this.lblReLoadFilters.AccessibleName = null;
            resources.ApplyResources(this.lblReLoadFilters, "lblReLoadFilters");
            this.lblReLoadFilters.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblReLoadFilters.Font = null;
            this.lblReLoadFilters.Name = "lblReLoadFilters";
            this.toolTip.SetToolTip(this.lblReLoadFilters, resources.GetString("lblReLoadFilters.ToolTip"));
            // 
            // timer
            // 
            this.timer.Interval = 16;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // frmPicture
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.lblReLoadFilters);
            this.Controls.Add(this.lblFilterError);
            this.Controls.Add(this.lblCantCrop);
            this.Controls.Add(this.lblWantSave);
            this.Controls.Add(this.toolStrip);
            this.Font = null;
            this.Name = "frmPicture";
            this.toolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Load += new System.EventHandler(this.frmPicture_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPicture_FormClosing);
            this.Resize += new System.EventHandler(this.frmPicture_Resize);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripButton btnParalax;
        private System.Windows.Forms.Label lblWantSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator separator1;
        private System.Windows.Forms.ToolStripButton btnCrop;
        private System.Windows.Forms.Label lblCantCrop;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnPrevious;
        private System.Windows.Forms.ToolStripButton btnNext;
        private System.Windows.Forms.ToolStripButton btnFullScreen;
        private System.Windows.Forms.ToolStripButton btnSwitch;
        private System.Windows.Forms.ToolStripButton btnShowAdvOptions;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripDropDownButton btnFilter;
        private System.Windows.Forms.Label lblFilterError;
        private System.Windows.Forms.Label lblReLoadFilters;
    }
}