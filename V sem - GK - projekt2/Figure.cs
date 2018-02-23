using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Media3D;

namespace V_sem___GK___projekt2
{
    public abstract class Figure: ICloneable
    {
        public Point initDragPosition { get; set; } // used to find a vector of figure's movement
        abstract public System.Drawing.Color color { get; set; }
        abstract public System.Drawing.Color normalColor { get; set; }
        abstract public System.Drawing.Color markColor { get; set; }
        virtual public int thickness { get; set; }

        public Figure()
        {
            initDragPosition = new Point(-1, -1);
        }

        virtual public void Draw(Bitmap myBitmap) { }
        //virtual public void Erase(Canvas myCnavas) { }
        abstract public bool isMarked { get; set; }
        abstract public bool WasClicked(Point p);
        abstract public void MarkUnmark();
        abstract public bool MoveFigureWithinRectangle(Point p, double xMax, double yMax);
        abstract public void MoveFigure(Point p);
        abstract public void ChangeColor(System.Drawing.Color col);
        abstract public void Fill(Bitmap myBitmap, System.Drawing.Color lightColor, Vector3D lightVector);
        abstract public object Clone();
    }
}
