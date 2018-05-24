using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using OpenTK.Graphics.OpenGL;

namespace StereoEditor
{
    public partial class frmPicture : Form
    {
        Bitmap ImgBmp = null;
        float aspect = 1.0f;
        /// <summary>Creates a new form displaying JPS bmp</summary>
        public frmPicture(Bitmap bmp, FileInfo[] nextFiles, string curFile)
        {
            InitializeComponent();
            ImgBmp = bmp;
            aspect = (float)bmp.Width / (float)bmp.Height;

            ImageFiles = nextFiles;

            //Locates current ind position
            for (int i = 0; i < nextFiles.Length; i++)
            {
                if (nextFiles[i].FullName == curFile) curImgIndex = i;
            }
        }

        OpenTK.GLControl GLPic;
        private void frmPicture_Load(object sender, EventArgs e)
        {
            //ToolStripItem ti = btnFilter.DropDownItems.Add("teste");
            //ti.Click += new EventHandler(ti_Click);

            #region OpenGL Window creation
            OpenTK.Graphics.ColorFormat cf = new OpenTK.Graphics.ColorFormat();
            OpenTK.Graphics.GraphicsMode gm =
                new OpenTK.Graphics.GraphicsMode(32, 24, 8, 4, cf, 4, true);
            this.GLPic = new OpenTK.GLControl(gm);
            
            GLPic.MakeCurrent();

            this.Controls.Add(GLPic);

            GLPic.Paint += new PaintEventHandler(GLWindow_Paint);

            frmPicture_Resize(sender, e);
            #endregion

            Application.DoEvents();

            #region Mouse events
            GLPic.MouseWheel += new MouseEventHandler(GLPic_MouseWheel);
            GLPic.MouseDown += new MouseEventHandler(GLPic_MouseDown);
            GLPic.MouseUp += new MouseEventHandler(GLPic_MouseUp);
            GLPic.MouseMove += new MouseEventHandler(GLPic_MouseMove);
            GLPic.MouseEnter += new EventHandler(GLPic_MouseEnter);
            GLPic.MouseLeave += new EventHandler(GLPic_MouseLeave);
            GLPic.Cursor = Cursors.Cross;
            GLPic.DoubleClick += new EventHandler(GLPic_DoubleClick);
            GLPic.KeyDown += new KeyEventHandler(GLPic_KeyDown);
            #endregion

            #region VBO Creation

            if (curMode == StereoMode.CrossedEyes) CreateCrossedEyesVBOData();
            else if (curMode == StereoMode.Wiggle) CreateAnimated3DVBOData();
            UpdateVBOs();

            #endregion

            GLPic.MakeCurrent();
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.1f, 0.1f, 0.3f, 0.0f);
            GL.Enable(EnableCap.Texture2D);


            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);


            //Z-Buffer
            GL.ClearDepth(1.0f);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.DepthTest);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.DontCare);

            ////Materiais, funcoes para habilitar cor
            //GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse); //tem q vir antes do enable
            //GL.Enable(EnableCap.ColorMaterial);


            //Redimension to picture
            if (curMode == StereoMode.Wiggle) this.Width = (int)(aspect * (float)GLPic.Height * 0.5f);
            else if (curMode == StereoMode.CrossedEyes) this.Width = (int)(aspect * (float)GLPic.Height * 1.8f);

            SetupViewport();

            timer.Enabled = true;

            //Filters
            if (StereoEditor.CLFilters.Count > 0)
            {
                foreach (StereoEditor.CLFilter f in StereoEditor.CLFilters)
                {
                    ToolStripItem t = btnFilter.DropDownItems.Add(f.FilterName);
                    t.Click += new EventHandler(t_Click);
                }
            }
        }


        #region View modes and corresponding VBOs
        private enum StereoMode
        {
            Wiggle,
            CrossedEyes
        };

        private StereoMode curMode = StereoMode.Wiggle;

        private void btnSwitch_Click(object sender, EventArgs e)
        {
            if (curMode == StereoMode.Wiggle)
            {
                curMode = StereoMode.CrossedEyes;
                CreateCrossedEyesVBOData();
            }
            else
            {
                curMode = StereoMode.Wiggle;
                CreateAnimated3DVBOData();
            }
            UpdateVBOs();

            frmPicture_Resize(sender, e);
        }

        /// <summary>Creates VBOs for usage with crossed eyes</summary>
        private void CreateCrossedEyesVBOData()
        {
            VBOVertex = new float[] 
            {
                0.0f,0.0f,0.0f,
                0.5f,0.0f,0.0f,
                0.5f,1.0f,0.0f,
                0.0f,1.0f,0.0f,

                0.5f,0.0f,0.0f,
                1.0f,0.0f,0.0f,
                1.0f,1.0f,0.0f,
                0.5f,1.0f,0.0f
            };
            //VBOVertex = new float[] 
            //{
            //    0.0f,0.0f,0.0f,
            //    0.5f,0.0f,0.0f,
            //    0.5f,1.0f,0.0f,
            //    0.0f,1.0f,0.0f,

            //    0.0f,0.0f,0.0f,
            //    0.5f,0.0f,0.0f,
            //    0.5f,1.0f,0.0f,
            //    0.0f,1.0f,0.0f
            //};

            VBOColor = new float[]
            {
                1.0f,0.5f,0.0f,1.0f,
                1.0f,0.5f,0.0f,1.0f,
                1.0f,0.0f,0.0f,1.0f,
                1.0f,0.0f,0.0f,1.0f,
                
                0.0f,0.0f,1.0f,1.0f,
                0.0f,0.0f,1.0f,1.0f,
                0.0f,0.5f,1.0f,1.0f,
                0.0f,0.5f,1.0f,1.0f
            };
            //VBOColor = new float[]
            //{
            //    0.0f,0.0f,1.0f,1.0f,
            //    0.0f,0.0f,1.0f,1.0f,
            //    0.0f,0.0f,1.0f,1.0f,
            //    0.0f,0.0f,1.0f,1.0f,

            //    0.8f,0.0f,0.0f,0.5f,
            //    0.8f,0.0f,0.0f,0.5f,
            //    0.8f,0.0f,0.0f,0.5f,
            //    0.8f,0.0f,0.0f,0.5f
            //};

            for (int i = 0; i < VBOColor.Length; i++) VBOColor[i] = 1;

            VBOElements = new int[]
            {
                0,1,2,
                0,2,3,

                4,5,6,
                4,6,7
            };
        }

        /// <summary>Creates VBOs for usage with crossed eyes</summary>
        private void CreateAnimated3DVBOData()
        {
            VBOVertex = new float[] 
            {
                0.0f,0.0f,0.0f,
                1.0f,0.0f,0.0f,
                1.0f,1.0f,0.0f,
                0.0f,1.0f,0.0f,

                0.0f,0.0f,0.0f,
                1.0f,0.0f,0.0f,
                1.0f,1.0f,0.0f,
                0.0f,1.0f,0.0f
            };
            //VBOVertex = new float[] 
            //{
            //    0.0f,0.0f,0.0f,
            //    0.5f,0.0f,0.0f,
            //    0.5f,1.0f,0.0f,
            //    0.0f,1.0f,0.0f,

            //    0.0f,0.0f,0.0f,
            //    0.5f,0.0f,0.0f,
            //    0.5f,1.0f,0.0f,
            //    0.0f,1.0f,0.0f
            //};

            VBOColor = new float[]
            {
                1.0f,0.5f,0.0f,1.0f,
                1.0f,0.5f,0.0f,1.0f,
                1.0f,0.0f,0.0f,1.0f,
                1.0f,0.0f,0.0f,1.0f,
                
                0.0f,0.0f,1.0f,1.0f,
                0.0f,0.0f,1.0f,1.0f,
                0.0f,0.5f,1.0f,1.0f,
                0.0f,0.5f,1.0f,1.0f
            };
            //VBOColor = new float[]
            //{
            //    0.0f,0.0f,1.0f,1.0f,
            //    0.0f,0.0f,1.0f,1.0f,
            //    0.0f,0.0f,1.0f,1.0f,
            //    0.0f,0.0f,1.0f,1.0f,

            //    0.8f,0.0f,0.0f,0.5f,
            //    0.8f,0.0f,0.0f,0.5f,
            //    0.8f,0.0f,0.0f,0.5f,
            //    0.8f,0.0f,0.0f,0.5f
            //};

            for (int i = 0; i < VBOColor.Length; i++) VBOColor[i] = 1;

            VBOElements = new int[]
            {
                0,1,2,
                0,2,3,

                4,5,6,
                4,6,7
            };
        }

        #endregion

        /// <summary>Updates VBOs in GPU memory</summary>
        private void UpdateVBOs()
        {
            if (GLVBOs[0] == 0)
            {
                GL.GenBuffers(4, GLVBOs);
            }
            CreateTexVBO();

            //Stores data in the buffers
            BufferUsageHint Hint = BufferUsageHint.StaticDraw;

            GL.BindBuffer(BufferTarget.ArrayBuffer, GLVBOs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(VBOVertex.Length * sizeof(float)), VBOVertex, Hint);

            //GL.BindBuffer(BufferTarget.ArrayBuffer, GLVBOs[1]);
            //GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(VBONormals.Count * sizeof(float)), NormalsData.ToArray(), Hint);

            GL.BindBuffer(BufferTarget.ArrayBuffer, GLVBOs[1]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(VBOColor.Length * sizeof(float)), VBOColor, Hint);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, GLVBOs[3]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(VBOElements.Length * sizeof(int)), VBOElements, Hint);
        }
        
        #region Drawing OpenGL

        void CreateTexVBO()
        {
            VBOTexCoord = new float[] 
            {
                0.5f,0.0f,
                1.0f,0.0f,
                1.0f,1.0f,
                0.5f,1.0f,

                0.0f,0.0f, 
                0.5f,0.0f,
                0.5f,1.0f,
                0.0f,1.0f
            };

            float invScale = 1.0f / (float)scale;
            for (int i = 0; i < 8; i += 2)
            {
                VBOTexCoord[i] = (float)paralax - (float)(tx + txTemp) + 0.5f + (VBOTexCoord[i] - 0.5f) * invScale;
                VBOTexCoord[i + 1] = -(float)(ty + tyTemp) + VBOTexCoord[i + 1] * invScale;
            }

            for (int i = 8; i < 16; i += 2)
            {
                VBOTexCoord[i] = -(float)paralax - (float)(tx + txTemp) + invScale * VBOTexCoord[i];
                VBOTexCoord[i + 1] = -(float)(ty + tyTemp) + VBOTexCoord[i + 1] * invScale;
            }


            GL.BindBuffer(BufferTarget.ArrayBuffer, GLVBOs[2]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(VBOTexCoord.Length * sizeof(float)), VBOTexCoord, BufferUsageHint.DynamicDraw);
        }

        int GLTex;

        /// <summary>Holds OpenGL VBO numbers</summary>
        int[] GLVBOs = new int[4];
        /// <summary>Elements VBO</summary>
        int[] VBOElements;
        /// <summary>OpenGL VBOs</summary>
        float[] VBOVertex, VBOTexCoord, VBOColor;

        double scale = 1.0;
        double paralax = 0;
        double tx = 0, ty = 0, txTemp = 0, tyTemp = 0;
        void GLWindow_Paint(object sender, PaintEventArgs e)
        {
            GLPic.MakeCurrent();

            //GL.DrawBuffer(DrawBufferMode.BackLeft);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(0.1f, 0.1f, 0.3f, 0.0f);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, 1, 0, 1, -1, 1);

            if (curMode == StereoMode.CrossedEyes)
            {
                #region Draws 3D mouse cursor
                float xMouse, yMouse; float cursorSize = 0.05f;
                xMouse = curMousePos.X > 0.5 ? curMousePos.X - 0.5f : curMousePos.X;
                yMouse = 1.0f - curMousePos.Y;
                GL.Translate(0.0f, 0.0f, -1.0f);

                //Adjust cursor color
                DateTime dt = DateTime.Now;
                double param = dt.Second + dt.Millisecond * 0.001f + 60.0 * dt.Minute + 3600.0 * dt.Hour;
                float cursorColor = (float)Math.Pow((0.5 + 0.5 * Math.Sin(4.1 * param)), 8);
                GL.Color4(cursorColor, cursorColor, cursorColor, 1.0f);

                GL.LineWidth(2);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.Begin(BeginMode.Lines);
                {
                    //Cursor of right image (displayed to the left)
                    GL.Vertex3(xMouse - cursorSize * 0.5f, yMouse, 0.1f);
                    GL.Vertex3(xMouse + cursorSize * 0.5f, yMouse, 0.1f);
                    GL.Vertex3(xMouse, yMouse - cursorSize * 0.5f * aspect, 0.2f);
                    GL.Vertex3(xMouse, yMouse + cursorSize * 0.5f * aspect, 0.2f);

                    //Cursor of left image (displayed to the right)
                    GL.Vertex3(0.5f + xMouse - cursorSize * 0.5f, yMouse, 0.2f);
                    GL.Vertex3(0.5f + xMouse + cursorSize * 0.5f, yMouse, 0.2f);
                    GL.Vertex3(0.5f + xMouse, yMouse - cursorSize * 0.5f * aspect, 0.2f);
                    GL.Vertex3(0.5f + xMouse, yMouse + cursorSize * 0.5f * aspect, 0.2f);
                }
                GL.End();
                #endregion
            }

            //GLPic.SwapBuffers();
            //return;

            //Check texture
            if (GLTex == 0)
            {
                GLTex = GL.GenTexture();
                ApplyTex(ImgBmp, GLTex);
            }

            #region Perform animation if using animated stereo mode

            if (curMode == StereoMode.Wiggle)
            {
                //Adjust cursor color
                DateTime dt2 = DateTime.Now;
                double param2 = dt2.Second + dt2.Millisecond * 0.001f + 60.0 * dt2.Minute + 3600.0 * dt2.Hour;

                double temp = Math.Sin(3.1 * param2);
                float minIntens = 0.2f;

                float imgIntens = (float)(temp * (0.5 - minIntens) + 0.5);
                VBOColor[16 + 3] = imgIntens; VBOColor[16 + 7] = imgIntens; VBOColor[16 + 11] = imgIntens; VBOColor[16 + 15] = imgIntens;

                GL.BindBuffer(BufferTarget.ArrayBuffer, GLVBOs[1]);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(VBOColor.Length * sizeof(float)), VBOColor, BufferUsageHint.DynamicDraw);
            }
            
            #endregion


            #region Draw VBOs
            GL.BindTexture(TextureTarget.Texture2D, GLTex);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.ColorArray);
            //GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, GLVBOs[0]);
            GL.VertexPointer(3, VertexPointerType.Float, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, GLVBOs[1]);
            GL.ColorPointer(4, ColorPointerType.Float, 0, 0);

            //GL.BindBuffer(BufferTarget.ArrayBuffer, parte.GLBuffers[2]);
            //GL.NormalPointer(NormalPointerType.Float, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, GLVBOs[2]);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, GLVBOs[3]);

            GL.DrawElements(BeginMode.Triangles, VBOElements.Length, DrawElementsType.UnsignedInt, 0);

            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.ColorArray);
            //GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
            #endregion


            //RIGHT
            //AplicaTextura(bmp2);
            //if (bmp2 != null) GL.BindTexture(TextureTarget.Texture2D, texture[1]);
            //GL.Scale(scale, scale, scale);
            //GL.Translate((tx + txTemp) * scale - paralax, (ty + tyTemp) * scale, 0);


            GLPic.SwapBuffers();
        }
        
        private void SetupViewport()
        {
            int w = GLPic.Width;
            int h = GLPic.Height;
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area
        }

        private void frmPicture_Resize(object sender, EventArgs e)
        {
            GLPic.MakeCurrent();

            if (curMode == StereoMode.CrossedEyes)
            {
                GLPic.Height = this.Height - (int)(1.75 * toolStrip.Height);
                GLPic.Width = (int)(aspect * (float)GLPic.Height);

                if (GLPic.Width > this.Width)
                {
                    GLPic.Width = this.Width;
                    GLPic.Height = (int)((float)this.Width / aspect);
                }
            }
            else
            {
                GLPic.Height = this.Height - (int)(1.75 * toolStrip.Height);
                GLPic.Width = (int)(0.5f * aspect * (float)GLPic.Height);

                if (GLPic.Width > this.Width)
                {
                    GLPic.Width = this.Width;
                    GLPic.Height = (int)((float)this.Width / (0.5f * aspect));
                }
            }

            GLPic.Top = (this.Height - GLPic.Height - toolStrip.Height) >> 1;
            GLPic.Left = (this.Width - GLPic.Width) >> 1;

            SetupViewport();
             
        }
        #endregion

        #region Mouse Control
        bool clicado = false; Point p0 = new Point();
        PointF curMousePos = new PointF();
        void GLPic_MouseMove(object sender, MouseEventArgs e)
        {
            curMousePos.X = (float)e.X / (float)GLPic.Width;
            curMousePos.Y = (float)e.Y / (float)GLPic.Height;
            if (clicado)
            {
                float x = -(p0.X - e.X) / (float)this.Width;
                float y = (p0.Y - e.Y) / (float)this.Height;

                txTemp = x / scale;
                tyTemp = y / scale;

                CreateTexVBO();
            }
             

        }

        void GLPic_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                tx += txTemp; ty += tyTemp;
                txTemp = 0; tyTemp = 0;
                clicado = false;

                CreateTexVBO();
                 
            }
        }

        void GLPic_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                clicado = true;
                p0 = new Point(e.X, e.Y);
            }
        }

        void GLPic_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!clicado)
            {
                scale *= (1 + 0.001 * (double)e.Delta);
                if (scale < 1) scale = 1;
            }
            else
            {
                paralax += (double)e.Delta * 0.000017 / scale;
            }

            CreateTexVBO();
             

        }

        void GLPic_MouseLeave(object sender, EventArgs e)
        {
            if (curMode == StereoMode.CrossedEyes) Cursor.Show();
        }

        void GLPic_MouseEnter(object sender, EventArgs e)
        {
            if (curMode == StereoMode.CrossedEyes) Cursor.Hide();
        }
        #endregion

        #region Create texture

        /// <summary>Limit texture size to speed up image display</summary>
        private static  int LIMITIMAGEDIMENSIONSTO = 2048;

        private void ApplyTex(Bitmap TexBitmap, int GLTexture)
        {
            try
            {
                
                if (TexBitmap != null)
                {
                    //Reads hardware limitation
                    int maxTexSize;
                    GL.GetInteger(GetPName.MaxTextureSize, out maxTexSize);

                    //Compatibility with onboard GPUs
                    Bitmap TextureBitmap = ResizeToPowerOf2(TexBitmap, Math.Min(maxTexSize, LIMITIMAGEDIMENSIONSTO));

                    //texture, if there is one
                    System.Drawing.Bitmap image = new System.Drawing.Bitmap(TextureBitmap);
                    image.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
                    System.Drawing.Imaging.BitmapData bitmapdata;
                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);

                    bitmapdata = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                    GL.BindTexture(TextureTarget.Texture2D, GLTexture);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, image.Width, image.Height,
                        0, (PixelFormat)(int)All.BgrExt, PixelType.UnsignedByte, bitmapdata.Scan0);

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);		// Linear Filtering
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);		// Linear Filtering

                    image.UnlockBits(bitmapdata);
                    image.Dispose();

                    GL.BindTexture(TextureTarget.Texture2D, GLTexture);
                }
            }
            catch
            { }
        }


        /// <summary>Method for resizing an image</summary>
        /// <param name="img">Image to resize</param>
        private static Bitmap ResizeToPowerOf2(Image img, int MaxDim)
        {
            //get the height and width of the image
            int originalW = img.Width;
            int originalH = img.Height;

            //get the new size based on the percentage change
            int resizedW = (int)Math.Pow(2, Math.Ceiling(Math.Log((double)originalW, 2)));
            int resizedH = (int)Math.Pow(2, Math.Ceiling(Math.Log((double)originalH, 2)));

            if (resizedH > MaxDim) resizedH = MaxDim;
            if (resizedW > MaxDim) resizedW = MaxDim;

            //create a new Bitmap the size of the new image
            Bitmap bmp = new Bitmap(resizedW, resizedH);

            //create a new graphic from the Bitmap
            Graphics graphic = Graphics.FromImage((Image)bmp);
            graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            //draw the newly resized image
            graphic.DrawImage(img, 0, 0, resizedW, resizedH);

            //dispose and free up the resources
            graphic.Dispose();

            //return the image
            return bmp;
        }
        #endregion

        #region File save

        /// <summary>Has the picture been saved after being modified?</summary>
        bool saved = true;

        /// <summary>Name of previously saved file</summary>
        string fileName = "";
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Stereo JPS|*.JPS";
            sfd.FileName = fileName;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    MPOReader.Save(this.ImgBmp, sfd.FileName, 98);
                    fileName = sfd.FileName;
                    saved = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void frmPicture_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!saved)
            {
                DialogResult r = MessageBox.Show(lblWantSave.Text, this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    btnSave_Click(sender, e);
                }
                else if (r == DialogResult.Cancel) e.Cancel = true;
            }
        }
        #endregion

        #region Image edition

        /// <summary>Reloads bitmap onto GL window after edition</summary>
        private void ReloadBmp()
        {
            saved = false;

            scale = 1.0;
            paralax = 0;
            tx = 0; ty = 0;

            CreateTexVBO();

            GLPic.MakeCurrent();
            ApplyTex(ImgBmp, GLTex);

             
        }

        private void btnCrop_Click(object sender, EventArgs e)
        {
            PointF p0L, p0R, dim;

            GetBoxCoords(out p0L, out p0R, out dim);

            try
            {
                ImgBmp = StereoEditor.Crop(ImgBmp, p0L, p0R, dim);
                ReloadBmp();
            }
            catch
            {
                MessageBox.Show(lblCantCrop.Text, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region Get current box coordinates
        /// <summary>
        /// Gets coordinates of current regions in 0 to 1 interval
        /// </summary>
        private void GetBoxCoords(out PointF p0L, out PointF p0R, out PointF dim)
        {
            p0L = new PointF(2.0f * (VBOTexCoord[0] - 0.5f), 1.0f - VBOTexCoord[5]);
            p0R = new PointF(2.0f * (VBOTexCoord[8]), 1.0f - VBOTexCoord[13]);
            dim = new PointF(2.0f * (VBOTexCoord[12] - VBOTexCoord[8]), VBOTexCoord[13] - VBOTexCoord[9]);

            p0L.X = CorrectToInterval0to1(p0L.X);
            p0L.Y = CorrectToInterval0to1(p0L.Y);
            p0R.X = CorrectToInterval0to1(p0R.X);
            p0R.Y = CorrectToInterval0to1(p0R.Y);
        }

        private float CorrectToInterval0to1(float x)
        {
            return x >= 0 ? x - (float)Math.Floor(x) : 1 + x + (float)Math.Floor(-x);
        }

        #endregion

        #region Parallax
        private void btnParalax_Click(object sender, EventArgs e)
        {
            bool succeededOnce = false;
            try
            {
                //iterates because image changes after parallax calculation
                //so, next results can differ a bit
                for (int i = 0; i < 7; i++)
                {
                    PointF p0L, p0R, dim; bool succeeded;
                    GetBoxCoords(out p0L, out p0R, out dim);

                    //uses a silghtly smaller region than the displayed
                    p0L.X += 0.15f * dim.X;
                    p0R.X += 0.15f * dim.X;
                    dim.X *= 0.7f;
                    p0L.Y += 0.2f * dim.Y;
                    p0R.Y += 0.2f * dim.Y;
                    dim.Y *= 0.6f;

                    float c = StereoEditor.ParallaxFind(ImgBmp, p0L, p0R, dim, out succeeded);

                    if (!succeeded && i == 0)
                    {
                        btnParalax.BackColor = Color.Red; i = 7;
                    }
                    else
                    {
                        btnParalax.BackColor = Color.LightGreen;
                        succeededOnce = true;
                    }

                    //This is because paralax is applyed to the left AND right images (x2) and the picture scale is 0 to 0.5 (x2 too)
                    // 1/4 compensates the scaling
                    paralax += c * 0.25f;

                    CreateTexVBO();
                     

                    Application.DoEvents();
                    System.Threading.Thread.Sleep(10);
                }
            }
            catch
            {
                if (!succeededOnce) btnParalax.BackColor = Color.Red;
            }
        }
        #endregion

        private void timer_Tick(object sender, EventArgs e)
        {
            //Forces redraw
            GLPic.Invalidate();
        }

        #region Load image and next pictures


        FileInfo[] ImageFiles;
        int curImgIndex = 0;
        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Stereoscopic Images|*.JPS;*.JPG;*.MPO";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < ofd.FileNames.Length; i++)
                {
                    try
                    {
                        LoadBmpFromFile(ofd.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
            frmPicture_Resize(sender, e);
        }

        private void LoadBmpFromFile(string fileName)
        {
            this.Text = "";

            System.IO.FileInfo fi = new System.IO.FileInfo(fileName);

            if (fi.Extension.ToLower() == ".mpo")
            {
                List<Bitmap> bmps = MPOReader.ReadFromMPF(fileName);
                ImgBmp = MPOReader.AssembleJPS(bmps[0], bmps[1]);
            }
            else
            {
                ImgBmp = new Bitmap(fileName);
            }

            ImageFiles = fi.Directory.GetFiles("*" + fi.Extension);
            aspect = (float)ImgBmp.Width / (float)ImgBmp.Height;

            scale = 1.0;
            paralax = 0;
            tx = 0; ty = 0; txTemp = 0; tyTemp = 0;

            ReloadBmp();
            saved = true;
            this.Text = fileName;

            //Locates current ind position
            for (int ii = 0; ii < ImageFiles.Length; ii++)
            {
                if (ImageFiles[ii].FullName == fileName) curImgIndex = ii;
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (ImageFiles != null && curImgIndex > 0)
            {
                curImgIndex--;
                LoadBmpFromFile(ImageFiles[curImgIndex].FullName);
            }
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (ImageFiles!=null && curImgIndex < ImageFiles.Length - 1)
            {
                curImgIndex++;
                LoadBmpFromFile(ImageFiles[curImgIndex].FullName);
            }
        }

        #endregion

        #region Full Screen


        private void btnFullScreen_Click(object sender, EventArgs e)
        {
            GoFullScreen();
        }

        void GLPic_DoubleClick(object sender, EventArgs e)
        {
            if (this.IsFulScreen) QuitFullScreen();
            else GoFullScreen();
        }

        void GLPic_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                QuitFullScreen();
            }
        }

        /// <summary>Gets information about whether this screen mode is fullscreen</summary>
        private bool IsFulScreen
        {
            get { return this.FormBorderStyle == FormBorderStyle.None; }
        }

        private void QuitFullScreen()
        {
            this.BackColor = Color.Gray;

            toolStrip.Visible = true;

            this.MdiParent.WindowState = FormWindowState.Normal;

            this.MdiParent.Top = 0;
            this.MdiParent.Left = 0;
            this.MdiParent.Width = (int)((float)Screen.PrimaryScreen.WorkingArea.Width * 0.99f);
            this.MdiParent.Height = (int)((float)Screen.PrimaryScreen.WorkingArea.Height * 0.9f);

            this.MdiParent.FormBorderStyle = FormBorderStyle.Sizable;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            frmMain frm = (frmMain)this.MdiParent;
            frm.ShowControls();

        }

        private void GoFullScreen()
        {
            this.MdiParent.WindowState = FormWindowState.Normal;
            this.MdiParent.Top = 0;
            this.MdiParent.Left = -2;
            this.MdiParent.Width = (int)(1.2f*(float)Screen.PrimaryScreen.WorkingArea.Width);
            this.MdiParent.Height = Screen.PrimaryScreen.WorkingArea.Height;
            
            this.WindowState = FormWindowState.Maximized;

            toolStrip.Visible = false;

            this.MdiParent.FormBorderStyle = FormBorderStyle.None;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Black;

            frmMain frm = (frmMain)this.MdiParent;
            frm.HideControls();
            frm.BackColor = Color.Black;
        }

        #endregion

        #region Show/hide advanced options
        /// <summary>Advanced options. Populated with invisible objects in the toolbar</summary>
        List<ToolStripItem> AdvOptions;
        private void btnShowAdvOptions_Click(object sender, EventArgs e)
        {
            if (AdvOptions == null)
            {
                //Populates with invisible controls
                AdvOptions = new List<ToolStripItem>();

                foreach (ToolStripItem c in toolStrip.Items)
                {
                    if (!c.Visible) AdvOptions.Add(c);
                }
            }

            if (AdvOptions.Count > 0)
            {
                if (AdvOptions[0].Visible) foreach (ToolStripItem c in AdvOptions) c.Visible = false;
                else foreach (ToolStripItem c in AdvOptions) c.Visible = true;
            }
        }
        #endregion

        #region Filters - fun stuff :-)
        private void btnFilter_Click(object sender, EventArgs e)
        {
            //btnFilter.DropDownItems.Clear();
            //StereoEditor.CLFilters.Clear();

            if (StereoEditor.CLFilters.Count == 0)
            {
                LoadFilters();
            }
        }
        private void btnFilter_DoubleClick(object sender, EventArgs e)
        {
            LoadFilters();
        }

        private void LoadFilters()
        {
            StereoEditor.CLFilters.Clear();
            btnFilter.DropDownItems.Clear();

            DirectoryInfo di = new DirectoryInfo(Application.StartupPath);
            FileInfo[] filters = di.GetFiles("*.CLFilter");

            foreach (FileInfo fi in filters)
            {
                try
                {
                    StereoEditor.AddCLFilterFromFile(fi.FullName);
                    ToolStripItem t = btnFilter.DropDownItems.Add(fi.Name.Split('.')[0]);
                    t.Click += new EventHandler(t_Click);
                }
                catch
                {

                }
            }

            ToolStripItem tt = btnFilter.DropDownItems.Add("-");
            ToolStripItem tt2 = btnFilter.DropDownItems.Add(lblReLoadFilters.Text);
            tt2.Click += new EventHandler(tt2_Click);
        }

        void tt2_Click(object sender, EventArgs e)
        {
            LoadFilters();
        }

        void t_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripItem t = (ToolStripItem)sender;
                int id = btnFilter.DropDownItems.IndexOf(t);
                ImgBmp = StereoEditor.ApplyFilter(id, ImgBmp);

                ReloadBmp();
            }
            catch(Exception ex)
            {
                MessageBox.Show(lblFilterError.Text + ": " + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Enable/disable drawing when dropdown shows
        private void btnFilter_DropDownOpened(object sender, EventArgs e)
        {
            //timer.Enabled = false;
        }
        private void btnFilter_DropDownOpening(object sender, EventArgs e)
        {
            //timer.Enabled = false;
        }
        private void btnFilter_DropDownClosed(object sender, EventArgs e)
        {
            //timer.Enabled = true;
        }
        #endregion








    }
}
