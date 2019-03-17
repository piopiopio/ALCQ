using System;
using System.Collections.Generic;
using System.Security.RightsManagement;
using System.Windows.Controls;
using Microsoft.Win32;
using netDxf;

namespace ReverseKinematic
{
    public class Scene : ViewModelBase
    {

        public Vector3 MaxXMaxY;
        public Vector3 MinXMinY;

        public Scene()
        {
            material.ThresholdReached += Recalculate;
            markup = 5;
            pieces = 1;
        }

        void Recalculate(object sender, EventArgs e)
        {
            Recalculate();
        }

        void Recalculate()
        {
            OnPropertyChanged(nameof(Length));
            OnPropertyChanged(nameof(RectangleArea));
            OnPropertyChanged(nameof(MaterialCost));
            OnPropertyChanged(nameof(CuttingCost));
            OnPropertyChanged(nameof(Pieces));
            OnPropertyChanged(nameof(TotalPrice));
        }

        private double markup;
        public double Markup
        {
            get { return markup; }
            set
            {
                markup = value;
                Recalculate();
            }
        }

        private double pieces;
        public double Pieces
        {
            get { return pieces; }
            set
            {
                pieces = value;
                Recalculate();
            }
        }
        public double TotalPrice
        {
            get
            {
                return pieces*(MaterialCost + CuttingCost) * (1 + Markup / 100) + InitializationFee;
            }
        }
        public double InitializationFee
        {
            get { return 10; }
        }
        public Material Material
        {
            get
            {
                return material;
            }
            set
            {

            }
        }

        public double MaterialCost
        {
            get
            {             
                return RectangleArea * Material.GetOneMM2Price;

            }
        }

        public double CuttingCost
        {
            get
            {
                return Length * Material.SelectedThickness.Item2;
            }
        }
        private Material material = new Material();
        public double RectangleArea
        {
            get => Math.Abs((MaxXMaxY.X - MinXMinY.X) * (MaxXMaxY.Y - MinXMinY.Y));
            set { }
        }


        public double Length { get; set; }


        private void ExtendMinMaxCoordinates(Vector3 newPoint)
        {
            if (Vector3.IsNaN(MinXMinY)) MinXMinY = newPoint;
            if (Vector3.IsNaN(MaxXMaxY)) MaxXMaxY = newPoint;

            MaxXMaxY.X = Math.Max(newPoint.X, MaxXMaxY.X);
            MaxXMaxY.Y = Math.Max(newPoint.Y, MaxXMaxY.Y);

            MinXMinY.X = Math.Min(newPoint.X, MinXMinY.X);
            MinXMinY.Y = Math.Min(newPoint.Y, MinXMinY.Y);
        }


        public void LoadFile(Canvas mainDrawingCanvas, DockPanel mainDockPanel)
        {
            Length = 0;

            MinXMinY = Vector3.NaN;
            MaxXMaxY = Vector3.NaN;


            mainDrawingCanvas.Children.Clear();
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.dxf)|*.dxf";
            if (openFileDialog.ShowDialog() == false) return;


            // your dxf file name
            // string file = "sample1.dxf";
            var file = openFileDialog.FileName;

            //// by default it will create an AutoCad2000 DXF version
            //DxfDocument dxf = new DxfDocument();
            //// an entity
            //netDxf.Entities.Line entity = new netDxf.Entities.Line(new Vector2(5, 5), new Vector2(10, 5));
            //// add your entities here
            //dxf.AddEntity(entity);
            //// save to file
            //dxf.Save(file);

            //bool isBinary;
            //// this check is optional but recommended before loading a DXF file
            //DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(file, out isBinary);
            //// netDxf is only compatible with AutoCad2000 and higher DXF version
            //if (dxfVersion < DxfVersion.AutoCad2000) return;
            //// load file

            var loaded = DxfDocument.Load(file);

            var objectList = Calculate(loaded);
            Render(objectList, mainDrawingCanvas);

            Recalculate();
        }

        public List<Shape> Calculate(DxfDocument loaded)
        {
            var ObjectList = new List<Shape>();

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
            mainDrawingCanvas.Width = MaxXMaxY.X - MinXMinY.X;
            mainDrawingCanvas.Height = MaxXMaxY.Y - MinXMinY.Y;

            foreach (var item in ObjectList) item.Draw(mainDrawingCanvas, MinXMinY);
        }
    }
}