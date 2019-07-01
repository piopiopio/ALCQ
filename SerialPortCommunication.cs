using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows;
using System.Windows.Shapes;

namespace ReverseKinematic
{
    public class SerialPortCommunication : ViewModelBase
    {
        public ColorRecognition ColorRecognition = new ColorRecognition();
        public WeightRecognition WeightRecognition=new WeightRecognition();
        public WeightRecognition MateriaRecognition=new WeightRecognition();

        private const int pixelCount = 225;
        private readonly SerialPort port;
        private int cameraResolution = 15;
        private string localSerialPortBuffer;
        private int pixelSize = 10;


        public SerialPortCommunication()
        {
            port = new SerialPort("COM5", 250000, Parity.None, 8, StopBits.One);
            port.DataReceived += port_DataReceived;
            port.Open();
        }


        private void port_DataReceived(object sender,
            SerialDataReceivedEventArgs e)
        {
            var newData = port.ReadExisting();
            localSerialPortBuffer += newData;

            var CompleteMessage = getBetween(localSerialPortBuffer, "START_MESSAGE", "END_MESSAGE");

            if (CompleteMessage != "")
            {
                localSerialPortBuffer = "";


                var ColorMessage = getBetween(CompleteMessage, "START_COLOR_READ", "END_COLOR_READ");
                Application.Current.Dispatcher.Invoke(() => { ColorRecognition.Update(ColorMessage); });

                var WeightMessage = getBetween(CompleteMessage, "START_WEIGHT_READ", "END_WEIGHT_READ");
                Application.Current.Dispatcher.Invoke(() => { WeightRecognition.Update(ColorMessage); });

                var MateriakMessage = getBetween(CompleteMessage, "START_MATERIAL_READ", "END_MATERIAL_READ");
                Application.Current.Dispatcher.Invoke(() => { MateriaRecognition.Update(ColorMessage); });
            }
        }

        private static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                if (End == -1) return "";
                return strSource.Substring(Start, End - Start);
            }

            return "";
        }
    }
}