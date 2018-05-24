using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace StereoEditor
{
    public static class MPOReader
    {
        #region Read file and retrieve Codec
        /// <summary>
        /// Reads data from a stream until the end is reached. The
        /// data is returned as a byte array. An IOException is
        /// thrown if any of the underlying IO calls fail.
        /// </summary>
        /// <param name="stream">The stream to read data from</param>
        /// <param name="initialLength">The initial buffer length</param>
        private static byte[] ReadFully(Stream stream, int initialLength)
        {
            //EXTRACTED FROM
            //http://www.yoda.arachsys.com/csharp/readbinary.html
            //Unknown author

            // If we've been passed an unhelpful initial length, just
            // use 32K.
            if (initialLength < 1)
            {
                initialLength = 32768;
            }

            byte[] buffer = new byte[initialLength];
            int read = 0;

            int chunk;
            while ((chunk = stream.Read(buffer, read, buffer.Length - read)) > 0)
            {
                read += chunk;

                // If we've reached the end of our buffer, check to see if there's
                // any more information
                if (read == buffer.Length)
                {
                    int nextByte = stream.ReadByte();

                    // End of stream? If so, we're done
                    if (nextByte == -1)
                    {
                        return buffer;
                    }

                    // Nope. Resize the buffer, put in the byte we've just
                    // read, and continue
                    byte[] newBuffer = new byte[buffer.Length * 2];
                    Array.Copy(buffer, newBuffer, buffer.Length);
                    newBuffer[read] = (byte)nextByte;
                    buffer = newBuffer;
                    read++;
                }
            }
            // Buffer is now too big. Shrink it.
            byte[] ret = new byte[read];
            Array.Copy(buffer, ret, read);
            return ret;
        }

        private static System.Drawing.Imaging.ImageCodecInfo GetEncoder(System.Drawing.Imaging.ImageFormat format)
        {

            //System.Drawing.Imaging.ImageCodecInfo[] codecs = System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders();
            System.Drawing.Imaging.ImageCodecInfo[] codecs = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();

            foreach (System.Drawing.Imaging.ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        #endregion

        #region Read MPO files and assemble/split side-by-side pairs
        /// <summary>Opens a Multi Picture File .MPO or .MPF and returns the bitmaps contained within</summary>
        /// <param name="FileName">MPO File to read</param>
        public static List<Bitmap> ReadFromMPF(string FileName)
        {
            byte[] b = new byte[] { 255, 216, 255, 225, 224, 226 };
            byte[] arq = null;
            using (FileStream fs = new FileStream(FileName, FileMode.Open))
            {
                arq = ReadFully(fs, 0);

                fs.Close();
            }

            List<int> indArqs = new List<int>();

            for (int i = 0; i < arq.Length - 3; i++)
            {
                if (b[0] == arq[i] && b[1] == arq[i + 1] && b[2] == arq[i + 2] && (b[3] == arq[i + 3] || b[4] == arq[i + 3] || b[5] == arq[i + 3]))
                {
                    indArqs.Add(i);
                }
            }
            indArqs.Add(arq.Length - indArqs[indArqs.Count - 1]);

            //Decodes and writes bitmaps
            List<Bitmap> resp = new List<Bitmap>();
            for (int i = 0; i < indArqs.Count - 1; i++)
            {
                MemoryStream str = new MemoryStream(arq, indArqs[i], indArqs[i + 1]);
                resp.Add(new Bitmap(str));
            }

            return resp;
        }

        ///// <summary>Pixel size = 4</summary>
        //private static int PIXELSIZE = 4;

        /// <summary>Builds a side-by-side image using left and right stereo pairs. Assembles right image to the left</summary>
        public static Bitmap AssembleJPS(Bitmap Left, Bitmap Right)
        {
            BitmapData bmdleft = Left.LockBits(new Rectangle(0, 0, Left.Width, Left.Height),
System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            BitmapData bmdright = Right.LockBits(new Rectangle(0, 0, Right.Width, Right.Height),
System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            Bitmap resp = new Bitmap(Left.Width + Right.Width, Math.Max(Left.Height, Right.Height));

            BitmapData bmdbmp = resp.LockBits(new Rectangle(0, 0, resp.Width, resp.Height),
System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                int rightOffset = Right.Width <<2;

                //Right image
                for (int yy = 0; yy < Right.Height; yy++)
                {
                    byte* rowBmp = (byte*)bmdbmp.Scan0 + (yy * bmdbmp.Stride);

                    byte* rowBmpRight = (byte*)bmdright.Scan0 + (yy * bmdright.Stride);
                    for (int xx = 0; xx < Right.Width; xx++)
                    {
                        int ind = xx <<2;
                        rowBmp[ind] = rowBmpRight[ind];
                        rowBmp[ind + 1] = rowBmpRight[ind + 1];
                        rowBmp[ind + 2] = rowBmpRight[ind + 2];
                        rowBmp[ind + 3] = rowBmpRight[ind + 3];
                    }
                }

                //Left image
                for (int yy = 0; yy < Left.Height; yy++)
                {
                    byte* rowBmp = (byte*)bmdbmp.Scan0 + (yy * bmdbmp.Stride);
                    byte* rowBmpLeft = (byte*)bmdleft.Scan0 + (yy * bmdleft.Stride);
                    for (int xx = 0; xx < Left.Width; xx++)
                    {
                        int ind = xx <<2;
                        rowBmp[rightOffset + ind] = rowBmpLeft[ind];
                        rowBmp[rightOffset + ind + 1] = rowBmpLeft[ind + 1];
                        rowBmp[rightOffset + ind + 2] = rowBmpLeft[ind + 2];
                        rowBmp[rightOffset + ind + 3] = rowBmpLeft[ind + 3];
                    }
                }
            }

            Left.UnlockBits(bmdleft);
            Right.UnlockBits(bmdright);
            resp.UnlockBits(bmdbmp);


            return resp;
        }

        /// <summary>Splits a side-by-side image into left-right pairs - [0] is left, [1] is right. Note: in a JPS the right image stays to the left</summary>
        /// <param name="bmp">Bitmap to split</param>
        public static List<Bitmap> SplitJPS(Bitmap bmp)
        {
            int w = bmp.Width / 2;
            int h = bmp.Height;

            Bitmap Right = new Bitmap(w, h);
            Bitmap Left = new Bitmap(w, h);

            BitmapData bmdleft = Left.LockBits(new Rectangle(0, 0, Left.Width, Left.Height),
System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            BitmapData bmdright = Right.LockBits(new Rectangle(0, 0, Right.Width, Right.Height),
System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            BitmapData bmdbmp = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                int rightOffset = w <<2;

                //Right image
                for (int yy = 0; yy < h; yy++)
                {
                    byte* rowBmp = (byte*)bmdbmp.Scan0 + (yy * bmdbmp.Stride);

                    byte* rowBmpRight = (byte*)bmdright.Scan0 + (yy * bmdright.Stride);
                    for (int xx = 0; xx < w; xx++)
                    {
                        int ind = xx <<2;
                        rowBmpRight[ind] = rowBmp[ind];
                        rowBmpRight[ind + 1] = rowBmp[ind + 1];
                        rowBmpRight[ind + 2] = rowBmp[ind + 2];
                        rowBmpRight[ind + 3] = rowBmp[ind + 3];
                    }

                    //Left image
                    byte* rowBmpLeft = (byte*)bmdleft.Scan0 + (yy * bmdleft.Stride);
                    for (int xx = 0; xx < w; xx++)
                    {
                        int ind = xx <<2;
                        rowBmpLeft[ind] = rowBmp[rightOffset + ind];
                        rowBmpLeft[ind + 1] = rowBmp[rightOffset + ind + 1];
                        rowBmpLeft[ind + 2] = rowBmp[rightOffset + ind + 2];
                        rowBmpLeft[ind + 3] = rowBmp[rightOffset + ind + 3];
                    }
                }
            }

            bmp.UnlockBits(bmdbmp);
            Left.UnlockBits(bmdleft);
            Right.UnlockBits(bmdright);

            return new List<Bitmap>() { Left, Right };
        }

        /// <summary>Saves a list of bitmaps into a Multipicture File. Not working.</summary>
        /// <param name="FileName">File to save to</param>
        /// <param name="bmps">List of bitmaps to write</param>
        private static void WriteMPF(string FileName, List<Bitmap> bmps)
        {
            if (System.IO.File.Exists(FileName))
            {
                System.IO.File.Delete(FileName);
            }

            System.Drawing.Imaging.Encoder Encoder = System.Drawing.Imaging.Encoder.Quality;
            System.Drawing.Imaging.EncoderParameters myEncoderParameters = new System.Drawing.Imaging.EncoderParameters(1);
            System.Drawing.Imaging.EncoderParameter myEncoderParameter = new System.Drawing.Imaging.EncoderParameter(Encoder, 97L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            System.Drawing.Imaging.ImageCodecInfo jgpEncoder = GetEncoder(System.Drawing.Imaging.ImageFormat.Jpeg);

            foreach (Bitmap bmp in bmps)
            {

                using (FileStream fstr = new FileStream(FileName, FileMode.Append))
                {
                    bmp.Save(fstr, jgpEncoder, myEncoderParameters);
                    fstr.Close();
                }
            }
        }
        #endregion

        #region File Save as JPG and conversion from MPO to JPG
        /// <summary>Saves image to a file</summary>
        /// <param name="bmp">Bitmap to save</param>
        /// <param name="FileName">File name</param>
        /// <param name="Quality">Image quality, from 0 to 100. 97 should do fine</param>
        public static void Save(Bitmap bmp, string FileName, int Quality)
        {
            Int64 quality = (Int64)Quality;

            System.Drawing.Imaging.Encoder Encoder = System.Drawing.Imaging.Encoder.Quality;
            System.Drawing.Imaging.EncoderParameters myEncoderParameters = new System.Drawing.Imaging.EncoderParameters(1);
            System.Drawing.Imaging.EncoderParameter myEncoderParameter = new System.Drawing.Imaging.EncoderParameter(Encoder, quality);
            myEncoderParameters.Param[0] = myEncoderParameter;
            System.Drawing.Imaging.ImageCodecInfo jgpEncoder = GetEncoder(System.Drawing.Imaging.ImageFormat.Jpeg);

            bmp.Save(FileName, jgpEncoder, myEncoderParameters);
        }

        /// <summary>Number of files converted so far</summary>
        public static int QtdConverted = 0;
        /// <summary>Number of files to be converted</summary>
        public static int TotalToBeConverted = 0;

        /// <summary>Converts all .MPO files in a folder to JPS. Useful function for multithreading</summary>
        /// <param name="args">[0]-directory, [1]-Desired extension, [2]-nthreads, [3] quality, [4] leftright</param>
        public static void ConvertFolderMPOtoJPG(object args)
        {
            object[] argss = (object[])args;
            ConvertFolderMPOtoJPG((string)argss[0], (string)argss[1], (int)argss[2], (int)argss[3], (bool)argss[4]);
        }

        /// <summary>Converts all .MPO files in a folder to JPS</summary>
        /// <param name="Directory">Directory to read .MPO files from</param>
        public static void ConvertFolderMPOtoJPG(string Directory)
        {
            ConvertFolderMPOtoJPG(Directory, ".JPS", 16, 97, false);
        }

        /// <summary>Converts all .MPO files in a folder to JPS</summary>
        /// <param name="Directory">Directory to read .MPO files from</param>
        /// <param name="Extension">Extension to put in resulting files. Usually .JPS or .JPG</param>
        /// <param name="nThreads">Number of threads to use to convert in background. Set to 0 for serial</param>
        /// <param name="Quality">Image quality. 97 usually is OK</param>
        /// <param name="LeftRight">Convert to 2 JPG images (left/right) ?</param>
        public static void ConvertFolderMPOtoJPG(string Directory, string Extension, int nThreads, int Quality, bool LeftRight)
        {
            QtdConverted = 0;
            DirectoryInfo di = new DirectoryInfo(Directory);

            if (!Extension.StartsWith(".")) Extension = "." + Extension;

            if (di.Exists)
            {
                FileInfo[] fis = di.GetFiles("*.MPO");
                TotalToBeConverted = fis.Length;

                if (nThreads == 0)
                {
                    int i = 0;
                    foreach (FileInfo fi in fis)
                    {
                        object[] args = new object[3];
                        args[0] = fi.FullName; args[1] = Extension; args[2] = Quality;

                        if (LeftRight) ConvertMPOtoLeftRightPair(args);
                        else ConvertMPO(args);

                        i++;
                        QtdConverted = i;
                    }
                }
                else
                {
                    try
                    {
                        System.Threading.Thread[] t = new System.Threading.Thread[nThreads];
                        int curt = 0;

                        int i = 0;
                        while (i < fis.Length)
                        {
                            if (t[curt] == null || t[curt].IsAlive == false)
                            {
                                if (LeftRight) t[curt] = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ConvertMPOtoLeftRightPair));
                                else t[curt] = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ConvertMPO));

                                object[] args = new object[3];
                                args[0] = fis[i].FullName; args[1] = Extension; args[2] = Quality;

                                t[curt].Start(args);

                                while (!t[curt].IsAlive)
                                {
                                }

                                i++;
                                QtdConverted = i;
                            }
                            curt++;
                            if (curt >= nThreads)
                            {
                                curt = 0;
                            }
                            System.Threading.Thread.Sleep(10);
                        }

                        for (int ii = 0; ii < nThreads; ii++)
                        {
                            if (t[ii] != null) t[ii].Join();
                        }

                        QtdConverted = i;
                    }
                    catch
                    {
                    }

                }
            }

            TotalToBeConverted = 0;
        }

        /// <summary>Converts a .MPO file to a .JPG or .JPS</summary>
        /// <param name="args">args = object[2], args[0] = file full name, args[1] = desired extension - .JPS or .JPG, include dot. args[2] = int quality</param>
        public static void ConvertMPO(object args)
        {
            object[] s = (object[])args;
            int quality = (int)s[2];

            string outName = ((string)s[0]).Substring(0, ((string)s[0]).Length - 4) + s[1];

            List<Bitmap> bmps = ReadFromMPF((string)s[0]);

            if (bmps.Count != 2) throw new Exception("Stereoscopic pair not found");

            Save(AssembleJPS(bmps[0], bmps[1]), outName, quality);
        }

        /// <summary>Converts a .MPO file to .JPG pair</summary>
        /// <param name="args">args = object[2], args[0] = file full name, args[1] = desired extension - .JPS or .JPG, include dot. args[2] = int quality</param>
        public static void ConvertMPOtoLeftRightPair(object args)
        {
            object[] s = (object[])args;
            int quality = (int)s[2];

            string outName_R = ((string)s[0]).Substring(0, ((string)s[0]).Length - 4) + "_R.JPG";
            string outName_L = ((string)s[0]).Substring(0, ((string)s[0]).Length - 4) + "_L.JPG";

            List<Bitmap> bmps = ReadFromMPF((string)s[0]);

            if (bmps.Count != 2) throw new Exception("Stereoscopic pair not found");

            Save(bmps[0], outName_L, quality);
            Save(bmps[1], outName_R, quality);
        }

        #endregion

    }
}
