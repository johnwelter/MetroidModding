using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using System.Windows;

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
        Tile[,] CHRTable = new Tile[16,16];
        //256 tiles
        public CHRData()
        {
            for(int i = 0; i < 16; i++) 
            {
                for(int j = 0; j < 16; j++) 
                {
                    CHRTable[i,j] = new Tile();
                }
            }
        }

        public void FillTable(string chrPath)
        {
            //using a real CHR path, make our tiles
            byte[] buff = File.ReadAllBytes(chrPath);
            //i indexs each tile
            for(int i = 0; i < 256; i++)
            {
                int chrCol = i % 16;
                int chrRow = i / 16;
                //j indexes each row def
                for(int j = 0; j < 8; j++)
                {
                    int indexPartA = (i * 16) + j;
                    int indexPartB = indexPartA + 8;
                    uint tilePartA = buff[indexPartA];
                    uint tilePartB = buff[indexPartB];
                    for(int k = 7; k >= 0; k--)
                    {
                        int partA = (int)((tilePartA >> k) & 1);
                        int partB = (int)(((tilePartB >> k) & 1) * 2);
                        int value = partA + partB;
                        CHRTable[chrCol, chrRow].SetPixel(7-k, j, value);
                    }

                }
            }
        }

        public Tile GetTile(int col, int row)
        {
            return CHRTable[col, row];
        }
        
        
        
        public WriteableBitmap CreateCHRBitmap()
        {
            IList<Color> pallete = new List<Color>();
            pallete.Add(Color.FromRgb(0, 0, 0));
            pallete.Add(Color.FromRgb(64, 64, 64));
            pallete.Add(Color.FromRgb(128, 128, 128));
            pallete.Add(Color.FromRgb(255, 255, 255));


            BitmapPalette bitPal = new BitmapPalette(pallete);
            PixelFormat pf = PixelFormats.Indexed2;
            WriteableBitmap chrMapImage = new WriteableBitmap(128, 128, 32, 32, pf, bitPal);

            //we need to convert our chr table to a bitmap buffer
            List<byte> fullImage = new List<byte>();
            for(int i = 0; i < 16; i++)
            {
                for(int j = 0; j < 16; j++)
                {
                    byte[] pixels = CHRTable[j, i].GetBitmapArray();
                    chrMapImage.WritePixels(new Int32Rect(i*8, j*8, 8, 8), pixels, 2, 0);
                }
            }
            return chrMapImage;
        }




    }
}
