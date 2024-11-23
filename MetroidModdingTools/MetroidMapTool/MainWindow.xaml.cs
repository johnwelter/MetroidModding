using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows; 
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;

namespace MetroidMapTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WorldMap map = new WorldMap();
        public MainWindow()
        {
            InitializeComponent();
            //do a quick test
            CHRData testCHR = new CHRData();
            //WorldMap map = new WorldMap();
            OpenFileDialog dlg = new OpenFileDialog();
            bool? result = dlg.ShowDialog();
            if(result == true)
            {
                map.ConvertTexttoMap(dlg.FileName);
                //testCHR.FillTable(dlg.FileName);
            }

            imageControl.Source = map.CreateWorldMapImage();
            
        }
        private void Map_MouseDown(object sender, MouseEventArgs e)
        {
            //do something here
            Point position = GetImageCoordsAt(e);
            int mapData = ((int)map.GetRoom((int)position.X, (int)position.Y));
            Debug.WriteLine("X: " + ((int)position.X).ToString("X2") + " Y: " + ((int)position.Y).ToString("X2") + " " + mapData.ToString("X2"));
        }

        public Point GetImageCoordsAt(MouseEventArgs e)
        {
            if (imageControl != null && imageControl.IsMouseOver)
            {
                var controlSpacePosition = e.GetPosition(imageControl);
                if (imageControl.Source != null)
                {
                    // Convert from control space to image space
                    var x = Math.Floor((controlSpacePosition.X / imageControl.ActualWidth) * 32);
                    var y = Math.Floor((controlSpacePosition.Y / imageControl.ActualHeight) * 32);

                    return new Point(x, y);
                }
            }
            return new Point(-1, -1);
        }

    }
}
