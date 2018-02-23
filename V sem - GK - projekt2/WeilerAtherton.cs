using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V_sem___GK___projekt2
{
    public static class WeilerAtherton
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstPolygon">Subject polygon</param>
        /// <param name="secondPolygon">Clipping polygon - must be convex</param>
        /// <returns></returns>
        public static Polygon[] PerformAlgorithm(Polygon firstPolygon, Polygon secondPolygon)
        {
            List<Polygon> returnList = new List<Polygon>();
            Point[] intersectionPoints = FindIntersectionPoints(firstPolygon, secondPolygon);
            Polygon subjectPolygon = firstPolygon.Clone() as Polygon;
            Polygon clippingPolygon = secondPolygon.Clone() as Polygon;
            AddPointsToPolygon(subjectPolygon, intersectionPoints);
            AddPointsToPolygon(clippingPolygon, intersectionPoints);
            Polygon intersectionPolygon = new Polygon();
            Point intersectionPolygonStartingPoint = null; // starting point of currently edited intersection polygon
            int subjectReturnPoint = -1; // 'point' which we return to so as to continue traversing subject polygon, after we've finished recording new intersection polygon
            bool isNewIntersectionPolygonEdited = false; // true, if newly found intersection polygon recording is in progress 
            int startingIndex = 0;

            if (subjectPolygon.IsClockWise != clippingPolygon.IsClockWise)
            {
                clippingPolygon = ReverseVerticesOrder(clippingPolygon);
            }

            bool secondInsideFirst = true;
            foreach (var segment in secondPolygon.segmentList)
            {
                if(!Polygon.IsPointInsidePolygon(firstPolygon, segment.pointA))
                {
                    secondInsideFirst = false;
                    break;
                }
            }
            if (secondInsideFirst)
            {
                Polygon secondPolygonClone = secondPolygon.Clone() as Polygon;
                secondPolygonClone.FillColor = firstPolygon.FillColor;
                secondPolygonClone.FillTexture = firstPolygon.FillTexture;
                secondPolygonClone.BumpMap = firstPolygon.BumpMap;
                returnList.Add(secondPolygonClone);
                return returnList.ToArray();
            }

            while (Polygon.IsPointInsidePolygon(secondPolygon, subjectPolygon.segmentList[startingIndex].pointA) || subjectPolygon.segmentList[startingIndex].pointA.IsIntersectionPoint)
                // if starting point is inside clipping polygon OR current point is intersection point
                // we look for an offset to start traversing from a vertex outside of clippping polygon
            {
                startingIndex++;
                if (startingIndex == subjectPolygon.segmentList.Count) // we have traversed every subject polygon's vertex - entire subject is inside clipping polygon
                {
                    returnList.Add(firstPolygon);
                    return returnList.ToArray();
                }
            }

            for (int i = 0; i < subjectPolygon.segmentList.Count; ++i)
                // we traverse through every vertex of subject polygon
            {
                int vertexIndex = (i + startingIndex) % subjectPolygon.segmentList.Count; // index of currently viewed vertex (more precisely current segment)
                Segment currentSegment = subjectPolygon.segmentList[vertexIndex];

                if (!currentSegment.pointA.IsIntersectionPoint && !isNewIntersectionPolygonEdited)
                    // if we are outside of clipping polygon
                    continue;

                else if (currentSegment.pointA.IsIntersectionPoint && !isNewIntersectionPolygonEdited)
                    // we are entering clipping polygon 
                {
                    intersectionPolygon.addSegment(currentSegment.Clone() as Segment); // we add a first segment
                    intersectionPolygonStartingPoint = currentSegment.pointA;
                    isNewIntersectionPolygonEdited = true;
                }

                else if (!currentSegment.pointA.IsIntersectionPoint && isNewIntersectionPolygonEdited)
                    // we are on a non-intersection subject's vertex inside a clipping polygon
                {
                    Point pointToJoinSegment = intersectionPolygon.getSegmentByIdx(intersectionPolygon.segmentList.Count - 1).pointB;
                    intersectionPolygon.addSegment(currentSegment.AppendCloneToExisitingPoint(pointToJoinSegment) as Segment);
                }

                else if (currentSegment.pointA.IsIntersectionPoint && isNewIntersectionPolygonEdited)
                    // we are leaving clipping polygon - we jump to clipping polygon segment list and continue recording intersection polygon
                {
                    subjectReturnPoint = subjectPolygon.segmentList.IndexOf(currentSegment);
                    int clippingPolygonCurrentSegmentIndex = clippingPolygon.segmentList.IndexOf(clippingPolygon.segmentList.Find(delegate (Segment s)
                    {
                        return s.pointA == currentSegment.pointA;
                    }));
                    Point pointToJoinSegment = intersectionPolygon.getSegmentByIdx(intersectionPolygon.segmentList.Count - 1).pointB;
                    intersectionPolygon.addSegment(clippingPolygon.segmentList[clippingPolygonCurrentSegmentIndex].AppendCloneToExisitingPoint(pointToJoinSegment) as Segment);
                    clippingPolygonCurrentSegmentIndex = (clippingPolygonCurrentSegmentIndex + 1) % clippingPolygon.segmentList.Count;
                    while (!clippingPolygon.segmentList[clippingPolygonCurrentSegmentIndex].pointA.IsIntersectionPoint)
                        // we are on a non-intersection clipping polygon's vertex - we add this segment to a intersection polygon
                    {
                        //Segment segmentToAdd
                        pointToJoinSegment = intersectionPolygon.getSegmentByIdx(intersectionPolygon.segmentList.Count - 1).pointB;
                        intersectionPolygon.addSegment(clippingPolygon.segmentList[clippingPolygonCurrentSegmentIndex].AppendCloneToExisitingPoint(pointToJoinSegment) as Segment);
                        clippingPolygonCurrentSegmentIndex = (clippingPolygonCurrentSegmentIndex + 1) % clippingPolygon.segmentList.Count;
                    }

                    // now clippingPolygon.segmentList[clippingPolygonCurrentSegmentIndex].pointA is surely an intersection point
                    if (clippingPolygon.segmentList[clippingPolygonCurrentSegmentIndex].pointA == intersectionPolygonStartingPoint)
                        // we have recorded entire intersection polygon
                    {
                        isNewIntersectionPolygonEdited = false;
                        int segmentsNumber = intersectionPolygon.segmentList.Count;
                        Point polygonFirstPoint = intersectionPolygon.getSegmentByIdx(0).pointA;
                        intersectionPolygon.segmentList[segmentsNumber - 1].pointB = polygonFirstPoint;
                        returnList.Add(intersectionPolygon);
                        intersectionPolygon = new Polygon();
                    }
                    else
                        // we continue recording the intersection polygon (we jump back to a subject polygon segment list)
                    {
                        currentSegment = subjectPolygon.segmentList.Find(delegate (Segment s)
                        {
                            return s.pointA == clippingPolygon.segmentList[clippingPolygonCurrentSegmentIndex].pointA;
                        });
                        pointToJoinSegment = intersectionPolygon.getSegmentByIdx(intersectionPolygon.segmentList.Count - 1).pointB;
                        intersectionPolygon.addSegment(currentSegment.AppendCloneToExisitingPoint(pointToJoinSegment) as Segment);
                        int subjectPolygonCurrentSegmentIndex = subjectPolygon.segmentList.IndexOf(currentSegment);
                        // we set i one less than we want to have in next iteration
                        i = (subjectPolygon.segmentList.Count + subjectPolygonCurrentSegmentIndex - startingIndex) % subjectPolygon.segmentList.Count;
                    }
                }
            }

            foreach (var polygon in returnList)
            {
                foreach (var segment in polygon.segmentList)
                {
                    segment.pointA.IsIntersectionPoint = false;
                }
                polygon.FillColor = firstPolygon.FillColor;
                polygon.FillTexture = firstPolygon.FillTexture;
                polygon.BumpMap = firstPolygon.BumpMap;
            }

            return returnList.ToArray();
        }

        public static void AddPointsToPolygon(Polygon polygon, Point[] points)
        {
            foreach (var point in points)
            {
                polygon.AddPointOnSegment(point);
            }
        }

        public static Point[] FindIntersectionPoints(Polygon firstPolygon, Polygon secondPolygon)
        {
            List<Point> returnList = new List<Point>();
            foreach (var seg1 in firstPolygon.segmentList)
            {
                foreach (var seg2 in secondPolygon.segmentList)
                {
                    Point intersectionPoint = null;
                    if ((intersectionPoint = GetIntersectionPointOfSegments(seg1, seg2)) != null)
                    {
                        //if (!seg1.pointA.WasClicked(intersectionPoint) && !seg1.pointB.WasClicked(intersectionPoint) &&
                        //    !seg2.pointA.WasClicked(intersectionPoint) && !seg2.pointB.WasClicked(intersectionPoint))
                        //{
                            intersectionPoint.IsIntersectionPoint = true;
                            returnList.Add(intersectionPoint);
                        //}
                    }
                }
            }

            return returnList.ToArray();
        }

        ///// <summary>
        ///// Method prepares firstPolygon copy with additional intersection points of firstPolygon x secondPolygon
        ///// </summary>
        ///// <param name="firstPolygon">Polygon to be returned with added intersection points</param>
        ///// <param name="secondPolygon">Polygon for which we find intersection points with firstPolygon</param>
        ///// <returns>firstPolygon copy with additional intersection points included</returns>
        //public static Polygon PreparePolygonWithIntersectionPoints(Polygon firstPolygon, Polygon secondPolygon)
        //{
        //    Polygon returnPolygon = firstPolygon.Clone() as Polygon;

        //    foreach (var seg1 in firstPolygon.segmentList)
        //    // we create firstPolygon copy with additional intersection points on its edges
        //    {
        //        foreach (var seg2 in secondPolygon.segmentList)
        //        {
        //            Point intersectionPoint = null;
        //            if ((intersectionPoint = GetIntersectionPointOfSegments(seg1, seg2)) != null)
        //            {
        //                intersectionPoint.IsIntersectionPoint = true;
        //                returnPolygon.AddPointOnSegment(intersectionPoint);
        //            }
        //        }
        //    }
        //    return returnPolygon;
        //}

        /// <summary>
        /// Zwraca punkt przeciecia dwoch odcinkow (jezeli taki istnieje) wpp. null
        /// </summary>
        /// <param name="seg1">Odcinek pierwszy</param>
        /// <param name="seg2">Odcinek drugi</param>
        /// <returns>Punkt przeciecia prostych wyznaczonych przez odcinki</returns>
        public static Point GetIntersectionPointOfSegments(Segment seg1, Segment seg2)
        {
            if (IsSameSide(seg1.pointA, seg1.pointB, seg2) || IsSameSide(seg2.pointA, seg2.pointB, seg1))
                // segments do not intersect
            {
                return null;
            }
            else
            {
                return GetIntersectionPointOfLines(seg1, seg2);
            }
        }

        /// <summary>
        /// Zwraca punkt przeciecia dwoch prostych wyznaczonych przez odcinki
        /// </summary>
        /// <param name="seg1">Odcinek pierwszy</param>
        /// <param name="seg2">Odcinek drugi</param>
        /// <returns>Punkt przeciecia prostych wyznaczonych przez odcinki</returns>
        public static Point GetIntersectionPointOfLines(Segment seg1, Segment seg2)
        {
            Point direction1 = new Point(seg1.pointB.X - seg1.pointA.X, seg1.pointB.Y - seg1.pointA.Y);
            Point direction2 = new Point(seg2.pointB.X - seg2.pointA.X, seg2.pointB.Y - seg2.pointA.Y);
            double dotpointBrp = (direction1.X * direction2.Y) - (direction1.Y * direction2.X);

            Point c = new Point(seg2.pointA.X - seg1.pointA.X, seg2.pointA.Y - seg1.pointA.Y);
            double t = (c.X * direction2.Y - c.Y * direction2.X) / dotpointBrp;

            return new Point(seg1.pointA.X + (t * direction1.X), seg1.pointA.Y + (t * direction1.Y));
        }

        /// <summary>
        /// Sprawdza, czy dwa punkty leza po tej samej stronie prostej wyznaczonej przez odcinek s
        /// </summary>
        /// <param name="p1">Pierwszy punkt</param>
        /// <param name="p2">Drugi punkt</param>
        /// <param name="s">Odcinek wyznaczajacy prosta</param>
        /// <returns>
        /// True, jesli punkty leza po tej samej stronie prostej wyznaczonej przez odcinek 
        /// (lub co najmniej jeden z punktow lezy na prostej). W przeciwnym przypadku zwraca false.
        /// </returns>
        public static bool IsSameSide(Point p1, Point p2, Segment s)
        {
            int s1 = System.Math.Sign(Point.CrossProduct(s.pointA - s.pointB, p1 - s.pointB));
            int s2 = System.Math.Sign(Point.CrossProduct(s.pointA - s.pointB, p2 - s.pointB));
            if (s1 == 0 || s2 == 0)
                return true;
            return s1 == s2;
        }

        /// <summary>
        /// Reverses the order of vertices in a polygon, makes IsClockWIse = !IsClockWIse
        /// </summary>
        public static Polygon ReverseVerticesOrder(Polygon newPolygon)
        {
            newPolygon.segmentList.Reverse();
            foreach (var segment in newPolygon.segmentList)
            {
                Point temp = segment.pointA;
                segment.pointA = segment.pointB;
                segment.pointB = temp;
            }
            newPolygon.UpdateTags();
            return newPolygon;
        }
    }
}
