using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WindowsInput;
using WindowsInput.Native;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;
using Point = netDxf.Entities.Point;
using Size = System.Windows.Size;

namespace ReverseKinematic
{

    public partial class MainWindow : Window
    {

        private int SerialPortSpeed = 250000;
        private string SerialPortName = "COM5";

        private string drawingsPath = "C:\\SearchToolAllDrawings";
        private readonly MainViewModel _mainViewModel = new MainViewModel();
        InputSimulator inputSimulator = new InputSimulator();
        private readonly double zoomMax = 50;
        private readonly double zoomMin = 0.5;
        private readonly double zoomSpeed = 0.001;

        private string _loadOrderPath;
        private string LoadOrderPath
        {
            get { return _loadOrderPath; }
            set
            {
                _loadOrderPath = value;

            }
        }

        private double zoom = 1;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = _mainViewModel;

            pdfWebViewer.Navigate(new Uri("about:blank"));


            //Check if default port is available, if not assign first available port.
            var temp = SerialPort.GetPortNames();
            if (!temp.Contains(SerialPortName))
            {
                SerialPortName = temp.First();
            }

            _loadOrderPath= "C:\\OrdersFolder";
            _mainViewModel.Scene.RefreshDrawing += RefreshDrawing;
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {

            MainWindow1.Height = MainWindow1.Width * 9 / 16 + 24;
        }



        void RefreshDrawing(object sender, EventArgs e)
        {
            pdfWebViewer.Navigate(new Uri(drawingsPath + "/" + _mainViewModel.Scene.CurrentPart.fullName + ".pdf"));
        }

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
            RefreshDrawing(sender, e);

        }

        private void SearchPartButton_OnClick(object sender, RoutedEventArgs e)
        {
            _mainViewModel.Scene.Sort();
        }

        private void SettingsButton_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Settings settingsWindow = new Settings(SerialPortName, SerialPortSpeed, LoadOrderPath);

            settingsWindow.Owner = this;
            settingsWindow.CammeraThreshold += ShowCameraWindowCammeraThreshold;
            settingsWindow.SaveThreshold += SettingsWindowOnSaveThreshold;

            settingsWindow.Show();


        }

        private void SettingsWindowOnSaveThreshold(object sender, EventArgs e)
        {
            var temp = (SettingEventArgs)e;
           // _mainViewModel.Scene.serialData = new SerialPortCommunication(temp.BaudRate, temp.PortName);
        }

        private void RefreshButton_OnMouseDown(object sender, MouseButtonEventArgs e)
        {

            var da = new DoubleAnimation(0, 180, new Duration(TimeSpan.FromSeconds(1)));

            RefreshButton.RenderTransform =  new RotateTransform();
            RefreshButton.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            da.RepeatBehavior = RepeatBehavior.Forever;
            RefreshButton.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, da);


            _mainViewModel.Scene.UpdateMeasure();
            _mainViewModel.Scene.SerialData.MeasureAndComputationFinishedEvent +=SerialDataOnMeasureAndComputationFinishedEvent;
        }

        private void SerialDataOnMeasureAndComputationFinishedEvent(object sender, EventArgs e)
        {
            _mainViewModel.Scene.UpdateDataFromMicroConroller();
            this.Dispatcher.Invoke(() =>
            {
                
                var da = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(1)));
                RefreshButton.RenderTransform = new RotateTransform();
                ;
                RefreshButton.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                RefreshButton.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                RefreshButton.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, da);
            });
        }

        void ShowCameraWindowCammeraThreshold(object sender, EventArgs e)
        {

            _mainViewModel.Scene.SerialData._colorAgent.cameraView.Owner = this;
            _mainViewModel.Scene.SerialData._colorAgent.cameraView.CustomShow();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _mainViewModel.Scene.SerialData._colorAgent.cameraView.Owner = this;
        }
    }
}