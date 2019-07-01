using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using netDxf;
using netDxf.Entities;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using FontStyle = netDxf.Tables.FontStyle;
using Point = System.Windows.Point;
using Polyline = System.Windows.Shapes.Polyline;
using Size = System.Windows.Size;

namespace ReverseKinematic
{
    public abstract class Shape
    {
        public Vector3 MaxXMaxY = Vector3.NaN;

        //The vectors with the smallest and highest coordinates of element. Used for scale scene and sheet area calculation. 
        public Vector3 MinXMinY = Vector3.NaN;

        //Length of entity
        public double Length { get; set; }

        public virtual void Draw(Canvas canvas, Vector3 startVector, double lineWidth = 1)
        {
        }

        public void CheckIfNewPointExtendArea(Vector3 newPoint)
        {
            if (Vector3.IsNaN(MinXMinY)) MinXMinY = newPoint;
            if (Vector3.IsNaN(MaxXMaxY)) MaxXMaxY = newPoint;

            MaxXMaxY.X = Math.Max(newPoint.X, MaxXMaxY.X);
            MaxXMaxY.Y = Math.Max(newPoint.Y, MaxXMaxY.Y);

            MinXMinY.X = Math.Min(newPoint.X, MinXMinY.X);
            MinXMinY.Y = Math.Min(newPoint.Y, MinXMinY.Y);
        }
    }


    public class LwPolyline : Shape
    {
        private readonly List<Shape> LinesOrArcsCollection = new List<Shape>();

        public LwPolyline(netDxf.Entities.LwPolyline inputLwPolyline)
        {
            InputLwPolyline = inputLwPolyline;

            //for (int i = 1; i < InputLwPolyline.Vertexes.Count; i++)
            //{
            //    LinesOrArcsCollection.Add(new Line(InputLwPolyline.Vertexes[i - 1], InputLwPolyline.Vertexes[i]));
            //}
            var temp = InputLwPolyline.Explode();
            foreach (var item in temp)
                if (item.GetType() == typeof(netDxf.Entities.Line))
                    LinesOrArcsCollection.Add(new Line((netDxf.Entities.Line) item));
                else if (item.GetType() == typeof(netDxf.Entities.Arc))
                    LinesOrArcsCollection.Add(new Arc((netDxf.Entities.Arc) item));

            foreach (var item in LinesOrArcsCollection)
            {
                CheckIfNewPointExtendArea(item.MaxXMaxY);
                CheckIfNewPointExtendArea(item.MinXMinY);
                Length += item.Length;
            }
        }

        private netDxf.Entities.LwPolyline InputLwPolyline { get; }

        public override void Draw(Canvas canvas, Vector3 startVector, double lineWidth = 1)
        {
            foreach (var item in LinesOrArcsCollection) item.Draw(canvas, startVector, lineWidth);
        }
    }

    public class Spline : Shape
    {
        private readonly int chordVariable;

        public Spline(netDxf.Entities.Spline inputSpline)
        {
            InputSpline = inputSpline;


            double polygonLength = 0;
            for (var i = 0; i < InputSpline.ControlPoints.Count() - 1; i++)
                polygonLength += (InputSpline.ControlPoints[i + 1].Position - InputSpline.ControlPoints[i].Position)
                    .Modulus();

            chordVariable = (int) (polygonLength * 20);
            var DotsList = inputSpline.PolygonalVertexes(chordVariable);
            MinXMinY = DotsList.First();
            MaxXMaxY = DotsList.First();
            foreach (var item in DotsList) CheckIfNewPointExtendArea(item);

            Length = 0;
            for (var i = 0; i < DotsList.Count - 1; i++) Length += (DotsList[i + 1] - DotsList[i]).Modulus();
        }

        private netDxf.Entities.Spline InputSpline { get; }

        public override void Draw(Canvas canvas, Vector3 startVector, double lineWidth = 1)
        {
            canvas.Children.Add(GetCanvasSpline(InputSpline, startVector, canvas.Height));
        }

        public Polyline GetCanvasSpline(netDxf.Entities.Spline InputSpline, Vector3 startVector, double canvasHeight)
        {
            var transformedControlPointsList = new PointCollection();

            foreach (var item in InputSpline.PolygonalVertexes(chordVariable))
                transformedControlPointsList.Add(new Point(item.X - startVector.X,
                    -item.Y + canvasHeight + startVector.Y));

            var g = new StreamGeometry();
            StreamGeometryContext gc;
            using (gc = g.Open())
            {
                gc.BeginFigure(
                    transformedControlPointsList.First(),
                    false,
                    false);


                gc.PolyLineTo(transformedControlPointsList, false, false);
            }

            var path = new Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Data = g
            };


            //return path;
            var temp = new Polyline();
            temp.Stroke = Brushes.Black;
            temp.StrokeThickness = 1;
            temp.Points = transformedControlPointsList;
            return temp;
        }
    }

    public class Dimension : Shape
    {
        private TextBlock textBlock = new TextBlock();
        private netDxf.Entities.Dimension inputDimension;
        public Dimension(netDxf.Entities.Dimension inputDimension)
        {
            this.inputDimension = inputDimension;
            textBlock.Text = inputDimension.Measurement.ToString("N2");
            textBlock.Text = inputDimension.Measurement.ToString("0.##");
            //textBlock.Text = "AAA";
            // MinXMinY = new Vector3(inputDimension.TextReferencePoint.X, inputDimension.TextReferencePoint.Y, 0);
            MinXMinY = new Vector3(inputDimension.TextReferencePoint.X, inputDimension.TextReferencePoint.Y, 0);
           MaxXMaxY= new Vector3(inputDimension.TextReferencePoint.X, inputDimension.TextReferencePoint.Y, 0);
            textBlock.Foreground = new SolidColorBrush(Colors.Black);
            textBlock.FontSize = 4;

        }

        public override void Draw(Canvas canvas, Vector3 startVector, double lineWidth = 1)
        {
            System.Windows.Media.FormattedText text=new FormattedText(
                inputDimension.Measurement.ToString("N0"), CultureInfo.CurrentCulture, FlowDirection.LeftToRight,new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, FontStretches.Normal), textBlock.FontSize,System.Windows.Media.Brushes.Black);
            Canvas.SetLeft(textBlock, inputDimension.TextReferencePoint.X- startVector.X-text.Width/2);

            Canvas.SetTop(textBlock, -inputDimension.TextReferencePoint.Y + startVector.Y+canvas.Height);

            canvas.Children.Add(textBlock);
        }
    }

    public class Ellipse : Shape
    {
        public Ellipse(netDxf.Entities.Ellipse inputEllipse)
        {
            MaxXMaxY = new Vector3();
            MinXMinY = new Vector3();
            InputEllipse = inputEllipse;
            var a = inputEllipse.MajorAxis / 2;
            var b = inputEllipse.MinorAxis / 2;


            MinXMinY.X = inputEllipse.Center.X - a;
            MinXMinY.Y = inputEllipse.Center.Y - b;
            MaxXMaxY.X = inputEllipse.Center.X + a;
            MaxXMaxY.Y = inputEllipse.Center.Y + b;

            var h = (a - b) / (a + b);
            Length = Math.PI * (a + b) * (1 + 3 * h * h / (10 + Math.Sqrt(4 - 3 * h * h)));
        }

        private netDxf.Entities.Ellipse InputEllipse { get; }

        public override void Draw(Canvas canvas, Vector3 startVector, double lineWidth = 1)
        {
            var ellipse = GetCanvasEllipse(InputEllipse, startVector, canvas.Height);
            canvas.Children.Add(ellipse);
        }

        public System.Windows.Shapes.Ellipse GetCanvasEllipse(netDxf.Entities.Ellipse ellipse, Vector3 startVector,
            double canvasHeight)
        {
            var tempEllipse = new System.Windows.Shapes.Ellipse();
            tempEllipse.Height = ellipse.MinorAxis;
            tempEllipse.Width = ellipse.MajorAxis;

            tempEllipse.StrokeThickness = 1;
            tempEllipse.Stroke = Brushes.Black;


            tempEllipse.SetValue(Canvas.LeftProperty, -ellipse.MajorAxis / 2 + ellipse.Center.X - startVector.X);
            tempEllipse.SetValue(Canvas.TopProperty,
                -ellipse.MinorAxis / 2 - ellipse.Center.Y + canvasHeight + startVector.Y);

            return tempEllipse;
        }
    }


    public class Circle : Shape
    {
        public Circle(netDxf.Entities.Circle inputCircle)
        {
            MaxXMaxY = new Vector3();
            MinXMinY = new Vector3();
            InputCircle = inputCircle;
            MinXMinY.X = inputCircle.Center.X - inputCircle.Radius;
            MinXMinY.Y = inputCircle.Center.Y - inputCircle.Radius;
            MaxXMaxY.X = inputCircle.Center.X + inputCircle.Radius;
            MaxXMaxY.Y = inputCircle.Center.Y + inputCircle.Radius;

            Length = 2 * Math.PI * inputCircle.Radius;
        }

        private netDxf.Entities.Circle InputCircle { get; }

        public override void Draw(Canvas canvas, Vector3 startVector, double lineWidth = 1)
        {
            var circle = GetCanvasCircle(InputCircle, startVector, canvas.Height);

            canvas.Children.Add(circle);
        }

        public System.Windows.Shapes.Ellipse GetCanvasCircle(netDxf.Entities.Circle circle, Vector3 startVector,
            double canvasHeight)
        {
            var tempEllipse = new System.Windows.Shapes.Ellipse();
            tempEllipse.Height = 2 * circle.Radius;
            tempEllipse.Width = 2 * circle.Radius;

            tempEllipse.StrokeThickness = 1;
            tempEllipse.Stroke = Brushes.Black;
            tempEllipse.SetValue(Canvas.LeftProperty, -InputCircle.Radius + InputCircle.Center.X - startVector.X);
            tempEllipse.SetValue(Canvas.TopProperty,
                -InputCircle.Radius - InputCircle.Center.Y + canvasHeight + startVector.Y);
            return tempEllipse;
        }
    }

    public class Arc : Shape
    {
        public Arc(netDxf.Entities.Arc inputArc)
        {
            InputArc = inputArc;


            var startPoint = new Point();
            startPoint.X = inputArc.Radius * Math.Cos(Math.PI * inputArc.StartAngle / 180) + inputArc.Center.X;
            startPoint.Y = inputArc.Radius * Math.Sin(Math.PI * inputArc.StartAngle / 180) + inputArc.Center.Y;

            if (angleSubtract(inputArc.EndAngle, inputArc.StartAngle) > 180)
                Length = 2 * Math.PI * inputArc.Radius * angleSubtract(inputArc.EndAngle, inputArc.StartAngle) / 360;
            else
                Length = 2 * Math.PI * inputArc.Radius * angleSubtract(inputArc.EndAngle, inputArc.StartAngle) / 360;


            MinXMinY = new Vector3(
                Math.Cos(Math.PI * inputArc.StartAngle / 180) * inputArc.Radius + inputArc.Center.X,
                Math.Sin(Math.PI * inputArc.StartAngle / 180) * inputArc.Radius + inputArc.Center.Y, 0);
            MaxXMaxY = MinXMinY;
            if (angleSubtract(inputArc.EndAngle, inputArc.StartAngle) > 180)
                for (double i = 0; i < Math.Ceiling(Length); i += 0.5)
                {
                    var tempX = Math.Cos(Math.PI * (inputArc.StartAngle +
                                                    i * angleSubtract(inputArc.EndAngle, inputArc.StartAngle) / Length
                                         ) / 180) * inputArc.Radius + inputArc.Center.X;
                    var tempY = Math.Sin(Math.PI * (inputArc.StartAngle +
                                                    i * angleSubtract(inputArc.EndAngle, inputArc.StartAngle) / Length
                                         ) / 180) * inputArc.Radius + inputArc.Center.Y;
                    CheckIfNewPointExtendArea(new Vector3(tempX, tempY, 0));
                }
            else
                for (double i = 0; i < Math.Ceiling(Length); i += 0.5)
                {
                    var tempX = Math.Cos(Math.PI * (inputArc.StartAngle +
                                                    i * angleSubtract(inputArc.EndAngle, inputArc.StartAngle) / Length
                                         ) / 180) * inputArc.Radius + inputArc.Center.X;
                    var tempY = Math.Sin(Math.PI * (inputArc.StartAngle +
                                                    i * angleSubtract(inputArc.EndAngle, inputArc.StartAngle) / Length
                                         ) / 180) * inputArc.Radius + inputArc.Center.Y;
                    CheckIfNewPointExtendArea(new Vector3(tempX, tempY, 0));
                }

            MinXMinY.Y -= 1;
        }

        public netDxf.Entities.Arc InputArc { get; set; }

        private double angleSubtract(double end, double start)
        {
            double temp;
            if (end > start)
                temp = end - start;
            else
                temp = 360 + (end - start);
            return temp;
        }


        public override void Draw(Canvas canvas, Vector3 startVector, double lineWidth = 1)
        {
            canvas.Children.Add(GetCanvasArc(InputArc, startVector, canvas.Height));
        }

        public Path GetCanvasArc(netDxf.Entities.Arc arc, Vector3 startVector, double canvasHeight)
        {
            var startPoint = new Point();
            startPoint.X = arc.Radius * Math.Cos(Math.PI * arc.StartAngle / 180) + arc.Center.X - startVector.X;
            startPoint.Y = -arc.Radius * Math.Sin(Math.PI * arc.StartAngle / 180) - arc.Center.Y + canvasHeight +
                           startVector.Y;

            var endPoint = new Point();
            endPoint.X = arc.Radius * Math.Cos(Math.PI * arc.EndAngle / 180) + arc.Center.X - startVector.X;
            endPoint.Y = -arc.Radius * Math.Sin(Math.PI * arc.EndAngle / 180) - arc.Center.Y + canvasHeight +
                         startVector.Y;

            var g = new StreamGeometry();
            StreamGeometryContext gc;
            using (gc = g.Open())
            {
                gc.BeginFigure(
                    startPoint,
                    false,
                    false);

                gc.ArcTo(
                    endPoint,
                    new Size(arc.Radius, arc.Radius),
                    180d,
                    angleSubtract(arc.EndAngle, arc.StartAngle) > 180,
                    SweepDirection.Counterclockwise,
                    true,
                    false);
            }

            var path = new Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Data = g
            };


            return path;
        }
    }

    public class Line : Shape
    {

        public Line(netDxf.Entities.Line inputLine)
        {
            InputLine = inputLine;
            Length = (inputLine.EndPoint - inputLine.StartPoint).Modulus();
            var tempMin = new Vector3();
            var tempMax = new Vector3();

            tempMin.X = Math.Min(inputLine.StartPoint.X, inputLine.EndPoint.X);
            tempMax.X = Math.Max(inputLine.StartPoint.X, inputLine.EndPoint.X);

            tempMin.Y = Math.Min(inputLine.StartPoint.Y, inputLine.EndPoint.Y);
            tempMax.Y = Math.Max(inputLine.StartPoint.Y, inputLine.EndPoint.Y);

            MinXMinY = tempMin;
            MaxXMaxY = tempMax;
        }

        public Line(double x1, double y1, double x2,double y2)
        {
            
            Length = Math.Sqrt((x2-x1)*(x2-x1)+(y2-y1)*(y2-y1));
            var tempMin = new Vector3();
            var tempMax = new Vector3();

            tempMin.X = Math.Min(x1, x2);
            tempMax.X = Math.Max(x1, x2);

            tempMin.Y = Math.Min(y1,y2);
            tempMax.Y = Math.Max(y1,y2);
            InputLine = new netDxf.Entities.Line(tempMin, tempMax);
            MinXMinY = tempMin;
            MaxXMaxY = tempMax;

        }

        public Line(LwPolylineVertex vertex1, LwPolylineVertex vertex2)
        {
            InputLine = new netDxf.Entities.Line(vertex1.Position, vertex2.Position);
            Length = (InputLine.EndPoint - InputLine.StartPoint).Modulus();
            var tempMin = new Vector3();
            var tempMax = new Vector3();

            tempMin.X = Math.Min(InputLine.StartPoint.X, InputLine.EndPoint.X);
            tempMax.X = Math.Max(InputLine.StartPoint.X, InputLine.EndPoint.X);

            tempMin.Y = Math.Min(InputLine.StartPoint.Y, InputLine.EndPoint.Y);
            tempMax.Y = Math.Max(InputLine.StartPoint.Y, InputLine.EndPoint.Y);

            MinXMinY = tempMin;
            MaxXMaxY = tempMax;
        }

        public netDxf.Entities.Line InputLine { get; set; }

        public override void Draw(Canvas canvas, Vector3 startVector, double lineWidth = 1)
        {
            canvas.Children.Add(GetCanvasLine(InputLine, startVector, canvas.Height));
        }

        private System.Windows.Shapes.Line GetCanvasLine(netDxf.Entities.Line line, Vector3 StartVector,
            double canvasHeight)
        {
            var tempLine = new System.Windows.Shapes.Line();

            tempLine.X1 = line.StartPoint.X - StartVector.X;
            tempLine.Y1 = -line.StartPoint.Y + StartVector.Y;
            tempLine.X2 = line.EndPoint.X - StartVector.X;
            tempLine.Y2 = -line.EndPoint.Y + StartVector.Y;

            tempLine.X1 = tempLine.X1;
            tempLine.Y1 = tempLine.Y1 + canvasHeight;
            tempLine.X2 = tempLine.X2;
            tempLine.Y2 = tempLine.Y2 + canvasHeight;

            tempLine.Stroke = Brushes.Black;
            tempLine.StrokeThickness = 1;
            return tempLine;
        }
    }
}