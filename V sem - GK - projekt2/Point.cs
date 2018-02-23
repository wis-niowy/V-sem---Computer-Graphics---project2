using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Input;

namespace V_sem___GK___projekt2
{
    public class Point : ICloneable
    {
        public double X { get; set; }
        public double Y { get; set; }
        public bool isMarked { get; set; }
        public bool IsIntersectionPoint { get; set; }
        public const double ellipseSize = 5;
        public const double epsilon = 5;
        public PointTagArgs Tag { get; set; }
        public System.Drawing.Color color { get; set; }
        public System.Drawing.Color normalColor { get; set; }
        public System.Drawing.Color markColor { get; set; }

        public Point(double x = 0, double y = 0, MouseButtonEventHandler mouseDownHandler = null)
        {
            this.X = x;
            this.Y = y;
            this.isMarked = false;
            this.IsIntersectionPoint = false;
            this.Tag = new PointTagArgs();
            normalColor = System.Drawing.Color.Blue;
            markColor = System.Drawing.Color.Red;
            color = normalColor;
        }
        public void MarkUnmark()
        {
            if (!isMarked)
            {
                Mark();
            }
            else
            {
                Unmark();
            }
        }
        public void Mark()
        {
            isMarked = true;
            color = markColor;
        }
        public void Unmark()
        {
            isMarked = false;
            color = normalColor;
        }
        public void Draw(Bitmap myBitmap, int radius = (int)ellipseSize)
        {
            int x0 = (int)X;
            int y0 = (int)Y;
            int x = radius - 1;
            int y = 0;
            int dx = 1;
            int dy = 1;
            double err = dx - (radius * 2);

            while (x >= y)
            {
                if ((new Point(x0 + x, y0 + y)).IsPointInsideRectangle(myBitmap.Width, myBitmap.Height)) myBitmap.SetPixel(x0 + x, y0 + y, color);
                if ((new Point(x0 + y, y0 + x)).IsPointInsideRectangle(myBitmap.Width, myBitmap.Height)) myBitmap.SetPixel(x0 + y, y0 + x, color);
                if ((new Point(x0 - y, y0 + x)).IsPointInsideRectangle(myBitmap.Width, myBitmap.Height)) myBitmap.SetPixel(x0 - y, y0 + x, color);
                if ((new Point(x0 - x, y0 + y)).IsPointInsideRectangle(myBitmap.Width, myBitmap.Height)) myBitmap.SetPixel(x0 - x, y0 + y, color);
                if ((new Point(x0 - x, y0 - y)).IsPointInsideRectangle(myBitmap.Width, myBitmap.Height)) myBitmap.SetPixel(x0 - x, y0 - y, color);
                if ((new Point(x0 - y, y0 - x)).IsPointInsideRectangle(myBitmap.Width, myBitmap.Height)) myBitmap.SetPixel(x0 - y, y0 - x, color);
                if ((new Point(x0 + y, y0 - x)).IsPointInsideRectangle(myBitmap.Width, myBitmap.Height)) myBitmap.SetPixel(x0 + y, y0 - x, color);
                if ((new Point(x0 + x, y0 - y)).IsPointInsideRectangle(myBitmap.Width, myBitmap.Height)) myBitmap.SetPixel(x0 + x, y0 - y, color);

                if (err <= 0)
                {
                    y++;
                    err += dy;
                    dy += 2;
                }
                if (err > 0)
                {
                    x--;
                    dx += 2;
                    err += (-radius * 2) + dx;
                }
            }
        }
        public double CountDistance(Point p)
        {
            return Math.Sqrt(Math.Pow(p.X - this.X, 2) + Math.Pow(p.Y - this.Y, 2));
        }
        public bool WasClicked(Point p)
        {
            return this.CountDistance(p) <= ellipseSize;
        }
        public bool MoveWithinRectangle(Point deltaPoint, double xMax, double yMax)
        {
            bool returnValue = true;
            double copyX = this.X;
            double copyY = this.Y;
            this.X += deltaPoint.X;
            this.Y += deltaPoint.Y;
            if (!IsCircleInsideRectangle(xMax, yMax))
            {
                returnValue = false;
                this.X = copyX;
                this.Y = copyY;
            }
            return returnValue;
        }
        public void Move(Point deltaPoint)
        {
            this.X += deltaPoint.X;
            this.Y += deltaPoint.Y;
        }
        public object Clone()
        {
            Point copy = new Point();
            copy.X = this.X;
            copy.Y = this.Y;
            copy.isMarked = this.isMarked;
            copy.Tag = this.Tag;
            return copy;
        }
        private bool IsCircleInsideRectangle(double xMax, double yMax)
        {
            double circleSurrounding = ellipseSize + epsilon;
            return (this.X + circleSurrounding < xMax && this.X - circleSurrounding > 0 && this.Y + circleSurrounding < yMax && this.Y - circleSurrounding > 0);
        }
        public bool IsPointInsideRectangle(double xMax, double yMax)
        {
            return this.X > 0 && this.X < xMax && this.Y > 0 && this.Y < yMax;
        }
        public static double CrossProduct(Point p1, Point p2) { return p1.X * p2.Y - p2.X * p1.Y; }
        public static double DotProduct(Point p1, Point p2) { return p1.X * p2.X + p1.Y * p2.Y; }
        public static Point operator +(Point p1, Point p2) { return new Point(p1.X + p2.X, p1.Y + p2.Y); }
        public static Point operator -(Point p1, Point p2) { return new Point(p1.X - p2.X, p1.Y - p2.Y); }
    }

    /////////////////////////

    public class PointTagArgs
    {
        public List<Segment> adherentSegments;
        public Figure containingFigure
        {
            get
            {
                return adherentSegments.ElementAt(0).Tag.containingFigure;
            }
            set
            {
                containingFigure = value;
            }
        }
        public PointTagArgs()
        {
            adherentSegments = new List<Segment>();
        }
    }
}
