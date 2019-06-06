using System.Windows;
using System.Windows.Input;

namespace ReverseKinematic
{
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
        }


        private void MainDrawingCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _mainViewModel.Scene.Area(MainDrawingCanvas, e);
        }
    }
}