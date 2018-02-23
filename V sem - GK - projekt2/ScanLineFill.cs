using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Media3D;

namespace V_sem___GK___projekt2
{
    public interface IScanLineFill
    {
        void FillFigure(Bitmap myBitmap);
    }

    public class ScanLineFill: IScanLineFill
    {
        public Figure Figure { get; set; }
        public List<AETStructure> AET;
        public int[] sortedIndexes { get; set; }
        public int encounteredVertices; // number of vertices encountered by a scanner so far
        public double scanLinePosition;
        System.Drawing.Color lightColor;
        Vector3D lightCoords;

        public ScanLineFill(Figure fig, System.Drawing.Color lightColor, Vector3D lightCoords)
        {
            Figure = fig;
            this.lightColor = lightColor;
            this.lightCoords = lightCoords;
            sortedIndexes = new int[(Figure as Polygon).segmentList.Count];
            AET = new List<AETStructure>();
            for (int i = 0; i < sortedIndexes.Length; ++i)
            {
                sortedIndexes[i] = i;
            }
            encounteredVertices = 0;
        }

        void IScanLineFill.FillFigure(Bitmap myBitmap)
        {
            Polygon polygon = Figure as Polygon;
            QuickSort(0, sortedIndexes.Length - 1);
            //BubbleSort();

            double yMin = polygon.segmentList[sortedIndexes[0]].pointA.Y;
            double yMax = polygon.segmentList[sortedIndexes[sortedIndexes.Length - 1]].pointA.Y;

            for (scanLinePosition = yMin + 1; scanLinePosition < yMax; ++scanLinePosition) // loop that moves the scaner
            {
                //   adding/removing active segments
                while (Math.Abs(polygon.segmentList[sortedIndexes[encounteredVertices]].pointA.Y - (scanLinePosition - 1)) < 1.0) // if point is on (scanLinePosition - 1)
                    // check if the next vertex of the polygon is on a previous scan line (next vertex is at encounteredVertices index)
                {
                    Point thisPoint = polygon.segmentList[sortedIndexes[encounteredVertices]].pointA;
                    Point prevPoint = polygon.segmentList[mod(sortedIndexes[encounteredVertices] - 1, polygon.segmentList.Count)].pointA;
                    Point nextPoint = polygon.segmentList[(sortedIndexes[encounteredVertices] + 1) % polygon.segmentList.Count].pointA;
                    if (prevPoint.Y >= thisPoint.Y)
                    {
                        AETStructure newEl = new AETStructure(polygon.segmentList[mod(sortedIndexes[encounteredVertices] - 1, polygon.segmentList.Count)], thisPoint.X);
                        AET.Add(newEl); // add segment: prev -- this
                    }
                    else
                    {
                        AET.RemoveAll(delegate (AETStructure s)
                        {
                            return s.segment == polygon.segmentList[mod(sortedIndexes[encounteredVertices] - 1, polygon.segmentList.Count)];  // remove segment: prev -- this
                        });
                    }
                    if (nextPoint.Y >= thisPoint.Y)
                    {
                        AETStructure newEl = new AETStructure(polygon.segmentList[sortedIndexes[encounteredVertices]], thisPoint.X);
                        AET.Add(newEl); // add segmet: this -- next
                    }
                    else
                    {
                        AET.RemoveAll(delegate (AETStructure s)
                        {
                            return s.segment == polygon.segmentList[sortedIndexes[encounteredVertices]];  // remove segment: prev -- this
                        });
                    }

                    encounteredVertices++;
                }
                // sorting active segments by x
                AET.Sort(delegate (AETStructure s1, AETStructure s2)
                {
                    if (s1.x > s2.x) return 1;
                    else if (s1.x < s2.x) return -1;
                    else return 0;
                });

                // coloring

                if (polygon.FillTexture != null)
                    PrintPixelLineFromTexture(myBitmap);
                else
                    PrintPixelLineFromColor(myBitmap);

                // updating x

                for (int i = 0; i < AET.Count; ++i)
                {
                    AET[i].x += 1 / AET[i].m;
                }

            }// for
        }

        private void PrintPixelLineFromColor(Bitmap myBitmap)
        {
            Polygon polygon = Figure as Polygon;
            for (int i = 0; i < AET.Count; i += 2)
            {
                for (double pixel = AET[i].x + 1; pixel < AET[i + 1].x; ++pixel)
                {
                    Color fillColor = polygon.FillColor;
                    if ((new Point(pixel, scanLinePosition)).IsPointInsideRectangle(myBitmap.Width, myBitmap.Height))
                    {
                        Vector3D lightVector = lightCoords - new Vector3D(pixel, scanLinePosition - 1, 0);
                        //Vector3D lightVector = new Vector3D(pixel, scanLinePosition - 1, 0) - lightCoords;
                        fillColor = LambertLightModel.GetColor(fillColor, lightColor, lightVector, (int)pixel, (int)scanLinePosition - 1, polygon.BumpMap);
                        myBitmap.SetPixel((int)pixel, (int)scanLinePosition - 1, fillColor);
                    }
                        
                }
            }
        }

        private void PrintPixelLineFromTexture(Bitmap myBitmap)
        {
            Polygon polygon = Figure as Polygon;
            double offset = 0.0; // offset which we include when getting pixel from a texture
            //double textureWidth = polygon.FillTexture.Width;
            //double textureHeight = polygon.FillTexture.Height;
            if (polygon is RandomPolygon)
            {
                offset = myBitmap.Width - (polygon as RandomPolygon).WayGone;
            }
            for (int i = 0; i < AET.Count; i += 2)
            {
                for (double pixel = AET[i].x + 1; pixel < AET[i + 1].x; ++pixel)
                {
                    Color fillColor = polygon.FillTexture[(int)(pixel - offset) % polygon.FillTexture.GetLength(0), ((int)scanLinePosition - 1) % polygon.FillTexture.GetLength(1)];

                    if ((new Point(pixel, scanLinePosition)).IsPointInsideRectangle(myBitmap.Width, myBitmap.Height))
                    {
                        Vector3D lightVector = lightCoords - new Vector3D(pixel, scanLinePosition - 1, 0);
                        //Vector3D lightVector = new Vector3D(pixel, scanLinePosition - 1, 0) - lightCoords;
                        fillColor = LambertLightModel.GetColor(fillColor, lightColor, lightVector, (int)pixel, (int)scanLinePosition - 1, polygon.BumpMap, offset);
                        myBitmap.SetPixel((int)pixel, (int)scanLinePosition - 1, fillColor);
                    }
                        
                }
            }
        }

        public void QuickSort(int left, int right)
        {
            Polygon polygon = Figure as Polygon;
            
            var i = left;
            var j = right;
            var pivot = polygon.segmentList[sortedIndexes[(left + right) / 2]].pointA.Y; // sortedIndexes[(left + right) / 2];
            while (i < j)
            {
                while (polygon.segmentList[sortedIndexes[i]].pointA.Y < pivot) i++;
                while (polygon.segmentList[sortedIndexes[j]].pointA.Y > pivot) j--;
                if (i <= j)
                {
                    // swap
                    var tmp = sortedIndexes[i];
                    sortedIndexes[i++] = sortedIndexes[j];  // ++ and -- inside sortedIndexes braces for shorter code
                    sortedIndexes[j--] = tmp;
                }
            }
            if (left < j) QuickSort(left, j);
            if (i < right) QuickSort(i, right);
        }

        public void BubbleSort()
        {
            Polygon polygon = Figure as Polygon;
            for (int i = 0; i < sortedIndexes.Length - 1; ++i)
                for (int j = 0; j < sortedIndexes.Length - 1 - i; ++j)
                {
                    if (polygon.segmentList[sortedIndexes[j]].pointA.Y > polygon.segmentList[sortedIndexes[j + 1]].pointA.Y)
                    {
                        var tmp = sortedIndexes[j];
                        sortedIndexes[j] = sortedIndexes[j + 1]; 
                        sortedIndexes[j + 1] = tmp;
                    }
                }
        }

        int mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }

        public class AETStructure
        {
            public double x;
            public double m;
            public Segment segment;

            public AETStructure(Segment s, double startX)
            {
                m = (s.pointB.Y - s.pointA.Y) / (s.pointB.X - s.pointA.X);
                x = startX;
                segment = s;
            }
        }
    }

    //public class ScanLineFillWithLight : IScanLineFill
    //{
    //    IScanLineFill scf;
    //    System.Drawing.Color lightColor;
    //    Vector3D lightVector;
    //    Bitmap bumpMap;

    //    public ScanLineFillWithLight(IScanLineFill slf, System.Drawing.Color lightColor, Vector3D lightVector, Bitmap bumpMap)
    //    {
    //        this.scf = slf;
    //        this.lightColor = lightColor;
    //        this.lightVector = lightVector;
    //        this.bumpMap = bumpMap;
    //    }

    //    void IScanLineFill.FillFigure(Bitmap myBitmap, System.Drawing.Color color)
    //    {
    //        Vector3D Nobj = new Vector3D(0, 0, 1);
    //        Vector3D N;
    //        Polygon polygon = (scf as ScanLineFill).Figure as Polygon;
    //        scf.FillFigure(myBitmap, color);

    //    }

    //    private Vector3D BlinnNormalVectorModification(int x, int y, int xMax, int yMax) // xMax and yMax - size of myBitmap
    //    {
    //        Vector3D T = new Vector3D(1, 0, 0);
    //        Vector3D B = new Vector3D(0, 1, 0);
            
    //        if (x > bumpMap.Width || y > bumpMap.Height)
    //        {
    //            return new Vector3D(0, 0, 0);
    //        }

    //        int r_x = bumpMap.GetPixel((x + 1) % xMax, y).R - bumpMap.GetPixel(x, y).R;
    //        int g_x = bumpMap.GetPixel((x + 1) % xMax, y).G - bumpMap.GetPixel(x, y).G;
    //        int b_x = bumpMap.GetPixel((x + 1) % xMax, y).B - bumpMap.GetPixel(x, y).B;

    //        int r_y = bumpMap.GetPixel(x, (y + 1) % xMax).R - bumpMap.GetPixel(x, y).R;
    //        int g_y = bumpMap.GetPixel(x, (y + 1) % xMax).G - bumpMap.GetPixel(x, y).G;
    //        int b_y = bumpMap.GetPixel(x, (y + 1) % xMax).B - bumpMap.GetPixel(x, y).B;

    //        T = T * r_x + T * g_x + T * b_x;
    //        B = B * r_y + B * g_y + B * b_y;

    //        return T + B;
    //    }

    //}
}
