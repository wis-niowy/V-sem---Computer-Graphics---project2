using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace V_sem___GK___projekt2
{
    public class Segment : ICloneable
    {
        public Point pointA { get; set; }
        public Point pointB { get; set; }
        public SegmentTagArgs Tag { get; set; }
        public bool isMarked { get; set; }
        public bool isHorizontal
        {
            get
            {
                return pointA.Y == pointB.Y;
            }
        }
        public bool isVertical
        {
            get
            {
                return pointA.X == pointB.X;
            }
        }
        public int thickness { get; set; }
        public System.Drawing.Color color { get; set; }
        public System.Drawing.Color normalColor { get; set; }
        public System.Drawing.Color markColor { get; set; }
        private double length;
        public double Length
        {
            get
            {
                return pointA.CountDistance(pointB);
            }
            set
            {
                // TODO - setting Segment's length (moving pointB)
                double c = pointA.CountDistance(pointB);
                double z = value - c;
                double a = pointB.X - pointA.X;
                double b = pointB.Y - pointA.Y;
                double x = a * (c + z) / c - a;
                double y = b * (c + z) / c - b;
                pointA.X -= x / 2;
                pointA.Y -= y / 2;
                pointB.X += x / 2;
                pointB.Y += y / 2;
                length = value;
            }
        }

        public Segment(Point a, Point b)
        {
            this.Tag = new SegmentTagArgs();
            pointA = a;
            pointB = b;
            isMarked = false;
            normalColor = System.Drawing.Color.Black;
            markColor = System.Drawing.Color.Orange;
            color = normalColor;
            thickness = 1;
        }

        public void Draw(Bitmap myBitmap)
        {
            drawSegment(myBitmap);
        }

        public void drawSegment(Bitmap myBitmap)
        {
            int x = (int)pointA.X;
            int y = (int)pointA.Y;
            int x2 = (int)pointB.X;
            int y2 = (int)pointB.Y;
            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                AddPixel(x, y, myBitmap, color); // defined below
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
            pointA.Draw(myBitmap);
            pointB.Draw(myBitmap);
        }
        private void AddPixel(double x0, double y0, Bitmap myBitmap, System.Drawing.Color color)
        {
            if (thickness == 1)
            {
                if ((new Point(x0, y0)).IsPointInsideRectangle(myBitmap.Width, myBitmap.Height))
                    myBitmap.SetPixel((int)x0, (int)y0, color);
            }
            else
            {
                double myRadius = 1;
                for (int i = 0; i < thickness; ++i)
                {
                    double radius = myRadius;
                    double x = radius - 1;
                    double y = 0;
                    double dx = 1;
                    double dy = 1;
                    double err = dx - (radius * 2);

                    while (x >= y)
                    {
                        if ((new Point(x0 + x, y0 + y)).IsPointInsideRectangle(myBitmap.Width, myBitmap.Height)) myBitmap.SetPixel((int)(x0 + x), (int)(y0 + y), color);
                        if ((new Point(x0 + y, y0 + x)).IsPointInsideRectangle(myBitmap.Width, myBitmap.Height)) myBitmap.SetPixel((int)(x0 + y), (int)(y0 + x), color);
                        if ((new Point(x0 - y, y0 + x)).IsPointInsideRectangle(myBitmap.Width, myBitmap.Height)) myBitmap.SetPixel((int)(x0 - y), (int)(y0 + x), color);
                        if ((new Point(x0 - x, y0 + y)).IsPointInsideRectangle(myBitmap.Width, myBitmap.Height)) myBitmap.SetPixel((int)(x0 - x), (int)(y0 + y), color);
                        if ((new Point(x0 - x, y0 - y)).IsPointInsideRectangle(myBitmap.Width, myBitmap.Height)) myBitmap.SetPixel((int)(x0 - x), (int)(y0 - y), color);
                        if ((new Point(x0 - y, y0 - x)).IsPointInsideRectangle(myBitmap.Width, myBitmap.Height)) myBitmap.SetPixel((int)(x0 - y), (int)(y0 - x), color);
                        if ((new Point(x0 + y, y0 - x)).IsPointInsideRectangle(myBitmap.Width, myBitmap.Height)) myBitmap.SetPixel((int)(x0 + y), (int)(y0 - x), color);
                        if ((new Point(x0 + x, y0 - y)).IsPointInsideRectangle(myBitmap.Width, myBitmap.Height)) myBitmap.SetPixel((int)(x0 + x), (int)(y0 - y), color);

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
                    myRadius++;
                }
            }
        }
        public bool SetPointWithinRectangle(string choice, Point newCoords, double xMax, double yMax)
        {
            bool returnValue = true;
            Point copyPoint = null;
            switch (choice)
            {
                case "A":
                    copyPoint = pointA.Clone() as Point;
                    pointA = newCoords;
                    if (!IsSegmentInsideRectangle(xMax, yMax))
                    {
                        returnValue = false;
                        pointA = copyPoint;
                    }
                    break;
                case "B":
                    copyPoint = pointB.Clone() as Point;
                    pointB = newCoords;
                    if (!IsSegmentInsideRectangle(xMax, yMax))
                    {
                        returnValue = false;
                        pointB = copyPoint;
                    }
                    break;
            }

            return returnValue;
        }
        public void SetPoint(string choice, Point newCoords)
        {
            switch (choice)
            {
                case "A":
                    pointA = newCoords;
                    break;
                case "B":
                    pointB = newCoords;
                    break;
            }
        }
        public bool IsSegmentInsideRectangle(double xMax, double yMax)
        {
            double pointSurrounding = 0;
            if (thickness < Point.ellipseSize)
                pointSurrounding = Point.ellipseSize + Point.epsilon;
            else
                pointSurrounding = thickness + Point.epsilon;
            return pointA.X - pointSurrounding > 0 && pointA.X + pointSurrounding < xMax && pointA.Y - pointSurrounding > 0 && pointA.Y + pointSurrounding < yMax &&
                    pointB.X - pointSurrounding > 0 && pointB.X + pointSurrounding < xMax && pointB.Y - pointSurrounding > 0 && pointB.Y + pointSurrounding < yMax;
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
        private double CountDistance(Point p)
        {
            double x1 = pointA.X, y1 = pointA.Y, x2 = pointB.X, y2 = pointB.Y;
            double x3 = p.X, y3 = p.Y;
            double px = x2 - x1;
            double py = y2 - y1;
            double temp = (px * px) + (py * py);
            double u = ((x3 - x1) * px + (y3 - y1) * py) / (temp);
            if (u > 1)
            {
                u = 1;
            }
            else if (u < 0)
            {
                u = 0;
            }
            double x = x1 + u * px;
            double y = y1 + u * py;

            double dx = x - x3;
            double dy = y - y3;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            return dist;

        }
        public bool IsPointOnSegment(Point p)
        {
            double a = this.Length;
            double b = pointA.CountDistance(p);
            double c = pointB.CountDistance(p);
            return Math.Pow(a, 2) <= Math.Pow(b, 2) + Math.Pow(c, 2) ? false : (CountDistance(p) < 0.01);
        }
        public bool WasClicked(Point p)
        {
            double a = this.Length;
            double b = pointA.CountDistance(p);
            double c = pointB.CountDistance(p);
            return Math.Pow(a, 2) <= Math.Pow(b, 2) + Math.Pow(c, 2) ? false : (CountDistance(p) < 5);
        }
        public object Clone()
        {
            Segment copy = new Segment((Point)this.pointA.Clone(), (Point)this.pointB.Clone());
            copy.isMarked = this.isMarked;
            copy.Tag = this.Tag;
            copy.color = this.color;
            return copy;
        }
        public object AppendCloneToExisitingPoint(Point p)
        {
            Segment copy = this.Clone() as Segment;
            copy.pointA = p;
            return copy;
        }
    }



    public class SegmentTagArgs
    {
        public Figure containingFigure;
        public SegmentTagArgs()
        {

        }
    }
}
