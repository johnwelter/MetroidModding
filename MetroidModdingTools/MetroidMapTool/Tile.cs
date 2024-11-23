using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroidMapTool
{
    //a tile consists of an 8x8, 2 bit pixel grid
    //4 pixels can be stored per byte
    //so a tile can be made up of 2 bytes per row, for 8 rows, for a total of 16 bytes
    //this is equivalent to 8 shorts
    internal class Tile
    {
        ushort[] TilePixels = new ushort[8];

        public Tile()
        {
            Clear();
        }

        public void Clear()
        {
            for(int i = 0; i < 8; i++)
            {
                TilePixels[i] = 0;
            }
        }
        public void SetPixel(int col, int row, int val)
        {
            uint rowData = TilePixels[row];
            uint mask = 0b1100000000000000;
            uint newVal = (uint)val;
            mask = mask >> (col * 2);
            newVal = newVal << ((7 - col) * 2);
            mask = ~mask;
            rowData = rowData & mask;
            rowData = rowData | newVal;
            TilePixels[row] = (ushort)rowData;
        }
        public uint ReadPixel(int col, int row)
        {
            uint rowData = TilePixels[row];
            uint mask = 0b1100000000000000;
            mask = mask >> (col * 2);
            uint outVal = rowData & mask;
            outVal = outVal >> ((7 - col) * 2);
            return outVal;
        }

        public override String ToString()
        {
            String outString = "";
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    outString += ReadPixel(j, i);
                }
                outString += "\n";
            }
            return outString;
        }

        public byte[] GetBitmapArray()
        {
            byte[] bytes = new byte[16];
            for (int i = 0; i < 8; i++)
            {
                byte Lo = (byte)TilePixels[i];
                byte Hi = (byte)(TilePixels[i] >> 8);
                bytes[i * 2] = Hi;
                bytes[(i*2) + 1] = Lo;
            }
            return bytes;
        }

    }
}
