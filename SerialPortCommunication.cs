using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows;
using System.Windows.Shapes;

namespace ReverseKinematic
{
    public class SerialPortCommunication : ViewModelBase
    {
        public ColorAgent _colorAgent;
        public WeightAgent _weightAgent = new WeightAgent();
        public MaterialAgent _materiaAgent = new MaterialAgent();

        private const int pixelCount = 225;
        private SerialPort port;
        private int cameraResolution = 15;
        private string localSerialPortBuffer;
        private int pixelSize = 10;
        public bool deviceConnected = false;

        public SerialPortCommunication(string colorAgentInitialString, int boundRate = 250000, string portName = "COM5")
        {

            Connect(boundRate, portName);
            _colorAgent = new ColorAgent(colorAgentInitialString);
        }

        ~SerialPortCommunication()
        {
            port.Close();
        }

        public void Connect(int boundRate = 250000, string portName = "COM5")
        {
            try
            {
                port = new SerialPort(portName, boundRate, Parity.None, 8, StopBits.One);
                port.DataReceived += port_DataReceived;
                port.Open();
                deviceConnected = true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Device not found, please check if it is connected and connection settings");
                deviceConnected = false;
            }
        }

        public void Reconnct(int boundRate = 250000, string portName = "COM5")
        {

            Connect(boundRate, portName);
        }

        private bool GetMeasure = true;
        private bool ContinuousMeasure = true;

        private void port_DataReceived(object sender,
            SerialDataReceivedEventArgs e)
        {
            var newData = port.ReadExisting();
            localSerialPortBuffer += newData;

            var CompleteMessage = TextHelper.getBetween(localSerialPortBuffer, "START_MESSAGE", "END_MESSAGE");

            if (GetMeasure)
            {
                if (CompleteMessage != "")
                {
                    localSerialPortBuffer = "";


                    var ColorMessage = TextHelper.getBetween(CompleteMessage, "START_COLOR_READ", "END_COLOR_READ");
                    Application.Current.Dispatcher.Invoke(() => { _colorAgent.Update(ColorMessage); });

                    var WeightMessage = TextHelper.getBetween(CompleteMessage, "START_WEIGHT_READ", "END_WEIGHT_READ");
                    Application.Current.Dispatcher.Invoke(() => { _weightAgent.Update(WeightMessage); });

                    var MaterialMessage = TextHelper.getBetween(CompleteMessage, "START_MATERIAL_READ", "END_MATERIAL_READ");
                    Application.Current.Dispatcher.Invoke(() => { _materiaAgent.Update(MaterialMessage); });

                    //GetMeasure = GetMeasure && ContinuousMeasure;

                    EventHandler handler = MeasureAndComputationFinishedEvent;
                    handler?.Invoke(this, e);
                    GetMeasure = ContinuousMeasure;
                }
            }
            //else
            //{
            //    localSerialPortBuffer = "";
            //}
        }

        public event EventHandler MeasureAndComputationFinishedEvent;


        public void Update()
        {
            GetMeasure = true;
           localSerialPortBuffer = "";
        }
    }
}