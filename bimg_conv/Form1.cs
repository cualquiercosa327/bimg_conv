using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace test
{
    public partial class Form1 : Form
    {
        byte[] binaryImageFile;
        byte[] rawImageData;

        public Form1()
        {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            refresh();
        }

        public static byte[] SubArray(byte[] data, long index, long length)
        {
            byte[] result = new byte[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static byte[] ReplaceByOffset(byte[] dataToModify, byte[] dataWithReplacer, int startIndex)
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            refresh();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            refresh();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void refresh()
        {
            if (binaryImageFile != null && textBox1.Text != "" && textBox2.Text != "") { 
                int imgWidth = Convert.ToInt32(textBox1.Text);
                int imgHeight = Convert.ToInt32(textBox2.Text);
                if (panel2.Width < imgWidth) panel2.Width = imgWidth;
                if (panel2.Height < imgHeight) panel2.Height = imgHeight;
                try { 
                    pictureBox1.Image = Textures.ToBitmap(rawImageData, imgWidth, imgHeight, (Textures.ImageFormat)listBox1.SelectedIndex, false);
                }
                catch
                {
                    MessageBox.Show("Bad format, width or height. Please change.");
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog asd = new OpenFileDialog();
            
            if (asd.ShowDialog() == DialogResult.OK) { 
                binaryImageFile = File.ReadAllBytes(asd.FileName);
                rawImageData = SubArray(binaryImageFile, 0x20, binaryImageFile.Length - 0x20);
            
                string w = BitConverter.ToString(new byte[] { binaryImageFile[0x11], binaryImageFile[0x10] });
                w = w.Replace("-", string.Empty);
                Int16 width = Int16.Parse(w, System.Globalization.NumberStyles.HexNumber);

                string h = BitConverter.ToString(new byte[] { binaryImageFile[0x13], binaryImageFile[0x12] });
                h = h.Replace("-", string.Empty);
                Int16 height = Int16.Parse(h, System.Globalization.NumberStyles.HexNumber);

                textBox1.Text = width.ToString();
                textBox2.Text = height.ToString();

                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;

                listBox1.SelectedIndex = 3;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            binaryImageFile = ReplaceByOffset(binaryImageFile, rawImageData, 0x20);

            SaveFileDialog asd = new SaveFileDialog();
            asd.ShowDialog();

            File.WriteAllBytes(asd.FileName, binaryImageFile);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            int i;
            foreach (Textures.ImageFormat eValue in Enum.GetValues(typeof(Textures.ImageFormat)))
            {
                listBox1.Items.Add(eValue);
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog asd = new OpenFileDialog();
            asd.Filter = "PNG Files (*.png)|*.png|BMP Files (*.bmp)|*.bmp|JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif|All Files|*";

            if (asd.ShowDialog() == DialogResult.OK)
            {
                int imgWidth = Convert.ToInt32(textBox1.Text);
                int imgHeight = Convert.ToInt32(textBox2.Text);

                Bitmap theNewImage = new Bitmap(asd.FileName);

                if (theNewImage.Width == imgWidth && theNewImage.Height == imgHeight) { 
                    rawImageData = Textures.FromBitmap(theNewImage, (Textures.ImageFormat) listBox1.SelectedIndex);
                    refresh();
                }
                else
                {
                    MessageBox.Show("The image isn't the same size as the original!");
                }
            }

            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog asd = new SaveFileDialog();
            asd.Filter = "PNG Files (*.png)|*.png|BMP Files (*.bmp)|*.bmp|JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif|All Files|*";

            if (asd.ShowDialog() == DialogResult.OK)
            {
                int imgWidth = Convert.ToInt32(textBox1.Text);
                int imgHeight = Convert.ToInt32(textBox2.Text);
                Textures.ToBitmap(rawImageData, imgWidth, imgHeight, (Textures.ImageFormat)listBox1.SelectedIndex).Save(asd.FileName);
            }
        }
    }
}
