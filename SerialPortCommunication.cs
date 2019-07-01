using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;


namespace ReverseKinematic
{
    public class SerialPortCommunication : ViewModelBase
    {
        int pixelSize = 10;
        int cameraResolution = 15;
        private const int pixelCount = 225;
        private readonly byte[][] colorArray = new byte[4][];
        private readonly ColorRecognition ColorRecognition = new ColorRecognition();
        private string localSerialPortBuffer;
        private readonly SerialPort port;


        public SerialPortCommunication()
        {




            port = new SerialPort("COM5", 250000, Parity.None, 8, StopBits.One);
            port.DataReceived += port_DataReceived;
            port.Open();
            colorArray[0] = new byte[pixelCount];
            colorArray[1] = new byte[pixelCount];
            colorArray[2] = new byte[pixelCount];
            colorArray[3] = new byte[pixelCount];
        }

        public ObservableCollection<Rectangle> RectangleOryginal { get; }

        private void port_DataReceived(object sender,
            SerialDataReceivedEventArgs e)
        {
            var newData = port.ReadExisting();
            localSerialPortBuffer += newData;
            string[] StartMessageKey = { "START_COLOR_READ" };
            var CompleteMessage = getBetween(localSerialPortBuffer, "START_COLOR_READ", "END_COLOR_READ");

            if (CompleteMessage != "")
            {
                localSerialPortBuffer = "";
                var redIndex = CompleteMessage.IndexOf("RED");
                var greenIndex = CompleteMessage.IndexOf("GREEN");
                var blueIndex = CompleteMessage.IndexOf("BLUE");
                var whiteIndex = CompleteMessage.IndexOf("WHITE");

                var redString = CompleteMessage.Substring(redIndex + 5, greenIndex - redIndex - 12);
                var greenString = CompleteMessage.Substring(greenIndex + 7, blueIndex - greenIndex - 14);
                var blueString = CompleteMessage.Substring(blueIndex + 6, whiteIndex - blueIndex - 13);
                var whiteString = CompleteMessage.Substring(whiteIndex + 7, CompleteMessage.Length - whiteIndex - 14);

                var stringsArrays = new string[4][];
                stringsArrays[0] = redString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                stringsArrays[1] = greenString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                stringsArrays[2] = blueString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                stringsArrays[3] = whiteString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < 4; i++)
                    for (var j = 0; j < pixelCount; j++)
                        try
                        {
                            if (stringsArrays[i][j] == "-128") stringsArrays[i][j] = "127";
                            colorArray[i][j] = byte.Parse(stringsArrays[i][j]);
                        }
                        catch
                        {
                            MessageBox.Show("Error, check input device");
                        }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ColorRecognition.Update(colorArray);
                });
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

    //public class SerialPortCommunication : ViewModelBase
    //{
    //    //    // Create the serial port with basic settings
    //    //    private SerialPort port; 




    //    //    public SerialPortCommunication()
    //    //    {
    //    //        //port = new SerialPort("COM3",250000, Parity.None, 8, StopBits.One);

    //    //        //Console.WriteLine("Incoming Data:");

    //    //        //// Attach a method to be called when there
    //    //        //// is data waiting in the port's buffer
    //    //        //port.DataReceived += new
    //    //        //  SerialDataReceivedEventHandler(port_DataReceived);

    //    //        //// Begin communications
    //    //        //port.Open();

    //    //    }

    //    //    private void port_DataReceived(object sender,
    //    //      SerialDataReceivedEventArgs e)
    //    //    {
    //    //        // Show all the incoming data in the port's buffer
    //    //        var a = port.ReadExisting();
    //    //        Console.WriteLine(a);
    //    //        char[] key=new char[]{'\r','\n'};
    //    //       var ValueCollection= a.Split(key,StringSplitOptions.RemoveEmptyEntries);
    //    //        MassGrams = double.Parse(ValueCollection.Last());

    //    //    }

    //    private double massGrams;
    //    public double MassGrams
    //    {
    //        get { return massGrams; }
    //        set
    //        {
    //            massGrams = value;
    //            OnPropertyChanged(nameof(MassGrams));
    //        }
    //    }
    //}
}
