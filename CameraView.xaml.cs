using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    /// Interaction logic for CameraView.xaml
    /// </summary>
    public partial class CameraView : Window
    {
        const int pixelSize = 10;
        const int cameraResolution = 15;
        ObservableCollection<Rectangle> PixelsCollection = new ObservableCollection<Rectangle>();
        ObservableCollection<Rectangle> RedPixelsCollection = new ObservableCollection<Rectangle>();
        ObservableCollection<Rectangle> GreenPixelsCollection = new ObservableCollection<Rectangle>();
        ObservableCollection<Rectangle> BluePixelsCollection = new ObservableCollection<Rectangle>();
        ObservableCollection<Rectangle> WhitePixelsCollection = new ObservableCollection<Rectangle>();

        public Label[] Deviations = new Label[4];
        public Label[] Colors = new Label[4];
        public Rectangle MeanRectangle
        {
            get;
            set;
        }

        public CameraView()
        {
            InitializeComponent();

            Deviations[0] = RedStandardDeviation;
            Deviations[1] = GreenStandardDeviation;
            Deviations[2] = BlueStandardDeviation;
            Deviations[3] = WhiteStandardDeviation;

            Colors[0] = RedColorValue;
            Colors[1] = GreenColorValue;
            Colors[2] = BlueColorValue;
            Colors[3] = WhiteColorValue;

            for (int i = 0; i < cameraResolution; i++)
            {
                for (int j = 0; j < cameraResolution; j++)
                {
                    Rectangle tempRectangle = new Rectangle();
                    Canvas.SetLeft(tempRectangle, pixelSize * i);
                    Canvas.SetTop(tempRectangle, pixelSize * j);
                    tempRectangle.Width = pixelSize;
                    tempRectangle.Height = pixelSize;
                    tempRectangle.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    PixelsCollection.Add(tempRectangle);
                    MouseCameraCanvas.Children.Add(tempRectangle);
                }
            }

            for (int i = 0; i < cameraResolution; i++)
            {
                for (int j = 0; j < cameraResolution; j++)
                {
                    Rectangle tempRectangle = new Rectangle();
                    Canvas.SetLeft(tempRectangle, pixelSize * i);
                    Canvas.SetTop(tempRectangle, pixelSize * j+pixelSize*cameraResolution);
                    tempRectangle.Width = pixelSize;
                    tempRectangle.Height = pixelSize;
                    tempRectangle.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    RedPixelsCollection.Add(tempRectangle);
                    MouseCameraCanvas.Children.Add(tempRectangle);
                }
            }

            for (int i = 0; i < cameraResolution; i++)
            {
                for (int j = 0; j < cameraResolution; j++)
                {
                    Rectangle tempRectangle = new Rectangle();
                    Canvas.SetLeft(tempRectangle, pixelSize * i + pixelSize * cameraResolution);
                    Canvas.SetTop(tempRectangle, pixelSize * j + pixelSize * cameraResolution);
                    tempRectangle.Width = pixelSize;
                    tempRectangle.Height = pixelSize;
                    tempRectangle.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    GreenPixelsCollection.Add(tempRectangle);
                    MouseCameraCanvas.Children.Add(tempRectangle);
                }
            }

            for (int i = 0; i < cameraResolution; i++)
            {
                for (int j = 0; j < cameraResolution; j++)
                {
                    Rectangle tempRectangle = new Rectangle();
                    Canvas.SetLeft(tempRectangle, pixelSize * i);
                    Canvas.SetTop(tempRectangle, pixelSize * j + 2*pixelSize * cameraResolution);
                    tempRectangle.Width = pixelSize;
                    tempRectangle.Height = pixelSize;
                    tempRectangle.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    BluePixelsCollection.Add(tempRectangle);
                    MouseCameraCanvas.Children.Add(tempRectangle);
                }
            }

            for (int i = 0; i < cameraResolution; i++)
            {
                for (int j = 0; j < cameraResolution; j++)
                {
                    Rectangle tempRectangle = new Rectangle();
                    Canvas.SetLeft(tempRectangle, pixelSize * i+ pixelSize * cameraResolution);
                    Canvas.SetTop(tempRectangle, pixelSize * j + 2*pixelSize * cameraResolution);
                    tempRectangle.Width = pixelSize;
                    tempRectangle.Height = pixelSize;
                    tempRectangle.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    WhitePixelsCollection.Add(tempRectangle);
                    MouseCameraCanvas.Children.Add(tempRectangle);
                }
            }



            MeanRectangle =new Rectangle();
            Canvas.SetLeft(MeanRectangle, pixelSize * cameraResolution);
            Canvas.SetTop(MeanRectangle, 0);
            MeanRectangle.Width = pixelSize * cameraResolution;
            MeanRectangle.Height = pixelSize * cameraResolution;
            MeanRectangle.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            MouseCameraCanvas.Children.Add(MeanRectangle);
        }

        public void updateFrame(byte[][] colorArray, byte[] MeanColor)
        {
            for (int i = 0; i < cameraResolution* cameraResolution; i++)
            {
                PixelsCollection[i].Fill=new SolidColorBrush(Color.FromRgb(colorArray[0][i], colorArray[1][i], colorArray[2][i]));
                RedPixelsCollection[i].Fill=new SolidColorBrush(Color.FromRgb(colorArray[0][i], 0, 0));
                GreenPixelsCollection[i].Fill=new SolidColorBrush(Color.FromRgb(0,colorArray[1][i], 0));
                BluePixelsCollection[i].Fill=new SolidColorBrush(Color.FromRgb(0,0,colorArray[2][i]));
                WhitePixelsCollection[i].Fill=new SolidColorBrush(Color.FromRgb(colorArray[3][i], colorArray[3][i], colorArray[3][i]));
            }
            MeanRectangle.Fill= new SolidColorBrush(Color.FromRgb(MeanColor[0], MeanColor[1], MeanColor[2]));
        }
    


    }
}
