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


        enum RowType
        {
            White,
            Text
        }

        public static LinkedList<KeyValuePair<int, Color[,]>> feherSorok = new LinkedList<KeyValuePair<int, Color[,]>>();
        public static LinkedList<KeyValuePair<int, Color[,]>> szovegSorok = new LinkedList<KeyValuePair<int, Color[,]>>();

        public static List<KeyValuePair<int, int>> feherSorokPozicio = new List<KeyValuePair<int, int>>();
        public static List<KeyValuePair<int, int>> szovegSorokPozicio = new List<KeyValuePair<int, int>>();

        public static LinkedList<KeyValuePair<int, KeyValuePair<int, int>>> betuPoziciok = new LinkedList<KeyValuePair<int, KeyValuePair<int, int>>>();
        public static LinkedList<KeyValuePair<KeyValuePair<int, int>, KeyValuePair<int, int>>> betuSpacePoziciok = new LinkedList<KeyValuePair<KeyValuePair<int, int>, KeyValuePair<int, int>>>();

        class Segment
        {
            public Bitmap Image;
            public int row2 = 0;
            public int Column_To = 0;
            public bool FirstColCheck = true;
            public int processingRowIndex = 0;

            public void ReadImage()
            {
                Image = new Bitmap(@"F:\Iskola\DebreceniEgyetem\MSc\felev2\Kepfeldolgozas\character-segment\szoveg.png");
            }


            public bool IsRowWhite(int row)
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

            public bool WhiteCol(int row, int row2, int col)
            {
                try
                {
                    for (int j = row; j < row2; j++)
                    {
                        if (Image.GetPixel(col, j).R != 255 || Image.GetPixel(col, j).G != 255 || Image.GetPixel(col, j).B != 255)
                        {
                            return false;  //ekkor nem teljesen fehér az oszlop
                        }
                    }

                    return true;  //fehér oszlop
                }
                catch (Exception ex)
                {
                    throw;
                }

            }

            public void SeparateWhiteRows()
            {

                int start = -1;

                while (IsRowWhite(row2) && row2 != Image.Height - 1)
                {
                    if (start == -1)
                    {
                        start = row2;
                    }

                    if (row2 < Image.Height)
                    {
                        row2++;
                    }
                }

                feherSorokPozicio.Add(new KeyValuePair<int, int>(start, row2));

                if (row2 < Image.Height - 1)
                {
                    SeparateTextRows();
                }
            }

            public void SeparateTextRows()
            {
                int start = -1;

                while (!IsRowWhite(row2) && row2 != Image.Height - 1)
                {
                    if (start == -1)
                    {
                        start = row2;
                    }

                    if (row2 < Image.Height)
                    {
                        row2++;
                    }
                }

                szovegSorokPozicio.Add(new KeyValuePair<int, int>(start, row2));

                if (row2 < Image.Height - 1)
                {
                    SeparateWhiteRows();
                }

            }

            public void ScanImage()
            {
                var previousRowType = RowType.White;
                var rowType = RowType.White;

                for (int row = 0; row < Image.Height; row++)
                {
                    bool isWhite = IsRowWhite(row);

                    Color[,] sor = new Color[1, Image.Size.Width];
                    for (int i = 0; i < Image.Width; i++)
                    {
                        sor[0, i] = Image.GetPixel(i, row);
                    }


                    KeyValuePair<int, Color[,]> kv = new KeyValuePair<int, Color[,]>(row, sor);
                    previousRowType = rowType;

                    if (isWhite)
                    {
                        feherSorok.AddLast(kv);
                        rowType = RowType.White;
                    }
                    else
                    {
                        szovegSorok.AddLast(kv);
                        rowType = RowType.Text;
                    }

                    if (previousRowType != rowType)
                    {
                        var sorok = feherSorok.ToArray();
                    }
                }

                SeparateWhiteRows();
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
                        Image.SetPixel(j, sor.Key, Color.Blue);
                    }
                }
            }

            public void PaintWhiteRows()
            {

                foreach (var sor in feherSorokPozicio)
                {
                    for (int i = sor.Key; i < sor.Value; i++)
                    {
                        for (int j = 0; j < Image.Width; j++)
                        {
                            Image.SetPixel(j, i, Color.Red);
                        }
                    }
                }
            }

            public void PaintTextRows()
            {
                foreach (var sor in szovegSorokPozicio)
                {
                    for (int i = sor.Key; i < sor.Value; i++)
                    {
                        for (int j = 0; j < Image.Width; j++)
                        {
                            Image.SetPixel(j, i, Color.Blue);
                        }
                    }
                }
            }

            public void SeparateWhiteCols()
            {
                if (processingRowIndex >= szovegSorokPozicio.Count) return;

                int textRowFrom = szovegSorokPozicio[processingRowIndex].Key;
                int textRowTo = szovegSorokPozicio[processingRowIndex].Value;

                int Column_From = -1;
                while (WhiteCol(textRowFrom, textRowTo, Column_To) && Column_To < Image.Width - 1)
                {
                    if (Column_From == -1)
                    {
                        Column_From = Column_To;
                    }

                    if (Column_To < Image.Width - 1)
                    {
                        Column_To++;
                    }
                }

                var a = new KeyValuePair<int, int>(textRowFrom, textRowTo);
                var b = new KeyValuePair<int, int>(Column_From, Column_To);
                betuSpacePoziciok.AddLast(new KeyValuePair<KeyValuePair<int, int>, KeyValuePair<int, int>>(a, b));

                if (Column_To < Image.Width - 1)
                {
                    SeparateTextCols();
                }
                else
                {
                    processingRowIndex++;
                    Column_To = 0;
                    SeparateWhiteCols();
                }
            }

            public void SeparateTextCols()
            {
                if (processingRowIndex >= szovegSorokPozicio.Count) return;

                int textRowFrom = szovegSorokPozicio[processingRowIndex].Key;
                int textRowTo = szovegSorokPozicio[processingRowIndex].Value;

                int Column_From = -1;
                while (!WhiteCol(textRowFrom, textRowTo, Column_To) && Column_To < Image.Width - 1)
                {
                    if (Column_From == -1)
                    {
                        Column_From = Column_To;
                    }

                    if (Column_To < Image.Width - 1)
                    {
                        Column_To++;
                    }
                }

                betuPoziciok.AddLast(new KeyValuePair<int, KeyValuePair<int, int>>(Column_To, new KeyValuePair<int, int>(Column_From, Column_To)));

                if (Column_To < Image.Width - 1)
                {
                    SeparateWhiteCols();
                }
                else
                {
                    processingRowIndex++;
                    Column_To = 0;
                    SeparateTextCols();
                }
            }

            public void PaintWhiteCols()
            {
                foreach (var betuSpace in betuSpacePoziciok)
                {
                    var sor = betuSpace.Key;
                    var oszlop = betuSpace.Value;

                    for (int i = sor.Key; i < sor.Value; i++)
                    {
                        for (int j = oszlop.Key; j < oszlop.Value; j++)
                        {
                            Image.SetPixel(j, i, Color.YellowGreen);
                        }
                    }
                }
            }

            public void PaintTextCols()
            {

            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Segment Segmentation = new Segment();
            Segmentation.ReadImage();
            Segmentation.ScanImage();
            //Segmentation.PaintRows();
            Segmentation.PaintWhiteRows();

            Segmentation.SeparateWhiteCols();
            Segmentation.PaintWhiteCols();



            pictureBox1.Image = Segmentation.Image;
            this.Size = new Size(Segmentation.Image.Width + 100, Segmentation.Image.Height + 100);
            pictureBox1.Size = new Size(Segmentation.Image.Width, Segmentation.Image.Height);
            pictureBox1.Image = Segmentation.Image;

        }
    }
}

