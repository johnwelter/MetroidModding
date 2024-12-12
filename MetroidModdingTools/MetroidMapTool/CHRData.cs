using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using System.Windows;
using System.Windows.Media.Media3D;

namespace MetroidMapTool
{
    //CHRData contains the tiles necessary for displaying our tiles
    //we'll make some .bin files for this so the char data is editable - hopefully ophis can compile them!
    //then we'll also be able to open them up in yychar n such
    //
    internal class CHRData
    {
        //will not contain any pixel data- that should be created by the program at run time
        //this should instead contain just the pre-processed bytes of each tile
        Tile[,]? CHRTable;
        //256 tiles
        public CHRData()
        {
            InitCHRTable(16);
        }

        public CHRData(int rows)
        {
            InitCHRTable(rows);
        }

        public CHRData(string chrPath)
        {
            //construct CHR table from path to tile data
            //each tile takes 16 bytes. we need to figure out the number of rows to create, so we need the ceiling of # of bytes / 16, then ceiling of that /16
            //alternatively, we just need to div by 32 and ceiling that
            byte[] buff = File.ReadAllBytes(chrPath);
            int length = buff.Length;
            int rowCount = (int)MathF.Ceiling((float)length / 256.0f);
            InitCHRTable(rowCount);
            FillTable(buff);

        }

        public void FillTable(byte[] buffer)
        {
            if(CHRTable == null)
            {
                return;
            }

            int tileCount = (int)MathF.Ceiling((float)buffer.Length / 16.0f);

            //i indexs each tile
            for (int i = 0; i < tileCount; i++)
            {
                int chrCol = i % 16;
                int chrRow = i / 16;
                //j indexes each row def
                for (int j = 0; j < 8; j++)
                {
                    int indexPartA = (i * 16) + j;
                    int indexPartB = indexPartA + 8;
                    uint tilePartA = 0;
                    uint tilePartB = 0;


                    if(indexPartA < buffer.Length)
                    {
                        tilePartA = buffer[indexPartA];
                    }
                    
                    if(indexPartB < buffer.Length)
                    {
                        tilePartB = buffer[indexPartB];
                    }


                    for (int k = 7; k >= 0; k--)
                    {
                        int partA = (int)((tilePartA >> k) & 1);
                        int partB = (int)(((tilePartB >> k) & 1) * 2);
                        int value = partA + partB;
                        CHRTable[chrCol, chrRow].SetPixel(7 - k, j, value);
                    }

                }
            }
        }

        public Tile? GetTile(int col, int row)
        {
            if(CHRTable != null)
            {
                return CHRTable[col, row];
            }

            return null;
        }



        public WriteableBitmap? CreateCHRBitmap()
        {

            if (CHRTable == null)
            {
                return null;
            }

            int rowCount = CHRTable.GetLength(1);

            IList<Color> pallete = new List<Color>();
            pallete.Add(Color.FromRgb(0, 0, 0));
            pallete.Add(Color.FromRgb(64, 64, 64));
            pallete.Add(Color.FromRgb(128, 128, 128));
            pallete.Add(Color.FromRgb(255, 255, 255));


            BitmapPalette bitPal = new BitmapPalette(pallete);
            PixelFormat pf = PixelFormats.Indexed2;
            WriteableBitmap chrMapImage = new WriteableBitmap(128, CHRTable.GetLength(1) * 8, 32, 32, pf, bitPal);

            //we need to convert our chr table to a bitmap buffer
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < CHRTable.GetLength(1); j++)
                {
                    byte[] pixels = CHRTable[i, j].GetBitmapArray();
                    chrMapImage.WritePixels(new Int32Rect(i * 8, j * 8, 8, 8), pixels, 2, 0);
                }
            }
            return chrMapImage;
        }

        private void InitCHRTable(int rows)
        {
            CHRTable = new Tile[16, rows];
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    CHRTable[i, j] = new Tile();
                }
            }
        }

        public int GetCharTableRowCount()
        {
            if(CHRTable == null)
            {
                return 0;
            }

            return CHRTable.GetLength(1);
        }

    }
}
