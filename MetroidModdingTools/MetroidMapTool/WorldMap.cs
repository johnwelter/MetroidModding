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

namespace MetroidMapTool
{
    internal class WorldMap
    {
        byte[,] MapData = new byte[32, 32];

        public WorldMap()
        {
            for(int i = 0; i < 32; i++)
            {
                for(int j = 0; j < 32; j++)
                {
                    MapData[i, j] = 0xFF;
                }
            }  
        }
        public void ConvertTexttoMap(string mapText)
        {
            string file = File.ReadAllText(mapText);
            file = Regex.Replace(file, @"\r\n?|\n", " ");
            string[] bytes = file.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            byte[] outputBytes = new byte[1024];
            for(int i = 0; i < 1024; i ++)
            {
                byte room = Convert.ToByte(bytes[i], 16);
                MapData[i % 32, i / 32] = room;
                outputBytes[i] = room;
            }

            string replacementMap = mapText.Substring(0, mapText.Length - 3);
            replacementMap += "bin";

            File.WriteAllBytes(replacementMap, outputBytes);
        }

        public void OpenMap(string mapLocation)
        {
            if (!mapLocation.EndsWith("bin"))
            {
                //try opening as a text file instead, save out bin
                ConvertTexttoMap(mapLocation);
                return;
            }

            //else, treat as a bin file 

            byte[] outputBytes = File.ReadAllBytes(mapLocation);
            for (int i = 0; i < 1024; i++)
            {
                MapData[i % 32, i / 32] = outputBytes[i];
            }

        }

        public byte GetRoom(int col, int row)
        {
            return MapData[col, row];
        }

        public WriteableBitmap CreateWorldMapImage()
        {
            IList<Color> pallete = new List<Color>();
            pallete.Add(Color.FromRgb(0, 0, 0));
            pallete.Add(Color.FromRgb(64, 64, 64));
            pallete.Add(Color.FromRgb(128, 128, 128));
            pallete.Add(Color.FromRgb(255, 255, 255));


            BitmapPalette bitPal = new BitmapPalette(pallete);
            PixelFormat pf = PixelFormats.Gray8;
            WriteableBitmap chrMapImage = new WriteableBitmap(32, 32, 32, 32, pf, bitPal);

            //we need to convert our chr table to a bitmap buffer
            List<byte> fullImage = new List<byte>();
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    byte[] colorVal = { 0 };
                    if (MapData[i, j] != 255)
                    {
                        colorVal[0] = (byte)(255 - (int)MapData[i, j]);
                    }

                    chrMapImage.WritePixels(new Int32Rect(i, j, 1, 1), colorVal, 1, 0);
                }
            }
            return chrMapImage;

        }

    }
}
