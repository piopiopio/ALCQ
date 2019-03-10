using System;
using System.Collections.Generic;
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
            MaterialList.Add("Stainless steel");
            MaterialList.Add("Carbon steel");
            MaterialList.Add("Aluminium");
            OnPropertyChanged(nameof(MaterialList));

            MaterialThicknessList.Add(0.5);
            MaterialThicknessList.Add(1);
            MaterialThicknessList.Add(2);
            MaterialThicknessList.Add(3);
            MaterialThicknessList.Add(4);
            OnPropertyChanged(nameof(MaterialThicknessList));
        }

        public double RectangleArea
        {
            get => Math.Abs((MaxXMaxY.X - MinXMinY.X) * (MaxXMaxY.Y - MinXMinY.Y));
            set { }
        }

        public double Cost { get; set; }
        public double Length { get; set; }

        public List<string> MaterialList { get; set; } = new List<string>();

        public List<double> MaterialThicknessList { get; set; } = new List<double>();

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
        }

        public List<Shape> Calculate(DxfDocument loaded)
        {
            var ObjectList = new List<Shape>();

            foreach (var item in loaded.Lines) ObjectList.Add(new Line(item));

            foreach (var item in loaded.Arcs) ObjectList.Add(new Arc(item));

            foreach (var item in loaded.Circles) ObjectList.Add(new Circle(item));

            foreach (var item in loaded.Ellipses) ObjectList.Add(new Ellipse(item));

            foreach (var item in loaded.Splines) ObjectList.Add(new Spline(item));


            foreach (var item in ObjectList)
            {
                ExtendMinMaxCoordinates(item.MaxXMaxY);
                ExtendMinMaxCoordinates(item.MinXMinY);
                Length += item.Length;
            }

            OnPropertyChanged(nameof(Length));
            OnPropertyChanged(nameof(RectangleArea));

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