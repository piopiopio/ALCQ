using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Input;

namespace ReverseKinematic
{
    public class Scene : ViewModelBase
    {
        private readonly Materials _materials;
        private readonly Surface _surface;

        private Part currentPart;

        private double mass;

        public ObservableCollection<Part> partsList = new ObservableCollection<Part>();
        public ObservableCollection<Part> filteredPartsList = new ObservableCollection<Part>();
        public SerialPortCommunication serialData;// = new SerialPortCommunication();

        public double loadCellCalibrationValue;

        public Scene()
        {
            StreamReader sr = new StreamReader("..\\..\\Settings.txt");
            var settingsToParse = sr.ReadToEnd();



            string drawingsPath = TextHelper.getBetween(settingsToParse, "DRAWINGS_PATH\r\n", "\r\nEND_END_DRAWINGS_PATH");
            ;
            var files = Directory.GetFiles("C:\\SearchToolAllDrawings", "*.pdf", SearchOption.AllDirectories);
            foreach (var item in files) partsList.Add(new Part(Path.GetFileNameWithoutExtension(item)));
            //File tempFile = new File(files[0]);




            var surfacesString = TextHelper.getBetween(settingsToParse, "SURFACES\r\n", "\r\nEND_SURFACES");
            var cameraCalibrationString = TextHelper.getBetween(settingsToParse, "COLOR_SENSOR_CALIBRATION\r\n", "\r\nEND_COLOR_SENSOR_CALIBRATION");
            //  serialData._colorAgent.RecalibrateSensor(cameraCalibrationString);
            serialData = new SerialPortCommunication(cameraCalibrationString);
            _surface = new Surface(surfacesString);




            var materialsString = TextHelper.getBetween(settingsToParse, "MATERIALS\r\n", "\r\nEND_MATERIALS");
            var inductiveSensorCalibrationString = TextHelper.getBetween(settingsToParse, "INDUCTIVE_SENSOR_CALIBRATION_VALUE\r\n", "\r\nEND_INDUCTIVE_SENSOR_CALIBRATION_VALUE");
            _materials = new Materials(materialsString, inductiveSensorCalibrationString);




            var loadCellCalibrationValueString = TextHelper.getBetween(settingsToParse, "LOAD_CELL_CALIBRATION_VALUE\r\n", "\r\nEND_LOAD_CELL_CALIBRATION_VALUE");
            loadCellCalibrationValue = double.Parse(loadCellCalibrationValueString.Remove(loadCellCalibrationValueString.Length - 1));








            string historyOrderToParse;
            string historyOrderPath = "..\\..\\OrdersHistory.txt";
            StreamReader historyOrderReader = new StreamReader(historyOrderPath);

            historyOrderToParse = historyOrderReader.ReadToEnd();
            historyOrderReader.Close();

            string ordersPath = TextHelper.getBetween(settingsToParse, "ORDERS_FOLDER_PATH\r\n", "\r\nEND_ORDERS_FOLDER_PATH");
            var newOrders = Directory.GetFiles(ordersPath, "*.txt", SearchOption.TopDirectoryOnly);

            foreach (var item in newOrders)
            {
                String fileName;
                using (StreamReader orderReader = new StreamReader(item))
                {
                    fileName = Path.GetFileName(item);
                    historyOrderToParse += "\n";
                    historyOrderToParse += orderReader.ReadToEnd();
                }

                File.Move(item, ordersPath + "\\Parsed\\" + fileName);
            }


            StreamWriter historyOrderWriter=new StreamWriter(historyOrderPath);
            historyOrderWriter.Write(historyOrderToParse);
            historyOrderWriter.Close();
            historyOrderToParse=historyOrderToParse.Replace("\r\n","\n");
            var lines = historyOrderToParse.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);


            //Add orders 

            Orders.Add("");
            
            foreach (var line in lines)
            {
                var data = line.Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                var part = partsList.Where(t => t.Name == data[3]);
                if (part.Any())
                {
                    part.First().OrderList.Add(new Order(data[0], data[1], data[4], data[2], data[5]));
                }

                if (!Orders.Any(t => t == data[1]))
                {
                    Orders.Add(data[1]);
                }

            }

            filteredPartsList = partsList;

        }

        private string selectedOrder;
        public string SelectedOrder
        {
            get { return selectedOrder; }
            set
            {
                selectedOrder = value;
                OnPropertyChanged(nameof(SelectedOrder));
                if (selectedOrder == "")
                {
                    filteredPartsList = PartsList;
                }
                else
                {
                    var a=PartsList.Where(t => t.OrderList.Any(k=>k.OrderNumber==selectedOrder)).ToList();

                    filteredPartsList=new ObservableCollection<Part>();
                    foreach (var item in a)
                    {
                        filteredPartsList.Add(item);
                    }
                }
                OnPropertyChanged(nameof(filteredPartsList));
                CurrentPart.IsSelected = false;
                CurrentPart=null;
            }
        }
        private ObservableCollection<string> orders=new ObservableCollection<string>();

        public ObservableCollection<string> Orders
        {
            get { return orders; }
            set
            {
                orders = value; 
                OnPropertyChanged(nameof(Orders));
            }
        }

        public double Mass
        {
            get => mass;
            set
            {
                mass = value;
                OnPropertyChanged(nameof(Mass));
            }
        }

        public ObservableCollection<Part> PartsList
        {
            get => partsList;
            set => partsList = value;
        }
        public ObservableCollection<Part> FilteredPartsList
        {
            get => filteredPartsList;
            set => filteredPartsList = value;
        }

        public Part CurrentPart
        {
            get => currentPart;
            set
            {
                currentPart = value;
                OnPropertyChanged(nameof(CurrentPart));
            }
        }


        public Materials Materials
        {
            get => _materials;
            set { }
        }

        private int inductiveSensorRead;
        public int InductiveSensorRead
        {
            get { return inductiveSensorRead; }
            set
            {
                inductiveSensorRead = value;
                OnPropertyChanged("InductiveSensorRead");
            }
        }
        public Surface Surface
        {
            get => _surface;
            set { }
        }

        public SerialPortCommunication SerialData
        {
            get => serialData;
            set => serialData = value;
        }

        public void Sort()
        {
            var tempList = FilteredPartsList.ToList();
            tempList.Sort((x, y) => Math.Abs(x.Mass - mass).CompareTo(Math.Abs(y.Mass - mass)));
            FilteredPartsList = new ObservableCollection<Part>(tempList);

            currentPart = filteredPartsList.First();

            foreach (var item in PartsList)
            {
                item.IsSelected = false;
            }
            filteredPartsList.First().IsSelected=true;
            OnPropertyChanged(nameof(FilteredPartsList));
            OnPropertyChanged(nameof(currentPart));
            //var tempList = partsList.ToList();
            //tempList.Sort((x, y) => Math.Abs(x.Mass - mass).CompareTo(Math.Abs(y.Mass - mass)));
            //PartsList = new ObservableCollection<Part>(tempList);
            //OnPropertyChanged(nameof(PartsList));
            EventHandler handler = RefreshDrawing;
            var e = new EventArgs();
            handler?.Invoke(this, e);
        }
        public event EventHandler RefreshDrawing;

        public void UpdateMeasure()
        {
            SerialData.Update();
        }

        public void UpdateDataFromMicroConroller()
        {
            if (SerialData.deviceConnected == false)
            {
                MessageBox.Show("Device not found, please check if it is connected and connection settings");
                SerialData.Connect();
            }
            else
            {
                Mass = SerialData._weightAgent.Mass / loadCellCalibrationValue;
                InductiveSensorRead = SerialData._materiaAgent.sensorRead;
                Surface.IdentifySurface(SerialData._colorAgent.AdjustedColorsMean);
                Materials.IdentifyMaterial(SerialData._materiaAgent.sensorRead);
            }
        }
    }
}