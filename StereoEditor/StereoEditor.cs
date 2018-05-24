using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using OpenCLTemplate;

namespace StereoEditor
{
    /// <summary>Class to process stereo pairs</summary>
    public class StereoEditor
    {
        #region Convert image percentage coordinates into pixel positions

        /// <summary>Gets pixel coordinates from percentage coordinates. Compensates x0L to the right by adding bmp.Width/2</summary>
        private static void GetPixelCoords(Bitmap bmp, PointF P0imgL, PointF P0imgR, PointF dimensions,
            out int x0L, out int y0L, out int x0R, out int y0R, out int dimX, out int dimY)
        {
            //Image dimensions
            int w = bmp.Width / 2;
            int h = bmp.Height;


            //Consistency check
            if (P0imgL.X < 0 || P0imgL.X > 1 || P0imgL.Y < 0 || P0imgL.Y > 1) throw new Exception("P0imgL coordinates must be between 0 and 1");
            if (P0imgR.X < 0 || P0imgR.X > 1 || P0imgR.Y < 0 || P0imgR.Y > 1) throw new Exception("P0imgR coordinates must be between 0 and 1");
            if (dimensions.X < 0 || dimensions.X > 1 || dimensions.Y < 0 || dimensions.Y > 1)
                throw new Exception("dimensions must be between 0 and 1");

            if (P0imgL.X + dimensions.X > 1 || P0imgL.Y + dimensions.Y > 1) throw new Exception("Box in imgL out of bounds");
            if (P0imgR.X + dimensions.X > 1 || P0imgR.Y + dimensions.Y > 1) throw new Exception("Box in imgR out of bounds");

            //Left image is to the right
            x0L = (int)(P0imgL.X * (w - 1)) + w;
            y0L = (int)(P0imgL.Y * (h - 1));

            x0R = (int)(P0imgR.X * (w - 1));
            y0R = (int)(P0imgR.Y * (h - 1));

            dimX = (int)(dimensions.X * w);
            dimY = (int)(dimensions.Y * h);
        }

        #endregion

        #region Extraction of subparts

        /// <summary>Crops a stereo pair and returns a new side-by-side bitmap with the desired region. JPS stereo displays left image to the right</summary>
        /// <param name="bmp">Stereo pair to be cropped. Coordinates go from 0 to 1 in X and Y directions</param>
        /// <param name="P0img1">Initial point in left image, 0 to 1</param>
        /// <param name="P0img2">Initial point in right image, 0 to 1</param>
        /// <param name="dimensions">Width and height of boxes to crop, 0 to 1</param>
        public static Bitmap Crop(Bitmap bmp, PointF P0imgL, PointF P0imgR, PointF dimensions)
        {
            //Image dimensions
            int w = bmp.Width / 2;
            int h = bmp.Height;

            int x0L, y0L, x0R, y0R, newW, newH;
            GetPixelCoords(bmp, P0imgL, P0imgR, dimensions, out x0L, out y0L, out x0R, out y0R, out newW, out newH);


            Bitmap bmpCrop = new Bitmap(2 * newW, newH);

            BitmapData bmdbmp = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            BitmapData bmdCrop = bmpCrop.LockBits(new Rectangle(0, 0, bmpCrop.Width, bmpCrop.Height),
System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                //Right image
                for (int yy = 0; yy < newH; yy++)
                {
                    byte* rowBmp = (byte*)bmdbmp.Scan0 + ((yy + y0R) * bmdbmp.Stride);
                    byte* rowBmpCrop = (byte*)bmdCrop.Scan0 + (yy * bmdCrop.Stride);

                    for (int xx = 0; xx < newW; xx++)
                    {
                        int ind = xx << 2;
                        int ind2 = (xx + x0R) << 2;

                        rowBmpCrop[ind] = rowBmp[ind2];
                        rowBmpCrop[ind + 1] = rowBmp[ind2 + 1];
                        rowBmpCrop[ind + 2] = rowBmp[ind2 + 2];
                        rowBmpCrop[ind + 3] = rowBmp[ind2 + 3];
                    }
                }

                //Left image
                for (int yy = 0; yy < newH; yy++)
                {
                    byte* rowBmp = (byte*)bmdbmp.Scan0 + ((yy + y0L) * bmdbmp.Stride);
                    byte* rowBmpCrop = (byte*)bmdCrop.Scan0 + (yy * bmdCrop.Stride);

                    for (int xx = 0; xx < newW; xx++)
                    {
                        int ind = (newW + xx) << 2;
                        int ind2 = (xx + x0L) << 2;

                        rowBmpCrop[ind] = rowBmp[ind2];
                        rowBmpCrop[ind + 1] = rowBmp[ind2 + 1];
                        rowBmpCrop[ind + 2] = rowBmp[ind2 + 2];
                        rowBmpCrop[ind + 3] = rowBmp[ind2 + 3];
                    }
                }
            }

            bmp.UnlockBits(bmdbmp);
            bmpCrop.UnlockBits(bmdCrop);

            return bmpCrop;
        }

        #endregion

        #region Find optimal parallax

        /// <summary>Maximum number of vertical pixels to use to compute optimal parallax</summary>
        private static int parallaxMaxVertPixels = 100;

        /// <summary>Gets or sets maximum number of vertical pixels to use to compute optimal parallax</summary>
        public static int ParallaxMaximumVerticalPixels
        {
            get { return parallaxMaxVertPixels; }
            set { parallaxMaxVertPixels = value; if (parallaxMaxVertPixels < 1) parallaxMaxVertPixels = 2; }
        }

        /// <summary>Attempts to find optimal parallax in a box region.
        /// Returns optimal displacement to the right of left image (left image is displayed to the right).
        /// Assumes that the center of left image appears on right image</summary>
        /// <param name="bmp">Stereo pair</param>
        /// <param name="bmp">Stereo pair to be cropped. Coordinates go from 0 to 1 in X and Y directions</param>
        /// <param name="P0img1">Initial point in left image, 0 to 1</param>
        /// <param name="P0img2">Initial point in right image, 0 to 1</param>
        /// <param name="dimensions">Width and height of region, 0 to 1</param>
        public static float ParallaxFind(Bitmap bmp, PointF P0imgL, PointF P0imgR, PointF dimensions, out bool succeeded)
        {
            //Gets pixel coordinates
            int x0L, y0L, x0R, y0R, regionW, regionH;
            GetPixelCoords(bmp, P0imgL, P0imgR, dimensions, out x0L, out y0L, out x0R, out y0R, out regionW, out regionH);

            //Number of vertical pixels
            int numVertPix = regionH >> 2;
            if (numVertPix > (regionW >> 2)) numVertPix = regionW >> 2;
            if (numVertPix > parallaxMaxVertPixels) numVertPix = parallaxMaxVertPixels;
            
            //Central y pixels
            int yCenterR = (y0R + regionH) >> 1;
            int yCenterL = (y0L + regionH) >> 1;

            //Y coordinates to use
            int yMinR = yCenterR - (numVertPix >> 1);
            int yMaxR = yCenterR + (numVertPix >> 1);
            int yMinL = yCenterL - (numVertPix >> 1);
            int yMaxL = yCenterL + (numVertPix >> 1);
            numVertPix = yMaxR - yMinR;

            //Copies image data to byte vectors. BGR format
            byte[] LeftPixels = new byte[3 * numVertPix * numVertPix];
            byte[] RightPixels = new byte[3 * regionW * numVertPix];

            BitmapData bmdbmp = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                //Left image
                int indToCenter = (regionW >> 1)-(numVertPix>>1);
                for (int yy = 0; yy < numVertPix; yy++)
                {
                    byte* rowBmp = (byte*)bmdbmp.Scan0 + ((yy + yMinL) * bmdbmp.Stride);

                    for (int xx = 0; xx < numVertPix; xx++)
                    {
                        int ind = (xx + x0L + indToCenter) << 2;

                        int indPix = 3 * (xx + yy * numVertPix);

                        LeftPixels[indPix] = rowBmp[ind];
                        LeftPixels[indPix + 1] = rowBmp[ind + 1];
                        LeftPixels[indPix + 2] = rowBmp[ind + 2];
                    }
                }

                //Right image
                for (int yy = 0; yy < numVertPix; yy++)
                {
                    byte* rowBmp = (byte*)bmdbmp.Scan0 + ((yy + yMinR) * bmdbmp.Stride);

                    for (int xx = 0; xx < regionW; xx++)
                    {
                        int ind = (xx + x0R) << 2;

                        int indPix = 3 * (xx + yy * regionW);

                        RightPixels[indPix] = rowBmp[ind];
                        RightPixels[indPix + 1] = rowBmp[ind + 1];
                        RightPixels[indPix + 2] = rowBmp[ind + 2];
                    }
                }
            }
            bmp.UnlockBits(bmdbmp);


            //Needs to find minimum of function BytesDiff(offset) ie, argmin of Bytesdiff(offset)
            int passes = 3;
            int[] pointsPerPass = new int[] { (int)(100.0f * (float)regionW / (float)regionH), 26, 6 };

            

            succeeded = false;

            //Initial search region. passes should be negligible compared to the offset values
            int offset0 = 2*passes;
            int offsetF = regionW - numVertPix - 2*passes - 1;

            int minOffset = 0;
            for (int k = 0; k < passes; k++)
            {
                float invPtsPerPass = 1.0f / (float)pointsPerPass[k];

                float minDiff = BytesDiff(LeftPixels, RightPixels, numVertPix, regionW, offset0);
                minOffset = offset0;

                for (int i = 1; i <= pointsPerPass[k]; i++)
                {
                    int ind = (int)(offset0 + (offsetF - offset0) * (float)i * invPtsPerPass);
                    float diff = BytesDiff(LeftPixels, RightPixels, numVertPix, regionW, ind);

                    if (diff < minDiff)
                    {
                        minDiff = diff;
                        minOffset = ind;
                    }
                }

                
                int temp = (int)(minOffset - (offsetF - offset0) * invPtsPerPass);
                if (temp > offset0) offset0 = temp;

                int temp2 = (int)(minOffset + (offsetF - offset0) * invPtsPerPass);
                if (temp2 < offsetF) offsetF = temp2;

                if (temp >= offset0 && temp2 <= offsetF) succeeded = true;
            }

            ////REMOVE
            //float[] dif = new float[regionW - numVertPix];
            //int indMin = 0; float min = 1000;
            //for (int i = 0; i < regionW - numVertPix; i++)
            //{
            //    dif[i] = BytesDiff(LeftPixels, RightPixels, numVertPix, regionW, i);
            //    if (min > dif[i] || i==0)
            //    {
            //        min = dif[i];
            //        indMin = i;
            //    }
            //}

            return (float)((regionW - numVertPix) / 2 - minOffset) / ((float)bmp.Width * 0.5f);
        }

        /// <summary>Computes byte difference between a h x h box and a wImg x h image</summary>
        /// <param name="boxBytes">Box bytes, RGB, dimension = 3*h*h</param>
        /// <param name="refImg">Image bytes, RGB, dimension = 3 * imgW * h</param>
        /// <param name="h">Image height, size of box</param>
        /// <param name="wImg">Image width</param>
        /// <param name="offset">Amount of bytes to displace box to the right. Can go up to wImg-h</param>
        private static float BytesDiff(byte[] boxBytes, byte[] refImg, int h, int wImg, int offset)
        {
            float totaldiff = 0;
            for (int x = 0; x < h; x++)
            {
                float coldiff = 0;
                for (int y = 0; y < h; y++)
                {
                    int ind1 = 3 * (x + h * y);
                    int ind2 = 3 * (x + offset + wImg * y);

                    float colordiff1 = Math.Abs(boxBytes[ind1] - refImg[ind2]);
                    float colordiff2 = Math.Abs(boxBytes[1 + ind1] - refImg[1 + ind2]);
                    float colordiff3 = Math.Abs(boxBytes[2 + ind1] - refImg[2 + ind2]);

                    colordiff1 = Math.Max(colordiff1, colordiff2);
                    colordiff1 = Math.Max(colordiff1, colordiff3);

                    byte b = (byte)colordiff1;
                    coldiff += b;
                }
                totaldiff += coldiff;
            }

            return totaldiff;
        }

        #endregion

        #region Filtering using OpenCL

        /// <summary>OpenCL filter. Contains name and compiled kernel</summary>
        public class CLFilter
        {
            /// <summary>Filter name</summary>
            public string FilterName;
            /// <summary>Filter kernel</summary>
            public CLCalc.Program.Kernel FilterKernel;

            /// <summary>Creates a new filter from a given code. Filter kernel name has to be the same as kernel name 
            /// (disregarding spaces - Ex: Kernel is GaussianBlur and filter name is Gaussian Blur)</summary>
            /// <param name="filterCode">Complete OpenCL kernel code to compile</param>
            /// <param name="filterName">Filter name</param>
            public CLFilter(string filterCode, string filterName)
            {
                if (CLCalc.CLAcceleration == CLCalc.CLAccelerationType.Unknown) CLCalc.InitCL(Cloo.ComputeDeviceTypes.Gpu);
                if (CLCalc.CLAcceleration != CLCalc.CLAccelerationType.UsingCL)
                    throw new Exception("OpenCL not available");

                CLCalc.Program.Compile(filterCode);

                this.FilterName = filterName;
                string kernelname = FilterName.Replace(" ","").ToLower();

                FilterKernel = new CLCalc.Program.Kernel(kernelname);
            }

            /// <summary>Creates a new filter from a given compiled kernel.</summary>
            /// <param name="filterCode">Code to compile</param>
            /// <param name="filterName">Filter name</param>
            public CLFilter(CLCalc.Program.Kernel filterKernel, string filterName)
            {
                if (CLCalc.CLAcceleration == CLCalc.CLAccelerationType.Unknown) CLCalc.InitCL();
                if (CLCalc.CLAcceleration != CLCalc.CLAccelerationType.UsingCL)
                    throw new Exception("OpenCL not available");

                this.FilterName = filterName;
                this.FilterKernel = filterKernel;
            }

            /// <summary>Creates an OpenCL kernel code to compile</summary>
            /// <param name="kernelName">Kernel name</param>
            /// <param name="CoreCode">User code which reads (float4)P[][] RGBA and outputs (float4)outP RGBA, colors from 0 to 1</param>
            public static string CreateKernelCode(string kernelName, string CoreCode)
            {
                string kernelVoid = "__kernel void " + kernelName.Replace(" ", "").ToLower() + " ";

                string header = kernelVoid + @"
                      (__read_only  image2d_t img1, 
                       __write_only image2d_t img2)
{
  const sampler_t smp = CLK_NORMALIZED_COORDS_FALSE | //Natural coordinates
    CLK_ADDRESS_CLAMP | //Clamp to zeros
    CLK_FILTER_NEAREST; //Don't interpolate


  int x0 = get_global_id(0);
  int y0 = get_global_id(1);
  
  int2 coord = (int2)(x0+3, y0+3);

  uint4 val = (uint4)(0,0,0,0);
  for (int i = 0; i < 7; i++)
  {
      for (int j = 0; j < 7;  j++)
      {
         coord = (int2)(x0+i, y0+j);
         val += read_imageui(img1, smp, coord);
      }
  }

  uint4 pixel;
  float4 P[7][7];
  for (int i = 0; i < 7; i++)
  {
      for (int j = 0; j < 7;  j++)
      {
         coord = (int2)(x0+i, y0+j);
         pixel = read_imageui(img1, smp, coord);
         //Converts to RGBA format
         P[i][j] = (float4)((float)pixel.z, (float)pixel.y, (float)pixel.x, (float)pixel.w);
         P[i][j] *= 0.00392156862745098f; //Normalize to 0-1
      }
  }
  float4 outP = P[2][2];

  float xCoord = (float)x0/(float)get_global_size(0);
  float yCoord = (float)y0/(float)get_global_size(1);

";

                //User manipulates P[][].
                //User has to know that outP is the output pixel

                string footer = @"

  coord.x = x0+3;coord.y = y0+3;
  outP *= 255.0f;
  //outputs C# BGRA color format from RGBA
  uint4 writeValue = (uint4)((uint)outP.z, (uint)outP.y, (uint)outP.x, (uint)outP.w);
  
  //Some GPUs appear to have problems with Clamp function in OpenCL
  writeValue.x = writeValue.x < 0 ? 0 : writeValue.x;
  writeValue.x = writeValue.x > 255 ? 255 : writeValue.x;
  writeValue.y = writeValue.y < 0 ? 0 : writeValue.y;
  writeValue.y = writeValue.y > 255 ? 255 : writeValue.y;
  writeValue.z = writeValue.z < 0 ? 0 : writeValue.z;
  writeValue.z = writeValue.z > 255 ? 255 : writeValue.z;
  writeValue.w = writeValue.w < 0 ? 0 : writeValue.w;
  writeValue.w = writeValue.w > 255 ? 255 : writeValue.w;
  
  write_imageui(img2, coord, writeValue);
}
";
                return header + CoreCode + footer;
            }
        }
        /// <summary>List of currently available filters</summary>
        public static List<CLFilter> CLFilters = new List<CLFilter>();

        /// <summary>Creates a new OpenCL filter from file</summary>
        /// <param name="FileName">File to use</param>
        public static void AddCLFilterFromFile(string FileName)
        {
            FileInfo fi = new FileInfo(FileName);
            if (!fi.Exists) throw new Exception("File not found");

            string kernelName = fi.Name.Split('.')[0];
            string userCode = "";
            using (StreamReader sr = new StreamReader(FileName))
            {
                userCode = sr.ReadToEnd();
            }

            AddCLFilterFromCode(kernelName, userCode);
        }

        /// <summary>Creates a new OpenCL filter from a core code which reads (float4)P[][] RGBA and outputs (float4)outP RGBA, colors from 0 to 1</summary>
        /// <param name="CoreCode">Kernel core code</param>
        public static void AddCLFilterFromCode(string kernelName, string CoreCode)
        {
            string fullCode = CLFilter.CreateKernelCode(kernelName, CoreCode);

            CLFilter filter = new CLFilter(fullCode, kernelName);
            CLFilters.Add(filter);
        }

        /// <summary>Apply filter to an image</summary>
        /// <param name="id">Filter index, CLFilters[id], to apply</param>
        /// <param name="bmp">Bitmap to be processes</param>
        public static Bitmap ApplyFilter(int id, Bitmap bmp)
        {
            //if (bmp.Width < 4096)
            //{
            //    CLCalc.Program.Image2D CLImgSrc = new CLCalc.Program.Image2D(bmp);
            //    CLCalc.Program.Image2D CLImgDst = new CLCalc.Program.Image2D(bmp);
            //    CLCalc.Program.MemoryObject[] args = new CLCalc.Program.MemoryObject[] { CLImgSrc, CLImgDst };

            //    CLFilters[id].FilterKernel.Execute(args, new int[] { bmp.Width - 7, bmp.Height - 7 });

            //    return CLImgDst.ReadBitmap();
            //}
            //else
            //{
                //Pictures can be too big; it's necessary to split
                List<Bitmap> bmps = MPOReader.SplitJPS(bmp);

                CLCalc.Program.Image2D CLImgSrc0 = new CLCalc.Program.Image2D(bmps[0]);
                CLCalc.Program.Image2D CLImgDst0 = new CLCalc.Program.Image2D(bmps[0]);
                CLCalc.Program.MemoryObject[] args0 = new CLCalc.Program.MemoryObject[] { CLImgSrc0, CLImgDst0 };

                CLFilters[id].FilterKernel.Execute(args0, new int[] { bmps[0].Width - 7, bmps[0].Height - 7 });

                CLCalc.Program.Image2D CLImgSrc1 = new CLCalc.Program.Image2D(bmps[1]);
                CLCalc.Program.Image2D CLImgDst1 = new CLCalc.Program.Image2D(bmps[1]);
                CLCalc.Program.MemoryObject[] args1 = new CLCalc.Program.MemoryObject[] { CLImgSrc1, CLImgDst1 };

                CLFilters[id].FilterKernel.Execute(args1, new int[] { bmps[1].Width - 7, bmps[1].Height - 7 });

                Bitmap bmpL = CLImgDst0.ReadBitmap();
                Bitmap bmpR = CLImgDst1.ReadBitmap();

                return MPOReader.AssembleJPS(bmpL, bmpR);
            //}
        }

        #endregion
    }
}
