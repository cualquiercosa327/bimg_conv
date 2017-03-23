using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;

namespace test
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]

        private static byte[] SubArray(byte[] data, long index, long length)
        {
            byte[] result = new byte[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        private static byte[] ReplaceByOffset(byte[] dataToModify, byte[] dataWithReplacer, int startIndex)
        {
            long newSize = Math.Max(dataToModify.Length, (startIndex + dataWithReplacer.Length));
            List<byte> res = new List<byte>();

            for (int i = 0; i < newSize; ++i)
            {
                if (i < startIndex || i >= startIndex + dataWithReplacer.Length)
                {
                    res.Add(dataToModify[i]);
                }
                else
                {
                    res.Add(dataWithReplacer[i - startIndex]);
                }
            }

            byte[] res2 = new byte[newSize];

            res2 = res.ToArray();

            return res2;
        }
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length < 1)
            {
                Application.Run(new Form1());
            }
            else
            {
                byte[] binaryImageFile;
                byte[] rawImageData;
                string defFile = args[0];
                string fileName = args[0];
                if (defFile.Length > 0)
                {
                    string fileType = defFile.Substring(fileName.LastIndexOf('.'));
                    if (fileType == ".bimg")
                    {
                        binaryImageFile = File.ReadAllBytes(fileName);
                        rawImageData = SubArray(binaryImageFile, 0x20, binaryImageFile.Length - 0x20);

                        string w = BitConverter.ToString(new byte[] { binaryImageFile[0x11], binaryImageFile[0x10] });
                        w = w.Replace("-", string.Empty);
                        Int16 width = Int16.Parse(w, System.Globalization.NumberStyles.HexNumber);

                        string h = BitConverter.ToString(new byte[] { binaryImageFile[0x13], binaryImageFile[0x12] });
                        h = h.Replace("-", string.Empty);
                        Int16 height = Int16.Parse(h, System.Globalization.NumberStyles.HexNumber);

                        string bmpFile = defFile.Substring(0, defFile.LastIndexOf('.'));
                        bmpFile += ".bmp";

                        int imgWidth = Convert.ToInt32(width);
                        int imgHeight = Convert.ToInt32(height);
                        Textures.ToBitmap(rawImageData, imgWidth, imgHeight, (Textures.ImageFormat)3).Save(bmpFile);
                    }
                    else
                    {
                        Bitmap theNewImage = new Bitmap(defFile);

                        rawImageData = Textures.FromBitmap(theNewImage, (Textures.ImageFormat)3);


                        string bimgFile = defFile.Substring(0, defFile.LastIndexOf('.'));
                        bimgFile += ".bimg";

                        binaryImageFile = new byte[rawImageData.Length + 0x20];
                        byte[] bimgHeader = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00,
                                          0x00, 0x01, 0x80, 0x00, 0x01, 0x02, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0xe0, 0x2e, 0x06, 0x51};

                        binaryImageFile = ReplaceByOffset(binaryImageFile, bimgHeader, 0x00);
                        binaryImageFile = ReplaceByOffset(binaryImageFile, rawImageData, 0x20);

                        File.WriteAllBytes(bimgFile, binaryImageFile);
                    }
                }
            }

        }
    }
}
