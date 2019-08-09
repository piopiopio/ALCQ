using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ReverseKinematic
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {

        public int selectedSerialSpeed;
        public string selectedSerialPort;

        private string _loadOrderPath="";
        public string LoadOrderPath
        {
            get { return _loadOrderPath; }
            set
            {
                _loadOrderPath = value;
              //  OnPropertyChanged(nameof(LoadOrderPath));
            }
        }


        public Settings(string portString, int boudRate, string loadOrderPath)
        {
            InitializeComponent();
            DataContext = this;
            var temp=SerialPort.GetPortNames();
            foreach (var item in temp)
            {
                SerialPortSelection.Items.Add(item);
            }

            SerialPortSelection.SelectedItem = portString;

            SerialSpeedSelection.Items.Add(250000);
            SerialSpeedSelection.Items.Add(9600);

            SerialSpeedSelection.SelectedValue = boudRate;

            selectedSerialSpeed = (int)SerialSpeedSelection.SelectedValue;
            selectedSerialPort = (string)SerialPortSelection.SelectedValue;
            LoadOrderPath = loadOrderPath;
        }

        private void SaveButton_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            SettingEventArgs setting=new SettingEventArgs();
            setting.BaudRate  = (int)SerialSpeedSelection.SelectedValue;
            setting.PortName = (string)SerialPortSelection.SelectedValue;
            setting.LoadOrderPath = LoadOrderPath;
            EventHandler handler = SaveThreshold;
            handler?.Invoke(this, setting);
        }

        private void CameraSettingsButton_OnMouseDownButton_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            EventHandler handler = CammeraThreshold;
            handler?.Invoke(this, e);
        }


        public event EventHandler CammeraThreshold;

        public event EventHandler SaveThreshold;


        private void LoadCellAettingsButton_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void InductiveSensorSettingsButton_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
    public class SettingEventArgs : EventArgs
    {
        public int BaudRate { get; set; }
        public string PortName { get; set; }
        public string LoadOrderPath { get; set; }
    }
}
