using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StereoEditor
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.LCID);

            InitializeComponent();
        }

        #region Menus

        #region File
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Stereoscopic Images|*.JPS;*.JPG;*.MPO";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                lblStatus.Text = lblOpenFiles.Text;
            

                for (int i = 0; i < ofd.FileNames.Length; i++)
                {
                    try
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(ofd.FileNames[i]);

                        Bitmap bmp = null;
                        if (fi.Extension.ToLower() == ".mpo")
                        {
                            List<Bitmap> bmps = MPOReader.ReadFromMPF(ofd.FileNames[i]);
                            bmp = MPOReader.AssembleJPS(bmps[0], bmps[1]);
                        }
                        else
                        {
                            bmp = new Bitmap(ofd.FileNames[i]);
                        }
                        frmPicture frmPic = new frmPicture(bmp, fi.Directory.GetFiles("*" + fi.Extension), fi.FullName);
                        frmPic.Text = ofd.FileNames[i];
                        frmPic.MdiParent = this;
                        frmPic.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                lblStatus.Text = lblReady.Text;
            

            }
        }
        private void importleftrightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Images|*.JPG;*.JPEG;*.BMP;*.GIF;*.TIF";

                string[] s = lblLeftRightImgs.Text.Split('|');

                //Left
                ofd.Title = s[0];
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string leftImg = ofd.FileName;

                    System.IO.FileInfo fi = new System.IO.FileInfo(leftImg);
                    if (fi.Name.Split('.')[0].ToUpper().EndsWith("L"))
                    {
                        ofd.FileName = fi.Name.Substring(0, fi.Name.Length - 5) +"R"+ fi.Extension;
                    }

                    //Right
                    ofd.Title = s[1];
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        lblStatus.Text = lblOpenFiles.Text;

                        string rightimg = ofd.FileName;

                        Bitmap left = new Bitmap(leftImg);
                        Bitmap right = new Bitmap(rightimg);

                        frmPicture frmPic = new frmPicture(MPOReader.AssembleJPS(left, right), fi.Directory.GetFiles("*" + fi.Extension), fi.FullName);
                        frmPic.Text = leftImg;
                        frmPic.MdiParent = this;
                        frmPic.Show();

                        lblStatus.Text = lblReady.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {



            Application.Exit();
        }
        #endregion

        #region About
        private static AboutBox ab;
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ab == null || ab.IsDisposed)
            {
                ab = new AboutBox();
                ab.MdiParent = this;
            }
            ab.Show();
        }
        #endregion

        #region Convert MPO
        private void cToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConvertFile(convFormat.JPS);
        }
        private void mPOToJPGStereoPairsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConvertFile(convFormat.StereoPair);
        }

        private void allMPOsInAFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConvertFolder(convFormat.JPS);
        }

        private void allMPOsInAFolderToJPGStereoPairsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConvertFolder(convFormat.StereoPair);
        }
        private enum convFormat
        {
            JPS, StereoPair
        }

        /// <summary>Opens a dialog box to convert a .MPO file to a given format</summary>
        /// <param name="newFormat"></param>
        private void ConvertFile(convFormat newFormat)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Multi Picture File|*.MPO";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //Convert asynchronously
                    System.IO.FileInfo fi = new System.IO.FileInfo(ofd.FileName);
                    object[] args = new object[3];
                    args[0] = fi.FullName; args[1] = ".JPS"; args[2] = 98;

                    System.Threading.Thread t;
                    if (newFormat == convFormat.StereoPair) t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(MPOReader.ConvertMPOtoLeftRightPair));
                    else t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(MPOReader.ConvertMPO));

                    t.Start(args);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }



        }

        private void ConvertFolder(convFormat newFormat)
        {
            allMPOsInAFolderToJPGStereoPairsToolStripMenuItem.Enabled = false;
            allMPOsInAFolderToolStripMenuItem.Enabled = false;

            bool stereoPair = (newFormat == convFormat.StereoPair);
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                lblStatus.Text = lblConvFolder.Text;// +fbd.SelectedPath;

                object[] args = new object[] { fbd.SelectedPath, ".JPS", 16, 98, stereoPair };
                
                try
                {
                    System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart
                        (MPOReader.ConvertFolderMPOtoJPG));

                    t.Start(args);

                    while (MPOReader.TotalToBeConverted == 0)
                    {
                        System.Threading.Thread.Sleep(40);
                        Application.DoEvents();
                    }

                    progBar.Visible = true;
                    progBar.Maximum = MPOReader.TotalToBeConverted;

                    while (MPOReader.TotalToBeConverted != 0)
                    {
                        progBar.Value = MPOReader.QtdConverted;

                        System.Threading.Thread.Sleep(40);
                        Application.DoEvents();
                    }

                    progBar.Visible = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                lblStatus.Text = lblReady.Text;
            }

            allMPOsInAFolderToJPGStereoPairsToolStripMenuItem.Enabled = true;
            allMPOsInAFolderToolStripMenuItem.Enabled = true;
        }
        #endregion


        #endregion


        private void frmMain_Load(object sender, EventArgs e)
        {
            lblStatus.Text = lblReady.Text;
        }

        /// <summary>Hides controls in this form</summary>
        public void HideControls()
        {
            statusStrip1.Visible = false;
            menuStrip.Visible = false;
        }
        /// <summary>Displays controls in this form</summary>
        public void ShowControls()
        {
            statusStrip1.Visible = true;
            menuStrip.Visible = true;
        }

    }
}
