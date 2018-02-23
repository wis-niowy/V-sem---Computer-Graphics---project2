using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace V_sem___GK___projekt2
{
    public class Polygon : Figure, ICloneable
    {
        public List<Segment> segmentList;
        public override bool isMarked { get; set; }
        public Color[,] FillTexture { get; set; }
        public Color FillColor { get; set; }
        public Color[,] BumpMap { get; set; }
        public override int thickness
        {
            get
            {
                return base.thickness;
            }

            set
            {
                base.thickness = value;
                foreach (var seg in segmentList)
                {
                    seg.thickness = value;
                }
            }
        }
        private System.Drawing.Color colorvar;
        public override System.Drawing.Color color
        {
            get
            {
                return colorvar;
            }
            set
            {
                colorvar = value;
                foreach (var seg in segmentList)
                {
                    seg.color = value;
                }
            }
        }
        private System.Drawing.Color normalColorvar;
        public override System.Drawing.Color normalColor
        {
            get
            {
                return normalColorvar;
            }
            set
            {
                normalColorvar = value;
                foreach (var seg in segmentList)
                {
                    seg.normalColor = value;
                }
            }
        }
        private System.Drawing.Color markColorvar;
        public override System.Drawing.Color markColor
        {
            get
            {
                return markColorvar;
            }
            set
            {
                markColorvar = value;
                foreach (var seg in segmentList)
                {
                    seg.markColor = value;
                }
            }
        }
        public bool IsClockWise
        {
            get
            {
                double sum = 0.0;
                foreach (var segment in segmentList)
                {
                    sum += (segment.pointB.X - segment.pointA.X) * (segment.pointB.Y + segment.pointA.Y);
                }
                return sum < 0;
            }
        }

        public Polygon() : base()
        {
            segmentList = new List<Segment>();
            isMarked = false;
            FillTexture = null;
            FillColor = Color.White;
            thickness = 1;
        }
        /// <summary>
        /// SHALLOW copy constructor
        /// </summary>
        /// <param name="arg"></param>
        public Polygon(Polygon arg)
        {
            segmentList = arg.segmentList;
            isMarked = arg.isMarked;
            FillTexture = arg.FillTexture;
            FillColor = arg.FillColor;
            BumpMap = arg.BumpMap;
            thickness = arg.thickness;
            color = arg.color;
            markColor = arg.markColor;
            normalColor = arg.normalColor;

        }
        public override void Draw(Bitmap myBitmap)
        {
            foreach (var seg in segmentList)
            {
                seg.drawSegment(myBitmap);
            }
        }
        public void addSegment(Segment s)
        {
            s.Tag.containingFigure = this;
            segmentList.Add(s);
        }
        public bool removeSegment(Segment s)
        {
            s.Tag.containingFigure = null;
            return segmentList.Remove(s);
        }
        public override void MarkUnmark()
        {
            if (!isMarked)
            {
                isMarked = true;
                foreach (var seg in segmentList)
                {
                    seg.Mark();
                    seg.pointA.Mark();
                    seg.pointB.Mark();
                }
            }
            else
            {
                isMarked = false;
                foreach (var seg in segmentList)
                {
                    seg.Unmark();
                    seg.pointA.Unmark();
                    seg.pointB.Unmark();
                }
            }

        }
        public Segment getSegmentByIdx(int i)
        {
            return i < 0 ? null : segmentList.ElementAt(i);
        }
        public int getSegmentsNumber()
        {
            return segmentList.Count;
        }
        public override bool WasClicked(Point p)
        {
            bool wasClicked = false;
            foreach (var seg in segmentList)
            {
                if (seg.WasClicked(p) || seg.pointA.WasClicked(p) || seg.pointB.WasClicked(p))
                {
                    wasClicked = true;
                    break;
                }
            }
            return wasClicked;
        }
        public override bool MoveFigureWithinRectangle(Point deltaPoint, double xMax, double yMax)
        {
            bool returnValue = true;
            Polygon thisCopy = this.Clone() as Polygon;
            foreach (var seg in segmentList)
            {
                if (!seg.pointA.MoveWithinRectangle(deltaPoint, xMax, yMax))
                {
                    returnValue = false;
                    break;
                }
            }
            if (!returnValue)
            {
                this.segmentList = thisCopy.segmentList;
            }
            return returnValue;
        }
        public override void MoveFigure(Point deltaPoint)
        {
            foreach(var seg in segmentList)
            {
                seg.pointA.Move(deltaPoint);
            }
        }
        public void AddPointOnSegment(Point pointToAdd)
        {
            foreach (var seg in segmentList)
            {
                if (seg.IsPointOnSegment(pointToAdd))
                {
                    int index = segmentList.IndexOf(seg);
                    if (index < 0) return;

                    segmentList.RemoveAt(index);
                    segmentList.Insert(index, new Segment(seg.pointA, pointToAdd));
                    segmentList.Insert(index + 1, new Segment(pointToAdd, seg.pointB));
                    // UpdateSegmentsColor();
                    UpdateTags();
                    break;
                }        
            }
            
        }
        public static bool IsPointInsidePolygon(Polygon polygon, Point testPoint)
        {
            bool result = false;
            int j = polygon.segmentList.Count() - 1;
            for (int i = 0; i < polygon.segmentList.Count(); i++)
            {
                if (polygon.segmentList[i].pointA.Y < testPoint.Y && polygon.segmentList[j].pointA.Y >= testPoint.Y || polygon.segmentList[j].pointA.Y < testPoint.Y && polygon.segmentList[i].pointA.Y >= testPoint.Y)
                {
                    if (polygon.segmentList[i].pointA.X + (testPoint.Y - polygon.segmentList[i].pointA.Y) / (polygon.segmentList[j].pointA.Y - polygon.segmentList[i].pointA.Y) * (polygon.segmentList[j].pointA.X - polygon.segmentList[i].pointA.X) < testPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        public void UpdateTags()
        {
            foreach (var segment in segmentList)
            {
                segment.pointA.Tag.adherentSegments = new List<Segment>();
                segment.pointB.Tag.adherentSegments = new List<Segment>();
            }
            foreach (var segment in segmentList)
            {
                segment.pointA.Tag.adherentSegments.Add(segment);
                segment.pointB.Tag.adherentSegments.Add(segment);
            }
        }
        public override void ChangeColor(System.Drawing.Color col)
        {
            foreach (var seg in segmentList)
            {
                seg.color = col;
            }
        }
        public void UpdateSegmentsColor()
        {
            foreach (var seg in segmentList)
            {
                seg.color = this.color;
                seg.normalColor = this.normalColor;
                seg.markColor = this.markColor;
            }
        }
        public void UpdateFigureColor()
        {
            this.color = segmentList.ElementAt(0).color;
            this.normalColor = segmentList.ElementAt(0).normalColor;
            this.markColor = segmentList.ElementAt(0).markColor;
        }
        public override void Fill(Bitmap myBitmap, System.Drawing.Color lightColor, Vector3D lightCoords)
        {
            IScanLineFill slf = new ScanLineFill(this, lightColor, lightCoords);
            slf.FillFigure(myBitmap);

        }
        public override object Clone()
        {
            Polygon copy = new Polygon();
            foreach (var seg in segmentList)
            {
                //copy.segmentList.Add(seg.Clone() as Segment); // wrong way, as Segment.Clone() produces each vertice in our figure twice
                if (copy.segmentList.Count == 0)
                {
                    copy.segmentList.Add(seg.Clone() as Segment);
                }
                else
                {
                    Point pointToJoinSegment = copy.getSegmentByIdx(copy.segmentList.Count - 1).pointB;
                    Point figureInitialPoint = copy.getSegmentByIdx(0).pointA;
                    if (seg.pointB == this.getSegmentByIdx(0).pointA) // if currently copied segment is the last one
                    {
                        copy.segmentList.Add(new Segment(pointToJoinSegment, figureInitialPoint));
                    }
                    else
                    {
                        copy.segmentList.Add(seg.AppendCloneToExisitingPoint(pointToJoinSegment) as Segment);
                    }
                }
            }
            copy.isMarked = this.isMarked;
            copy.initDragPosition = this.initDragPosition;
            return copy;
        }
    }

    public class RandomPolygon: Polygon
    {
        public double Speed { get; set; }
        public double WayGone { get; set; }

        public RandomPolygon(Polygon polygon): base(polygon)
        {
            Speed = 0.0;
            WayGone = 0.0;
        }
    }
}
