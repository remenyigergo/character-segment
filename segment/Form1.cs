using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace segment
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }




        public static List<KeyValuePair<int, int>> lineSpace_FromTo = new List<KeyValuePair<int, int>>();
        public static List<KeyValuePair<int, int>> lines_FromTo;
        public static LinkedList<KeyValuePair<int, Color[,]>> feherSorok = new LinkedList<KeyValuePair<int, Color[,]>>();
        public static LinkedList<KeyValuePair<int, Color[,]>> szovegSorok = new LinkedList<KeyValuePair<int, Color[,]>>();

        class Segment
        {
            public Bitmap Image;

            public void ReadImage()
            {
                Image = new Bitmap(@"C:\Iskola\Debreceni Egyetem\felev2\Kepfeldolgozas\beadandók\4\kepkicsi.png");
            }


            public bool WhiteRow(int row)
            {
                for (int j = 0; j < Image.Width; j++)
                {
                    if (Image.GetPixel(j, row).R != 255 || Image.GetPixel(j, row).G != 255 || Image.GetPixel(j, row).B != 255)
                    {
                        return false;  //ekkor nem teljesen fehér a sor
                    }
                }

                return true;  //fehér sor
            }

            public void ScanImage()
            {

                for (int row = 0; row < Image.Height; row++)
                {
                    bool IsWhite = WhiteRow(row);

                    Color[,] sor = new Color[1, Image.Size.Width];
                    for (int i = 0; i < Image.Width; i++)
                    {
                        sor[0, i] = Image.GetPixel(i, row);
                    }

                    KeyValuePair<int, Color[,]> kv = new KeyValuePair<int, Color[,]>(row, sor);

                    if (IsWhite)
                    {
                        feherSorok.AddLast(kv);
                    }  else
                    {
                        szovegSorok.AddLast(kv);
                    }
                }
            }

            public void PaintRows()
            {
                foreach (var sor in feherSorok)
                {
                    for (int j = 0; j < Image.Width; j++)
                    {
                        Image.SetPixel(j, sor.Key, Color.Red);
                    }
                }

                foreach (var sor in szovegSorok)
                {
                    for (int j = 0; j < Image.Width; j++)
                    {
                        //Image.SetPixel(j, sor.Key, Color.Blue);
                    }
                }
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Segment Segmentation = new Segment();
            Segmentation.ReadImage();
            Segmentation.ScanImage();
            Segmentation.PaintRows();

            pictureBox1.Image = Segmentation.Image;
            this.Size = new Size(Segmentation.Image.Width + 100, Segmentation.Image.Height + 100);
            pictureBox1.Size = new Size(Segmentation.Image.Width, Segmentation.Image.Height);
            pictureBox1.Image = Segmentation.Image;

        }
    }
}
