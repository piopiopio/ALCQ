using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using netDxf;

namespace ReverseKinematic
{
    public abstract class Shape
    {
        //The vectors with the smallest and highest coordinates of element. Used for scale scene and sheet area calculation. 
        public Vector3 MinXMinY;
        public Vector3 MaxXMaxY;

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
        private netDxf.Entities.LwPolyline InputLwPolyline { get; set; }
        private List<Line> LineCollection=new List<Line>();
        public LwPolyline(netDxf.Entities.LwPolyline inputLwPolyline)
        {
            InputLwPolyline = inputLwPolyline;

            for (int i = 1; i < InputLwPolyline.Vertexes.Count; i++)
            {
                LineCollection.Add(new Line(InputLwPolyline.Vertexes[i-1], InputLwPolyline.Vertexes[i]));
            }
            }

        public override void Draw(Canvas canvas, Vector3 startVector, double lineWidth = 1)
        {
            foreach (var item in LineCollection)
            {
                item.Draw(canvas, startVector, lineWidth);
            }
        }
    }
    public class Spline : Shape
    {
        private netDxf.Entities.Spline InputSpline { get; set; }
        private int chordVariable = 0;

        public Spline(netDxf.Entities.Spline inputSpline)
        {
            InputSpline = inputSpline;


            double polygonLength = 0;
            for (int i = 0; i < InputSpline.ControlPoints.Count() - 1; i++)
            {
                polygonLength += (InputSpline.ControlPoints[i + 1].Position - InputSpline.ControlPoints[i].Position)
                    .Modulus();
            }

            chordVariable = (int) (polygonLength * 10);
            var DotsList = inputSpline.PolygonalVertexes(chordVariable);
            MinXMinY = DotsList.First();
            MaxXMaxY = DotsList.First();
            foreach (var item in DotsList)
            {
                CheckIfNewPointExtendArea(item);
            }

            Length = 0;
            for (int i = 0; i < DotsList.Count - 1; i++)
            {
                Length += (DotsList[i + 1] - DotsList[i]).Modulus();
            }
        }

        public override void Draw(Canvas canvas, Vector3 startVector, double lineWidth = 1)
        {
            canvas.Children.Add(GetCanvasSpline(InputSpline, startVector, canvas.Height));
        }

        public Polyline GetCanvasSpline(netDxf.Entities.Spline InputSpline, Vector3 startVector, double canvasHeight)
        {

            PointCollection transformedControlPointsList = new PointCollection();

            foreach (var item in InputSpline.PolygonalVertexes(chordVariable))
            {
                transformedControlPointsList.Add(new Point(item.X - startVector.X,
                    -item.Y + canvasHeight + startVector.Y));
            }

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
            Polyline temp= new Polyline();
            temp.Stroke = Brushes.Black;
            temp.StrokeThickness = 1;
            temp.Points = transformedControlPointsList;
            return temp;
        }
    }

    public class Ellipse : Shape
    {
        public Ellipse(netDxf.Entities.Ellipse inputEllipse)
        {
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

        private netDxf.Entities.Ellipse InputEllipse { get; set; }
        public override void Draw(Canvas canvas, Vector3 startVector, double lineWidth = 1)
        {
            var ellipse = GetCanvasEllipse(InputEllipse, startVector, canvas.Height);
            canvas.Children.Add(ellipse);
        }

        public System.Windows.Shapes.Ellipse GetCanvasEllipse(netDxf.Entities.Ellipse ellipse, Vector3 startVector,
            double canvasHeight)
        {
            System.Windows.Shapes.Ellipse tempEllipse = new System.Windows.Shapes.Ellipse();
            tempEllipse.Height = ellipse.MinorAxis;
            tempEllipse.Width = ellipse.MajorAxis;

            tempEllipse.StrokeThickness = 1;
            tempEllipse.Stroke = Brushes.Black;
            return tempEllipse;
        }

    }

    public class Circle : Shape
    {
        public Circle(netDxf.Entities.Circle inputCircle)
        {
            InputCircle = inputCircle;
            MinXMinY.X = inputCircle.Center.X - inputCircle.Radius;
            MinXMinY.Y = inputCircle.Center.Y - inputCircle.Radius;
            MaxXMaxY.X = inputCircle.Center.X + inputCircle.Radius;
            MaxXMaxY.Y = inputCircle.Center.Y + inputCircle.Radius;

            Length = 2 * Math.PI * inputCircle.Radius;
        }

        private netDxf.Entities.Circle InputCircle { get; set; }

        public override void Draw(Canvas canvas, Vector3 startVector, double lineWidth = 1)
        {
            var circle = GetCanvasCircle(InputCircle, startVector, canvas.Height);

            canvas.Children.Add(circle);
        }

        public System.Windows.Shapes.Ellipse GetCanvasCircle(netDxf.Entities.Circle circle, Vector3 startVector, double canvasHeight)
        {
            System.Windows.Shapes.Ellipse tempEllipse = new System.Windows.Shapes.Ellipse();
            tempEllipse.Height = 2 * circle.Radius;
            tempEllipse.Width = 2 * circle.Radius;

            tempEllipse.StrokeThickness = 1;
            tempEllipse.Stroke = Brushes.Black;
            tempEllipse.SetValue(Canvas.LeftProperty, -InputCircle.Radius+InputCircle.Center.X-startVector.X);
            tempEllipse.SetValue(Canvas.TopProperty, -InputCircle.Radius-InputCircle.Center.Y+canvasHeight+startVector.Y);
            return tempEllipse;
        }
    }

    public class Arc : Shape
    {
        public Arc(netDxf.Entities.Arc inputArc)
        {
            InputArc = inputArc;
            var tempMin = new Vector3();
            var tempMax = new Vector3();

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

        public Line(netDxf.Entities.LwPolylineVertex vertex1, netDxf.Entities.LwPolylineVertex vertex2)
        {
            InputLine =new netDxf.Entities.Line(vertex1.Position, vertex2.Position);
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

        private System.Windows.Shapes.Line GetCanvasLine(netDxf.Entities.Line line, Vector3 StartVector, double canvasHeight)
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