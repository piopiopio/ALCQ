using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;
using Size = System.Windows.Size;

namespace ReverseKinematic
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _mainViewModel = new MainViewModel();

        private readonly double zoomMax = 50;
        private readonly double zoomMin = 0.5;
        private readonly double zoomSpeed = 0.001;


        private double zoom = 1;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = _mainViewModel;




        }


        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {

            MainWindow1.Height = MainWindow1.Width * 9 / 16 + 24;
            //MainWindow1.Height = MainWindow1.Width * 9 / 16-24;
        }


        private void ClearScene(object sender, RoutedEventArgs e)
        {
        }


        private void LoadFile_OnClick(object sender, RoutedEventArgs e)
        {
            _mainViewModel.Scene.LoadFile(MainDrawingCanvas, MainDockPanel);
        }


        private void RobotCanvas_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //zoom += zoomSpeed * e.Delta; // Ajust zooming speed (e.Delta = Mouse spin value )
            //if (zoom < zoomMin) zoom = zoomMin;
            //if (zoom > zoomMax) zoom = zoomMax;

            //var mousePos = e.GetPosition(RobotCanvas);

            ////if (zoom > 1)
            ////{
            //RobotCanvas.RenderTransform = new ScaleTransform(zoom, zoom); // transform Canvas size from mouse position
            ////}
            ////else
            ////{
            ////    RobotCanvas.RenderTransform = new ScaleTransform(zoom, zoom); // transform Canvas size
            ////}
        }


        private void MainDrawingCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Bitmap tempBitmap = new Bitmap((int)MainDrawingCanvas.ActualWidth, (int)MainDrawingCanvas.ActualHeight);
            //ExportToPng(new Uri("C:\\Users\\Piotr\\Desktop\\n.png"), MainDrawingCanvas);
            var tempBitmap=ExportToBitmap("C:\\Users\\Piotr\\Desktop\\n1.png", MainDrawingCanvas);
          //  tempBitmap.GetPixel((int)((e.GetPosition(MainDrawingCanvas)).X), (int)(e.GetPosition(MainDrawingCanvas).Y));
            //BitmapFloodFill(tempBitmap, (int) e.GetPosition(MainDrawingCanvas).X,
            //    (int) e.GetPosition(MainDrawingCanvas).Y, 9999, 9999, Color.AliceBlue.B);
            //MainDrawingCanvas.Children.Clear();
            
            int[,] canvasArray=new int[(int)MainDrawingCanvas.Width, (int)MainDrawingCanvas.Height];

            for (int i = 0; i < tempBitmap.Width; i++)
            {
                for (int j = 0; j < tempBitmap.Height; j++)
                {
                    if (tempBitmap.GetPixel(i, j).B == Colors.AliceBlue.B)
                    {
                        canvasArray[i, j] = 0;
                    }
                    else
                    {
                        canvasArray[i, j] = 1;
                    }
                }
            }

            var Area=arrayFloodFill(canvasArray, (int)(e.GetPosition(MainDrawingCanvas).X), (int)(e.GetPosition(MainDrawingCanvas).Y),9999,9999);

            MessageBox.Show("Selected contour area: " + ((double)Area / 1000000).ToString("N2") + "m^2 Utilization: " + ((double)Area / (MainDrawingCanvas.Width * MainDrawingCanvas.Height)).ToString("N2") + "%");
        }

        public Bitmap ExportToBitmap(string path, Canvas surface)
        {

            RenderTargetBitmap bmpRen = new RenderTargetBitmap((int)surface.Width, (int)surface.Height, 96, 96, PixelFormats.Pbgra32);
            bmpRen.Render(surface);

            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmpRen));
            encoder.Save(stream);

            Bitmap bitmap = new Bitmap(stream);
            bitmap.Save(path);
            return bitmap;
        }


        //int BitmapFloodFill(Bitmap Bitmap, int startPositionX, int startPositionY, int endPositionX, int endPositionY, Byte colorToChange)
        //{


        //    if (Bitmap.GetPixel(startPositionX, startPositionY).B != colorToChange)
        //    {
        //        return 0;
        //    }

        //    var toFill = new List<int[]>();

        //    toFill.Add(new int[3] { startPositionX, startPositionY, 1 });
        //    int maxValue = 0;
        //    while (toFill.Any())
        //    {
        //        var p = toFill[0];
        //        toFill.RemoveAt(0);

        //        Bitmap.SetPixel(p[0], p[1], Color.Blue);
        //        maxValue = Math.Max(maxValue, p[2]);


        //        //if ((p[0] + 1) < Bitmap.GetLength(0))
        //        //{
        //        if ((Bitmap.GetPixel((p[0] + 1), (p[1])).B == colorToChange) &&
        //            !toFill.Any(t => (t[0] == (p[0] + 1)) && (t[1] == (p[1]))))
        //        {
        //            toFill.Add(new int[3] { (p[0] + 1), (p[1]), p[2] + 1 });
        //        }
        //        //}

        //        //if ((p[0] - 1) >= 0)
        //        //{

        //        if ((Bitmap.GetPixel((p[0] - 1), (p[1])).B == colorToChange) &&
        //            !toFill.Any(t => (t[0] == (p[0] - 1)) && (t[1] == (p[1]))))
        //        {
        //            toFill.Add(new int[3] { (p[0] - 1), (p[1]), p[2] + 1 });
        //        }
        //        //if ((Bitmap[p[0] - 1, p[1]] == colorToChange) &&
        //        //        !toFill.Any(t => (t[0] == (p[0] - 1)) && (t[1] == p[1])))
        //        //    {
        //        //        toFill.Add(new int[3] { p[0] - 1, p[1], p[2] + 1 });
        //        //    }
        //        //}

        //        //if ((p[1] + 1) < Bitmap.GetLength(1))
        //        //{
        //        if ((Bitmap.GetPixel((p[0]), (p[1] + 1)).B == colorToChange) &&
        //            !toFill.Any(t => (t[0] == (p[0])) && (t[1] == (p[1] + 1))))
        //        {
        //            toFill.Add(new int[3] { (p[0]), (p[1] + 1), p[2] + 1 });
        //        }

        //        //if ((Bitmap[p[0], p[1] + 1] == colorToChange) &&
        //        //        !toFill.Any(t => (t[0] == p[0]) && (t[1] == (p[1] + 1))))
        //        //    {
        //        //        toFill.Add(new int[3] { p[0], p[1] + 1, p[2] + 1 });
        //        //    }
        //        //}

        //        //if ((p[1] - 1) >= 0)
        //        //{
        //        if ((Bitmap.GetPixel((p[0]), (p[1] - 1)).B == colorToChange) &&
        //            !toFill.Any(t => (t[0] == (p[0])) && (t[1] == (p[1] - 1))))
        //        {
        //            toFill.Add(new int[3] { (p[0]), (p[1] - 1), p[2] + 1 });
        //        }

        //        //if ((Bitmap[p[0], p[1] - 1] == colorToChange) &&
        //        //                            !toFill.Any(t => (t[0] == p[0]) && (t[1] == (p[1] - 1))))
        //        //    {
        //        //        toFill.Add(new int[3] { p[0], p[1] - 1, p[2] + 1 });
        //        //    }
        //        //}

        //        // i++;
        //        //if (p[0] == endPositionX && p[1] == endPositionY)
        //        //{
        //        //    break;
        //        //}

        //        //Bitmap.Save("C:\\Users\\Piotr\\Desktop\\d2.png");
        //    }

        //    return maxValue;
        //}


        int arrayFloodFill(int[,] ConfigurationSpaceArray, int startPositionX, int startPositionY, int endPositionX, int endPositionY, int colorToChange = 0)
        {


            if (ConfigurationSpaceArray[startPositionX, startPositionY] != colorToChange)
            {
                return 0;
            }

            var toFill = new List<int[]>();

            toFill.Add(new int[3] { startPositionX, startPositionY, 1 });
            int maxValue = 0;
            int testArea = 0;
            while (toFill.Any())
            {
                var p = toFill[0];
                toFill.RemoveAt(0);

                ConfigurationSpaceArray[p[0], p[1]] = p[2];
                maxValue = Math.Max(maxValue, p[2]);


                //if ((p[0] + 1) < ConfigurationSpaceArray.GetLength(0))
                //{
                if ((ConfigurationSpaceArray[(p[0] + 1), (p[1])] == colorToChange) &&
                    !toFill.Any(t => (t[0] == (p[0] + 1)) && (t[1] == (p[1]))))
                {
                    toFill.Add(new int[3] { (p[0] + 1), (p[1]), p[2] + 1 });
                    testArea++;
                }
                //}

                //if ((p[0] - 1) >= 0)
                //{

                if ((ConfigurationSpaceArray[(p[0] - 1), (p[1])] == colorToChange) &&
                    !toFill.Any(t => (t[0] == (p[0] - 1)) && (t[1] == (p[1]))))
                {
                    toFill.Add(new int[3] { (p[0] - 1), (p[1]), p[2] + 1 });
                    testArea++;
                }
                //if ((ConfigurationSpaceArray[p[0] - 1, p[1]] == colorToChange) &&
                //        !toFill.Any(t => (t[0] == (p[0] - 1)) && (t[1] == p[1])))
                //    {
                //        toFill.Add(new int[3] { p[0] - 1, p[1], p[2] + 1 });
                //    }
                //}

                //if ((p[1] + 1) < ConfigurationSpaceArray.GetLength(1))
                //{
                if ((ConfigurationSpaceArray[(p[0]), (p[1] + 1)] == colorToChange) &&
                    !toFill.Any(t => (t[0] == (p[0])) && (t[1] == (p[1] + 1))))
                {
                    toFill.Add(new int[3] { (p[0]), (p[1] + 1), p[2] + 1 });
                    testArea++;
                }

                //if ((ConfigurationSpaceArray[p[0], p[1] + 1] == colorToChange) &&
                //        !toFill.Any(t => (t[0] == p[0]) && (t[1] == (p[1] + 1))))
                //    {
                //        toFill.Add(new int[3] { p[0], p[1] + 1, p[2] + 1 });
                //    }
                //}

                //if ((p[1] - 1) >= 0)
                //{
                if ((ConfigurationSpaceArray[(p[0]), (p[1] - 1)] == colorToChange) &&
                    !toFill.Any(t => (t[0] == (p[0])) && (t[1] == (p[1] - 1))))
                {
                    toFill.Add(new int[3] { (p[0]), (p[1] - 1), p[2] + 1 });
                    testArea++;
                }

                //if ((ConfigurationSpaceArray[p[0], p[1] - 1] == colorToChange) &&
                //                            !toFill.Any(t => (t[0] == p[0]) && (t[1] == (p[1] - 1))))
                //    {
                //        toFill.Add(new int[3] { p[0], p[1] - 1, p[2] + 1 });
                //    }
                //}

                // i++;
                //if (p[0] == endPositionX && p[1] == endPositionY)
                //{
                //    break;
                //}

            }

            return testArea;
        }
    }
}