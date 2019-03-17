using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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
            var line = new System.Windows.Shapes.Line();
            line.Stroke = Brushes.Black;
            line.X1 = 0;
            line.Y1 = 0;
            line.X2 = 100;
            line.Y2 = 100;
            line.StrokeThickness = 2;
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


    }
}