using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V_sem___GK___projekt2
{
    public static class Quickhull
    {
        public static Polygon CreateConvexPolygon(int xMin, int xMax, int yMin, int yMax)
        {
            Polygon returnPolygon = new Polygon();
            List<Point> points = new List<Point>();
            Random r = new Random();
            int section = r.Next(1, 4);
            int N = r.Next(4, 30);
            switch (section)
            {
                case 1:
                    for (int i = 0; i < N; i++)
                    {
                        int x = r.Next(xMax, (4 * xMax) / 3);
                        int y = r.Next(0, yMax / 2);
                        Point punkt = new Point(x, y);
                        points.Insert(i, punkt);
                    }
                    break;
                case 2:
                    for (int i = 0; i < N; i++)
                    {
                        int x = r.Next(xMax, (4 * xMax) / 3);
                        int y = r.Next(yMax / 3, (2 * yMax) / 3);
                        Point punkt = new Point(x, y);
                        points.Insert(i, punkt);
                    }
                    break;
                case 3:
                    for (int i = 0; i < N; i++)
                    {
                        int x = r.Next(xMax, (4 *xMax) / 3);
                        int y = r.Next(yMax / 2, yMax);
                        Point punkt = new Point(x, y);
                        points.Insert(i, punkt);
                    }
                    break;
            }
            

            List<Point> resultPolygonVertices = quickHull(points);
            for (int i = 0; i < resultPolygonVertices.Count; ++i)
            {
                returnPolygon.addSegment(new Segment(resultPolygonVertices[i], resultPolygonVertices[(i + 1) % resultPolygonVertices.Count]));
            }

            return returnPolygon;
        }

        public static List<Point> quickHull(List<Point> points)
        {
            List<Point> convexHull = new List<Point>();
            //if (points.Count < 3)
            //    return (List)points.clone();

            int minPoint = -1, maxPoint = -1;
            int minX = int.MaxValue;
            int maxX = int.MinValue;
            for (int i = 0; i < points.Count; i++)
            {
                if (points.ElementAt(i).X < minX)
                {
                    minX = (int)points.ElementAt(i).X;
                    minPoint = i;
                }
                if (points.ElementAt(i).X > maxX)
                {
                    maxX = (int)points.ElementAt(i).X;
                    maxPoint = i;
                }
            }
            Point A = points.ElementAt(minPoint);
            Point B = points.ElementAt(maxPoint);
            convexHull.Add(A);
            convexHull.Add(B);
            points.Remove(A);
            points.Remove(B);

            List<Point> leftSet = new List<Point>();
            List<Point> rightSet = new List<Point>();

            for (int i = 0; i < points.Count; i++)
            {
                Point p = points.ElementAt(i);
                if (pointLocation(A, B, p) == -1)
                    leftSet.Add(p);
                else if (pointLocation(A, B, p) == 1)
                    rightSet.Add(p);
            }
            hullSet(A, B, rightSet, convexHull);
            hullSet(B, A, leftSet, convexHull);

            return convexHull;
        }

        public static void hullSet(Point A, Point B, List<Point> set, List<Point> hull)
        {
            int insertPosition = hull.IndexOf(B);
            if (set.Count == 0)
                return;
            if (set.Count == 1)
            {
                Point p = set.ElementAt(0);
                set.Remove(p);
                hull.Insert(insertPosition, p);
                return;
            }
            int dist = int.MinValue;
            int furthestPoint = -1;
            for (int i = 0; i < set.Count; i++)
            {
                Point p = set.ElementAt(i);
                int distance = Distance(A, B, p);
                if (distance > dist)
                {
                    dist = distance;
                    furthestPoint = i;
                }
            }
            Point P = set.ElementAt(furthestPoint);
            set.RemoveAt(furthestPoint);
            hull.Insert(insertPosition, P);

            // Determine who's to the left of AP
            List<Point> leftSetAP = new List<Point>();
            for (int i = 0; i < set.Count; i++)
            {
                Point M = set.ElementAt(i);
                if (pointLocation(A, P, M) == 1)
                {
                    leftSetAP.Add(M);
                }
            }

            // Determine who's to the left of PB
            List<Point> leftSetPB = new List<Point>();
            for (int i = 0; i < set.Count; i++)
            {
                Point M = set.ElementAt(i);
                if (pointLocation(P, B, M) == 1)
                {
                    leftSetPB.Add(M);
                }
            }
            hullSet(A, P, leftSetAP, hull);
            hullSet(P, B, leftSetPB, hull);

        }

        public static int Distance(Point A, Point B, Point C)
        {
            int ABx = (int)(B.X - A.X);
            int ABy = (int)(B.Y - A.Y);
            int num = (int)(ABx * (A.Y - C.Y) - ABy * (A.X - C.X));
            if (num < 0)
                num = -num;
            return num;
        }

        public static int pointLocation(Point A, Point B, Point P)
        {
            int cp1 = (int)((B.X - A.X) * (P.Y - A.Y) - (B.Y - A.Y) * (P.X - A.X));
            if (cp1 > 0)
                return 1;
            else if (cp1 == 0)
                return 0;
            else
                return -1;
        }
    }
}
