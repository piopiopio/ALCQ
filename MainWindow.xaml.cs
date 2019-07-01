using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WindowsInput;
using WindowsInput.Native;
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
        private string drawingsPath = "C:\\SearchToolAllDrawings";
        private readonly MainViewModel _mainViewModel = new MainViewModel();
        InputSimulator inputSimulator=new InputSimulator();
        private readonly double zoomMax = 50;
        private readonly double zoomMin = 0.5;
        private readonly double zoomSpeed = 0.001;


        private double zoom = 1;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = _mainViewModel;

            pdfWebViewer.Navigate(new Uri("about:blank"));
           // pdfWebViewer.Navigate(new Uri("C:\\Users\\Piotr\\Desktop\\Part1.pdf"));



          //  BackgroundFocus(MainDockPanel);


        }

        //public void BackgroundFocus(UIElement el)
        //{
            
        //    Action a = () => el.Focus();
        //    Action b = () => test();
 
            
        //    el.Dispatcher.BeginInvoke(DispatcherPriority.Background, a);
        //    el.Dispatcher.BeginInvoke(DispatcherPriority.Background, b);

        //    //DispatcherTimer dispatcherTimer = new DispatcherTimer();
        //    //dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
        //    //dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
        //    //dispatcherTimer.Start();

        //}

        //private void dispatcherTimer_Tick(object sender, EventArgs e)
        //{
        // test();
        //}
        //public void test()
        //{
        //    InputSimulator InputSimulator = new InputSimulator();
        //    InputSimulator.Mouse.MoveMouseTo(1500,30000);
        //    InputSimulator.Mouse.LeftButtonClick();
        //    InputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_L);
        //}
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
           
            //  tempBitmap.GetPixel((int)((e.GetPosition(MainDrawingCanvas)).X), (int)(e.GetPosition(MainDrawingCanvas).Y));
            //BitmapFloodFill(tempBitmap, (int) e.GetPosition(MainDrawingCanvas).X,
            //    (int) e.GetPosition(MainDrawingCanvas).Y, 9999, 9999, Color.AliceBlue.B);
            //MainDrawingCanvas.Children.Clear();
            _mainViewModel.Scene.Area(MainDrawingCanvas, e);
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


        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            while ((dep != null) && !(dep is ListViewItem))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
                return;

            _mainViewModel.Scene.CurrentPart = (Part)MyListView.ItemContainerGenerator.ItemFromContainer(dep);

            //MessageBox.Show(item.Name);
            pdfWebViewer.Navigate(new Uri(drawingsPath +"/" + _mainViewModel.Scene.CurrentPart.fullName+".pdf"));

        }

        private void SearchPartButton_OnClick(object sender, RoutedEventArgs e)
        {
           _mainViewModel.Scene.Sort();
        }
    }
}