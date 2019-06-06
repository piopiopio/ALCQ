using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using netDxf;

namespace ReverseKinematic
{
    public class Scene : ViewModelBase
    {
        private readonly Material material = new Material();
        private double markup;

        public Vector3 MaxXMaxY;
        public Vector3 MinXMinY;
        public List<Shape> ObjectList = new List<Shape>();

        private double pieces;

        public Scene()
        {
            material.ThresholdReached += Recalculate;
            markup = 5;
            pieces = 1;
        }

        public double Markup
        {
            get => markup;
            set
            {
                markup = value;
                Recalculate();
            }
        }

        public double Pieces
        {
            get => pieces;
            set
            {
                pieces = value;
                Recalculate();
            }
        }

        public double TotalPrice => pieces * (MaterialCost + CuttingCost) * (1 + Markup / 100) + InitializationFee;

        public double InitializationFee => 10;

        public Material Material
        {
            get => material;
            set { }
        }

        public double MaterialCost => 1000 * 1000 * RectangleArea * Material.GetOneMM2Price;

        public double CuttingCost => Length * Material.SelectedThickness.Item2;

        public double RectangleArea
        {
            get => Math.Abs((MaxXMaxY.X - MinXMinY.X) * (MaxXMaxY.Y - MinXMinY.Y) / 1000000);
            set { }
        }


        public double Length { get; set; }

        private void Recalculate(object sender, EventArgs e)
        {
            Recalculate();
        }

        private void Recalculate()
        {
            OnPropertyChanged(nameof(Length));
            OnPropertyChanged(nameof(RectangleArea));
            OnPropertyChanged(nameof(MaterialCost));
            OnPropertyChanged(nameof(CuttingCost));
            OnPropertyChanged(nameof(Pieces));
            OnPropertyChanged(nameof(TotalPrice));
        }


        private void ExtendMinMaxCoordinates(Vector3 newPoint)
        {
            if (Vector3.IsNaN(MinXMinY)) MinXMinY = newPoint;

            if (Vector3.IsNaN(MaxXMaxY)) MaxXMaxY = newPoint;

            MaxXMaxY.X = Math.Max(newPoint.X, MaxXMaxY.X);
            MaxXMaxY.Y = Math.Max(newPoint.Y, MaxXMaxY.Y);

            MinXMinY.X = Math.Min(newPoint.X, MinXMinY.X);
            MinXMinY.Y = Math.Min(newPoint.Y, MinXMinY.Y);
        }


        public Bitmap ExportToBitmap(Canvas surface)
        {
            var bmpRen = new RenderTargetBitmap((int) surface.Width, (int) surface.Height, 96, 96,
                PixelFormats.Pbgra32);
            bmpRen.Render(surface);

            var stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmpRen));
            encoder.Save(stream);

            var bitmap = new Bitmap(stream);
            return bitmap;
        }

        public double Area(Canvas MainDrawingCanvas, MouseButtonEventArgs e)
        {
            var tempBitmap = ExportToBitmap(MainDrawingCanvas);

            var canvasArray = new int[(int) MainDrawingCanvas.Width, (int) MainDrawingCanvas.Height];

            for (var i = 0; i < tempBitmap.Width; i++)
            for (var j = 0; j < tempBitmap.Height; j++)
                if (tempBitmap.GetPixel(i, j).B == Colors.AliceBlue.B)
                    canvasArray[i, j] = 0;
                else
                    canvasArray[i, j] = 1;

            var Area = 0;
            try
            {
                Area = arrayFloodFill(canvasArray, (int) e.GetPosition(MainDrawingCanvas).X,
                    (int) e.GetPosition(MainDrawingCanvas).Y, 9999, 9999);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Click outside closed boundary. Try again.");
                Area = 0;
                return Area;
            }

            if (Area == 0)
            {
                MessageBox.Show("Click outside closed boundary. Try again.");
                return Area;
            }

            Area += (int) Length;
            Area -= ObjectList.Count();
            MessageBox.Show("Selected contour area: " + ((double) Area / 1000000).ToString("N2") + "m^2 Utilization: " +
                            (100 * (double) Area / (MainDrawingCanvas.Width * MainDrawingCanvas.Height))
                            .ToString("N0") + "%");

            return Area;
        }


        private int arrayFloodFill(int[,] ConfigurationSpaceArray, int startPositionX, int startPositionY,
            int endPositionX, int endPositionY, int colorToChange = 0)
        {
            if (ConfigurationSpaceArray[startPositionX, startPositionY] != colorToChange) return 0;

            var toFill = new List<int[]>();

            toFill.Add(new int[3] {startPositionX, startPositionY, 1});
            var maxValue = 0;
            var testArea = 0;
            while (toFill.Any())
            {
                var p = toFill[0];
                toFill.RemoveAt(0);

                ConfigurationSpaceArray[p[0], p[1]] = p[2];
                maxValue = Math.Max(maxValue, p[2]);

                if (ConfigurationSpaceArray[p[0] + 1, p[1]] == colorToChange &&
                    !toFill.Any(t => t[0] == p[0] + 1 && t[1] == p[1]))
                {
                    toFill.Add(new int[3] {p[0] + 1, p[1], p[2] + 1});
                    testArea++;
                }

                if (ConfigurationSpaceArray[p[0] - 1, p[1]] == colorToChange &&
                    !toFill.Any(t => t[0] == p[0] - 1 && t[1] == p[1]))
                {
                    toFill.Add(new int[3] {p[0] - 1, p[1], p[2] + 1});
                    testArea++;
                }

                if (ConfigurationSpaceArray[p[0], p[1] + 1] == colorToChange &&
                    !toFill.Any(t => t[0] == p[0] && t[1] == p[1] + 1))
                {
                    toFill.Add(new int[3] {p[0], p[1] + 1, p[2] + 1});
                    testArea++;
                }


                if (ConfigurationSpaceArray[p[0], p[1] - 1] == colorToChange &&
                    !toFill.Any(t => t[0] == p[0] && t[1] == p[1] - 1))
                {
                    toFill.Add(new int[3] {p[0], p[1] - 1, p[2] + 1});
                    testArea++;
                }
            }

            return testArea;
        }


        public void LoadFile(Canvas mainDrawingCanvas, DockPanel mainDockPanel)
        {
            Length = 0;

            MinXMinY = Vector3.NaN;
            MaxXMaxY = Vector3.NaN;
            ObjectList = new List<Shape>();

            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.dxf)|*.dxf";
            if (openFileDialog.ShowDialog() == false) return;

            var file = openFileDialog.FileName;


            var loaded = DxfDocument.Load(file);

            var objectList = Calculate(loaded);
            Render(objectList, mainDrawingCanvas);

            Recalculate();
        }

        public List<Shape> Calculate(DxfDocument loaded)
        {
            foreach (var item in loaded.Lines) ObjectList.Add(new Line(item));

            foreach (var item in loaded.Arcs) ObjectList.Add(new Arc(item));

            foreach (var item in loaded.Circles) ObjectList.Add(new Circle(item));

            foreach (var item in loaded.Ellipses) ObjectList.Add(new Ellipse(item));

            foreach (var item in loaded.Splines) ObjectList.Add(new Spline(item));

            foreach (var item in loaded.LwPolylines) ObjectList.Add(new LwPolyline(item));


            foreach (var item in ObjectList)
            {
                ExtendMinMaxCoordinates(item.MaxXMaxY);
                ExtendMinMaxCoordinates(item.MinXMinY);
                Length += item.Length;
            }


            return ObjectList;
        }


        public void Render(List<Shape> ObjectList, Canvas mainDrawingCanvas)
        {
            mainDrawingCanvas.Children.Clear();
            mainDrawingCanvas.Width = MaxXMaxY.X - MinXMinY.X;
            mainDrawingCanvas.Height = MaxXMaxY.Y - MinXMinY.Y;

            foreach (var item in ObjectList) item.Draw(mainDrawingCanvas, MinXMinY);
        }
    }
}