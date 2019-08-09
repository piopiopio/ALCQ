using System;
using System.Windows;

namespace ReverseKinematic
{
    public class ColorAgent : ViewModelBase
    {
        public double[][] calibrationColorArray = new double[4][];

        private const int pixelCount = 225;
        private readonly byte[][] colorArray = new byte[4][];
        public double[] AdjustedColorsMean = new double[4]; //R, G, B, Brightness 
        public CameraView cameraView;
        public double[] ColorsStandardDeviation = new double[4];
        private readonly double[] MeanColorAdjustments = {1.8, 4.2, 2.8, 1};


        public void RecalibrateSensor(object sender, EventArgs e)
        {
            //white calibration
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < pixelCount; j++)
                {
                    calibrationColorArray[i][j] = 255 / (double) colorArray[i][j];
                }
            }
        }

        public ColorAgent(string calibrationString)
        
        {
            var redIndex = calibrationString.IndexOf("RED");
            var greenIndex = calibrationString.IndexOf("GREEN");
            var blueIndex = calibrationString.IndexOf("BLUE");
            var whiteIndex = calibrationString.IndexOf("WHITE");

            var redString = calibrationString.Substring(redIndex + 5, greenIndex - redIndex - 12 + 4);
            var greenString = calibrationString.Substring(greenIndex + 7, blueIndex - greenIndex - 14 + 4);
            var blueString = calibrationString.Substring(blueIndex + 6, whiteIndex - blueIndex - 13 + 4);
            var whiteString =
                calibrationString.Substring(whiteIndex + 7, calibrationString.Length - whiteIndex - 14 + 6);

            var stringsArrays = new string[4][];
            stringsArrays[0] = redString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            stringsArrays[1] = greenString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            stringsArrays[2] = blueString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            stringsArrays[3] = whiteString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < 4; i++)
            {
                calibrationColorArray[i] = new double[pixelCount];
                for (var j = 0; j < pixelCount; j++)
                    try
                    {
                        //if (stringsArrays[i][j] == "-128") stringsArrays[i][j] = "127";
                        calibrationColorArray[i][j] = double.Parse(stringsArrays[i][j]);
                    }
                    catch
                    {
                        MessageBox.Show("Error, check input device");
                    }
            }


            colorArray[0] = new byte[pixelCount];
            colorArray[1] = new byte[pixelCount];
            colorArray[2] = new byte[pixelCount];
            colorArray[3] = new byte[pixelCount];
            cameraView = new CameraView();
            cameraView.RecalibrateThreshold += RecalibrateSensor;
            // cameraView.Show();
        }

        public void Update(byte[][] _colorArray)
        {

            var adjustedColorArray = new byte[4][];
            for (var i = 0; i < 4; i++)
            {
                adjustedColorArray[i] = new byte[pixelCount];
                for (var j = 0; j < pixelCount; j++)
                  //  adjustedColorArray[i][j] = (byte)Math.Min((MeanColorAdjustments[i] * colorArray[i][j]),255);
                    adjustedColorArray[i][j] = (byte)Math.Min((calibrationColorArray[i][j] * colorArray[i][j]),255);
            }


            for (var j = 0; j < 4; j++)
            {
                AdjustedColorsMean[j] = MeanValue(adjustedColorArray[j]);
                cameraView.Colors[j].Content = AdjustedColorsMean[j];
            }

            cameraView.updateFrame(adjustedColorArray,new byte[3] {(byte) AdjustedColorsMean[0], (byte) AdjustedColorsMean[1], (byte) AdjustedColorsMean[2]});


            for (var i = 0; i < 4; i++)
            {
                ColorsStandardDeviation[i] = 0;
                for (var j = 0; j < pixelCount; j++)
                    ColorsStandardDeviation[i] = ColorsStandardDeviation[i] +
                                                 adjustedColorArray[i][j] * (double) adjustedColorArray[i][j] /
                                                 pixelCount;

                ColorsStandardDeviation[i] = Math.Sqrt(ColorsStandardDeviation[i] - AdjustedColorsMean[i]);
                cameraView.Deviations[i].Content = ColorsStandardDeviation[i];
            }
        }

        private double[][] NormalizeColorsArray(byte[][] input)
        {
            var tempAdjustedColorsArrays = new double[3][];
            for (var i = 0; i < 3; i++)
            {
                tempAdjustedColorsArrays[i] = new double[pixelCount];
                for (var j = 0; j < pixelCount; j++)
                    tempAdjustedColorsArrays[i][j] = input[i][j] / (double) input[3][j];
            }

            return tempAdjustedColorsArrays;
        }


        private double MeanValue(double[] parametersList)
        {
            double mean = 0;
            foreach (var param in parametersList) mean += param;

            mean = mean / parametersList.Length;
            return mean;
        }

        private byte MeanValue(byte[] parametersList)
        {
            var mean = 0;
            foreach (var param in parametersList) mean += param;

            mean = mean / parametersList.Length;
            return (byte) mean;
        }

        public void Update(string CompleteMessage)
        {
            var redIndex = CompleteMessage.IndexOf("RED");
            var greenIndex = CompleteMessage.IndexOf("GREEN");
            var blueIndex = CompleteMessage.IndexOf("BLUE");
            var whiteIndex = CompleteMessage.IndexOf("WHITE");

            var redString = CompleteMessage.Substring(redIndex + 5, greenIndex - redIndex - 12+4);
            var greenString = CompleteMessage.Substring(greenIndex + 7, blueIndex - greenIndex - 14+4);
            var blueString = CompleteMessage.Substring(blueIndex + 6, whiteIndex - blueIndex - 13+4);
            var whiteString = CompleteMessage.Substring(whiteIndex + 7, CompleteMessage.Length - whiteIndex - 14+4);

            var stringsArrays = new string[4][];
            stringsArrays[0] = redString.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            stringsArrays[1] = greenString.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            stringsArrays[2] = blueString.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            stringsArrays[3] = whiteString.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

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

            Update(colorArray);
        }
    }
}