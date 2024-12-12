using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Security.Cryptography.Xml;
using System.Windows.Navigation;

namespace MetroidMapTool
{
   
    internal class MetaTile
    {
        byte[,] TileData = new byte[2, 2];

        public MetaTile()
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    TileData[i, j] = 0xFF;
                }
            }
        }

        public void SetQuarterTile(int yIdx,  int xIdx, byte tileIdx)
        {
            TileData[yIdx, xIdx] = tileIdx; 
        }

        public byte GetQuarterTile(int yIdx, int xIDx)
        {
            return TileData[yIdx, xIDx];
        }

        public byte[] GetBitmapArray(CHRData tileTableData)
        {
            byte[] tileBuffer = new byte[64];
            for (int i = 0; i < tileBuffer.Length; i++)
            {
                tileBuffer[i] = 0;
            }

            if (tileTableData == null) 
            { 
                return tileBuffer; 
            }

            for(int x = 0; x < 2; x++)
            {
                byte tileTableIndex = TileData[0,x];
                int col = tileTableIndex % 16;
                int row = tileTableIndex / tileTableData.GetCharTableRowCount();

                byte[] tileAPixels = tileTableData.GetTile(col, row).GetBitmapArray();


                tileTableIndex = TileData[1, x];
                col = tileTableIndex % 16;
                row = tileTableIndex / tileTableData.GetCharTableRowCount();

                byte[] tileBPixels = tileTableData.GetTile(col, row).GetBitmapArray();

                for(int y = 0; y < 8; y++)
                {
                    Array.Copy(tileAPixels, y * 2, tileBuffer, (y * 4) + (x * 32), 2);
                    Array.Copy(tileBPixels, y * 2, tileBuffer, (y * 4 + 2) + (x * 32), 2);
                }

            }
            return tileBuffer;
        }
       

    }

    //macro defs are not consistent, but probably safe to just save out 1KB 

    internal class MetaTileList
    {
        MetaTile[] TileDefinitions = new MetaTile[256];

        public MetaTileList()
        {
            for (int i = 0; i < 256; i++)
            {
                TileDefinitions[i] = new MetaTile();
            }
        }

        public void ConvertTexttoDefinition(string metaText)
        {
            string file = File.ReadAllText(metaText);
            file = Regex.Replace(file, @"\r\n?|\n|\t", " ");
            string[] bytes = file.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            byte[] outputBytes = new byte[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {

                //meta tiles are 2x2, and defined as such, so we need to make sure we define them correctly
                // i / 4 = definition index
                // i % 2 = width index
                // 1 / 2 % 2 = height index

                int defIndex = i / 4;
                int xIdx = i % 2;
                int yIdx = i / 2 % 2;

                byte quarterTile = Convert.ToByte(bytes[i], 16);
                TileDefinitions[defIndex].SetQuarterTile(yIdx, xIdx, quarterTile);
                outputBytes[i] = quarterTile;
            }

            string replacementMap = metaText.Substring(0, metaText.Length - 3);
            replacementMap += "bin";

            File.WriteAllBytes(replacementMap, outputBytes);
        }

        public void OpenTileDef(string defLocation)
        {
            if (!defLocation.EndsWith("bin"))
            {
                //try opening as a text file instead, save out bin
                ConvertTexttoDefinition(defLocation);
                return;
            }

            //else, treat as a bin file 

            byte[] outputBytes = File.ReadAllBytes(defLocation);
            for (int i = 0; i < outputBytes.Length; i++)
            {

                int defIndex = i / 4;
                int xIdx = i % 2;
                int yIdx = i / 2 % 2;

                TileDefinitions[defIndex].SetQuarterTile(xIdx, yIdx, outputBytes[i]);
            }
        }

        public WriteableBitmap? CreateMetaTileBitmap(CHRData tileData)
        {


            if(tileData == null)
            {
                return null;
            }

            IList<Color> pallete = new List<Color>();
            pallete.Add(Color.FromRgb(0, 0, 0));
            pallete.Add(Color.FromRgb(64, 64, 64));
            pallete.Add(Color.FromRgb(128, 128, 128));
            pallete.Add(Color.FromRgb(255, 255, 255));


            BitmapPalette bitPal = new BitmapPalette(pallete);
            PixelFormat pf = PixelFormats.Indexed2;
            WriteableBitmap metaTileImage = new WriteableBitmap(128, 512, 32, 32, pf, bitPal);

            //we need to convert our meta tile to a bitmap buffer
            for (int i = 0; i < 256; i++)
            {
                int col = i % 8;
                int row = i / 8;

                byte[] pixels = TileDefinitions[i].GetBitmapArray(tileData);
                metaTileImage.WritePixels(new Int32Rect(col * 16, row * 16, 16, 16), pixels, 4, 0);

            }

            return metaTileImage;

        }

    }

}
