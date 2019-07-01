using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseKinematic
{
    class ColorRecognition : ViewModelBase
    {
        double[] MeanColorAdjustments = new double[] { 1.8, 4.2, 2.8, 1 };
        private CameraView cameraView;
        private byte[][] colorArray { get; set; }
        private const int pixelCount = 225;
        // public double[][] AdjustedColorsArrays = new double[3][];
        public double[] AdjustedColorsMean = new double[4]; //R, G, B, Brightness 
        public double[] ColorsStandardDeviation = new double[4];
        public ColorRecognition()
        {

            cameraView = new CameraView();
            cameraView.Show();

        }

        public void Update(byte[][] _colorArray)
        {
            colorArray = _colorArray;
            //AdjustedColorsArrays = NormalizeColorsArray(inputArray);
            //for (int i = 0; i < 3; i++)
            //{
            //    AdjustedColorsMean[i] = MeanValue(AdjustedColorsArrays[i]);

            //}



            byte[][] adjustedColorArray = new byte[4][];
            for (int i = 0; i < 4; i++)
            {
                adjustedColorArray[i] = new byte[pixelCount];
                for (int j = 0; j < pixelCount; j++)
                {
                    adjustedColorArray[i][j] = (byte)(MeanColorAdjustments[i] * colorArray[i][j]);
                }
            }


            for (int j = 0; j < 4; j++)
            {
                AdjustedColorsMean[j] = MeanValue(adjustedColorArray[j]);
                cameraView.Colors[j].Content = AdjustedColorsMean[j];
            }

            cameraView.updateFrame(adjustedColorArray, new byte[3] { (byte)AdjustedColorsMean[0], (byte)AdjustedColorsMean[1], (byte)AdjustedColorsMean[2] });


            for (int i = 0; i < 4; i++)
            {
                ColorsStandardDeviation[i] = 0;
                for (int j = 0; j < pixelCount; j++)
                {
                    ColorsStandardDeviation[i] = ColorsStandardDeviation[i]+((double)(adjustedColorArray[i][j])*(double)(adjustedColorArray[i][j]))/(double)(pixelCount);
                }

                ColorsStandardDeviation[i] = Math.Sqrt(ColorsStandardDeviation[i] - AdjustedColorsMean[i]);
                cameraView.Deviations[i].Content = ColorsStandardDeviation[i];
            }

        }

        private double[][] NormalizeColorsArray(byte[][] input)
        {
            double[][] tempAdjustedColorsArrays = new double[3][];
            for (int i = 0; i < 3; i++)
            {
                tempAdjustedColorsArrays[i] = new double[pixelCount];
                for (int j = 0; j < pixelCount; j++)
                {
                    tempAdjustedColorsArrays[i][j] = input[i][j] / (double)input[3][j];
                }
            }

            return tempAdjustedColorsArrays;
        }


        private double MeanValue(double[] parametersList)
        {
            double mean = 0;
            foreach (var param in parametersList)
            {
                mean += param;
            }

            mean = mean / parametersList.Length;
            return mean;
        }

        private byte MeanValue(byte[] parametersList)
        {
            int mean = 0;
            foreach (var param in parametersList)
            {
                mean += param;
            }

            mean = mean / parametersList.Length;
            return (byte)mean;
        }

    }
}
