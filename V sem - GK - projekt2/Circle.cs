using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Media3D;

namespace V_sem___GK___projekt2
{
    public class Circle : Figure, ICloneable
    {
        public Point Center { get; set; }
        public double Radius { get; set; }
        public override System.Drawing.Color color { get; set; }
        public override System.Drawing.Color normalColor { get; set; }
        public override System.Drawing.Color markColor { get; set; }
        public override bool isMarked { get; set; }
        public static double epsilon = 5;
        public List<Circle> ConcentricCircles; // pointer to collection - includes 'this' as well

        public Circle(Point c, double r) : base()
        {
            this.Center = c; this.Radius = r;
            normalColor = System.Drawing.Color.Black;
            markColor = System.Drawing.Color.DarkGreen;
            color = normalColor;
            isMarked = false;
            thickness = 1;
        }
        public override void Draw(Bitmap myBitmap)
        {
            double x0 = Center.X;
            double y0 = Center.Y;
            double radius = this.Radius;
            double x = radius - 1;
            double y = 0;
            double dx = 1;
            double dy = 1;
            double err = dx - (radius * 2);

            while (x >= y)
            {
                AddPixel(x0 + x, y0 + y, myBitmap, color);
                AddPixel(x0 + y, y0 + x, myBitmap, color);
                AddPixel(x0 - y, y0 + x, myBitmap, color);
                AddPixel(x0 - x, y0 + y, myBitmap, color);
                AddPixel(x0 - x, y0 - y, myBitmap, color);
                AddPixel(x0 - y, y0 - x, myBitmap, color);
                AddPixel(x0 + y, y0 - x, myBitmap, color);
                AddPixel(x0 + x, y0 - y, myBitmap, color);

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
            Center.Draw(myBitmap, 5);
        }
        public override void MarkUnmark()
        {
            if (!isMarked)
            {
                isMarked = true;
                color = markColor;
                Center.Mark();
            }
            else
            {
                isMarked = false;
                color = normalColor;
                Center.Unmark();
            }
        }
        public override bool WasClicked(Point p)
        {
            double distanceFromCircle = Math.Abs(p.CountDistance(this.Center) - this.Radius);
            return (distanceFromCircle < epsilon || this.Center.WasClicked(p)) ? true : false;
        }
        public override bool MoveFigureWithinRectangle(Point deltaPoint, double xMax, double yMax)
        {
            bool returnValue = true;
            Circle thisCopy = this.Clone() as Circle;
            if (Center.MoveWithinRectangle(deltaPoint, xMax, yMax))
            {
                if (!IsCircleInsideRectangle(xMax, yMax))
                {
                    returnValue = false;
                }
            }
            if (!returnValue)
            {
                this.Center = thisCopy.Center;
            }
            return returnValue;
        }
        public override void MoveFigure(Point deltaPoint)
        {
            Center.Move(deltaPoint);
        }
        private bool IsCircleInsideRectangle(double xMax, double yMax)
        {
            double circleSurrounding = this.Radius + epsilon + thickness;
            return (this.Center.X + circleSurrounding < xMax && this.Center.X - circleSurrounding > 0 && this.Center.Y + circleSurrounding < yMax && this.Center.Y - circleSurrounding > 0);
        }
        public bool SetRadius(double radius, double xMax, double yMax)
        {
            bool returnValue = true;
            double copyRadius = this.Radius;
            this.Radius = radius;
            if (!IsCircleInsideRectangle(xMax, yMax))
            {
                this.Radius = copyRadius;
                returnValue = false;
            }
            return returnValue;
        }
        private void AddPixel(double x0, double y0, Bitmap myBitmap, System.Drawing.Color color)
        {
            if (thickness == 1)
            {
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
        public override void ChangeColor(System.Drawing.Color col)
        {
            this.color = col;
        }
        public override void Fill(Bitmap myBitmap, System.Drawing.Color lightColor, Vector3D lightVector)
        {
            throw new NotImplementedException();
        }
        public override object Clone()
        {
            Circle copy = new Circle(Center.Clone() as Point, Radius);
            return copy;
        }
    }
}
