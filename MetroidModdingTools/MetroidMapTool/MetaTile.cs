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

    }

}
