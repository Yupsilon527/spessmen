using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlixelChunk
{
    public PlixelChunk(PlixelMapMob parent, bool background)
    {
        this.parent = parent;
        isBackground = background;

        realWidth = parent.GetWidth();
        realHeight = parent.GetHeight();

        D2dPolygonSquares();
    }
    PlixelMapMob parent;
    public bool isBackground = false;
    public List<PlixelCollision> Shapes = new List<PlixelCollision>();

    enum Side
    {
        Left,
        Right,
        Bottom,
        Top
    }

    class Square
    {
        public int Mask;
        public int Index;
        public Point PointL;
        public Point PointR;
        public Point PointB;
        public Point PointT;
    }

    struct Point
    {
        public int x;
        public int y;
        public bool e;

        public static Point operator -(Point a, Point b) { a.x -= b.x; a.y -= b.y; return a; }
        public static implicit operator Vector2(Point a) { return new Vector2(a.x, a.y); }
    }

    private static List<Square> squares = new List<Square>();

    private static List<Square> activeSquares = new List<Square>();

    private static Square square;
    private static Side squareSide;
    private static int squareIndex;
    private static int squareWidth;
    private static int squareHeight;


    public int realWidth;
    public int realHeight;

    public RectInt revision;
    RectInt realrevision;

    private List<Point> points = new List<Point>();

    private List<Vector2> weldedPoints = new List<Vector2>();

    private List<PlixelLine> paths = new List<PlixelLine>();

    private static int pathCount;

    private byte GetPolyAlpha(int x, int y)
    {
        if (x >= 0 && x >= revision.xMin && x < revision.xMax)
        {
            if (y >= 0 && y >= revision.yMin && y < revision.yMax)
            {
                Plixel tile = parent.GetTileAt(x, y);
                return (byte)(tile?.IsSolid(isBackground) ?? false ? 255 : 0);
            }
        }

        return 0;
    }

    private int[,] subSquares = new int[256, 256];

    void D2dPolygonSquares()
    {
        for (var i = 0; i < 256; i++)
        {
            for (var j = 0; j < 256; j++)
            {
                var l = i - 128;
                var r = i - j;

                if (r != 0)
                {
                    subSquares[i, j] = (l * 255) / r;
                }
            }
        }
    }

    public void CalculateCells()
    {
        revision.xMin = Mathf.Max(0, revision.xMin);
        revision.xMax = Mathf.Min(realWidth, revision.xMax + 1);
        revision.yMin = Mathf.Max(0, revision.yMin);
        revision.yMax = Mathf.Min(realHeight, revision.yMax + 1);

        squareWidth = revision.xMax - revision.xMin + 2;
        squareHeight = revision.yMax - revision.yMin + 2;
        realrevision.xMin = revision.xMin * 255;
        realrevision.xMax = revision.xMax * 255 - 255;
        realrevision.yMin = revision.yMin * 255;
        realrevision.yMax = revision.yMax * 255 - 255;

        var reserve = squareWidth * squareHeight - squares.Count;

        for (var i = reserve; i > 0; i--)
            squares.Add(new Square());

        activeSquares.Clear();

        for (var y = revision.yMin - 1; y <= revision.yMax; y++)
        {
            var o = (y - revision.yMin + 1) * squareWidth - revision.xMin + 1;
            var bl = GetPolyAlpha(0, y); var useBl = bl >= 128;
            var tl = GetPolyAlpha(0, y + 1); var useTl = tl >= 128;

            for (var x = revision.xMin - 1; x <= revision.xMax; x++)
            {
                var square = squares[x + o];
                var br = GetPolyAlpha(x + 1, y); var useBr = br >= 128;
                var tr = GetPolyAlpha(x + 1, y + 1); var useTr = tr >= 128;
                var mask = ((useBl ? 1 : 0) + (useBr ? 2 : 0) + (useTl ? 4 : 0) + (useTr ? 8 : 0)) % 15;

                if (mask > 0)
                {
                    var xp = x * 255;
                    var yp = y * 255;

                    square.Mask = mask;
                    square.Index = x + o;

                    activeSquares.Add(square);

                    if (useBl ^ useBr) square.PointB = ClampPoint(xp + subSquares[bl, br], yp);
                    if (useTl ^ useTr) square.PointT = ClampPoint(xp + subSquares[tl, tr], yp + 255);
                    if (useBl ^ useTl) square.PointL = ClampPoint(xp, yp + subSquares[bl, tl]);
                    if (useBr ^ useTr) square.PointR = ClampPoint(xp + 255, yp + subSquares[br, tr]);
                }

                bl = br; useBl = useBr;
                tl = tr; useTl = useTr;
            }
        }
    }

    private Point ClampPoint(int x, int y)
    {
        var e = false;

        if (x < realrevision.xMin) { x = realrevision.xMin; e = true; } else if (x > realrevision.xMax) { x = realrevision.xMax; e = true; }
        if (y < realrevision.yMin) { y = realrevision.yMin; e = true; } else if (y > realrevision.yMax) { y = realrevision.yMax; e = true; }

        return new Point { x = x, y = y, e = e };
    }

    private Vector2[] Trace(float straighten)
    {
        while (true)
        {
            switch (square.Mask)
            {
                case 0:
                    {
                        weldedPoints.Clear();

                        var head = points[0];
                        var delta = default(Point);
                        var direction = default(Vector2);
                        var edge = false;

                        weldedPoints.Add(head);

                        straighten = 1.0f - straighten;

                        for (var i = points.Count - 1; i >= 1; i--)
                        {
                            var point = points[i];

                            if (point.x != head.x || point.y != head.y)
                            {
                                var newDelta = point - head;
                                var newDirection = ((Vector2)newDelta).normalized;
                                var different = newDelta.x != delta.x || newDelta.y != delta.y;
                                var newEdge = point.e;

                                if (different == true && (newEdge != edge || Vector2.Dot(direction, newDirection) < straighten))
                                {
                                    delta = newDelta;
                                    direction = newDirection;

                                    weldedPoints.Add(point);
                                }
                                else
                                {
                                    weldedPoints[weldedPoints.Count - 1] = point;
                                }

                                edge = newEdge;
                                head = point;
                            }
                        }
                    }
                    return weldedPoints.ToArray();

                case 1: square.Mask = 0; SubmitL(); break;

                case 2: square.Mask = 0; SubmitB(); break;

                case 3: square.Mask = 0; SubmitL(); break;

                case 4: square.Mask = 0; SubmitT(); break;

                case 5: square.Mask = 0; SubmitT(); break;

                case 6:
                    {
                        if (squareSide == Side.Right)
                        {
                            square.Mask = 14; SubmitT();
                        }
                        else
                        {
                            square.Mask = 7; SubmitB();
                        }
                    }
                    break;

                case 7: square.Mask = 0; SubmitT(); break;

                case 8: square.Mask = 0; SubmitR(); break;

                case 9:
                    {
                        if (squareSide == Side.Top)
                        {
                            square.Mask = 13; SubmitL();
                        }
                        else
                        {
                            square.Mask = 11; SubmitR();
                        }
                    }
                    break;

                case 10: square.Mask = 0; SubmitB(); break;

                case 11: square.Mask = 0; SubmitL(); break;

                case 12: square.Mask = 0; SubmitR(); break;

                case 13: square.Mask = 0; SubmitR(); break;

                case 14: square.Mask = 0; SubmitB(); break;
            }

            square = squares[squareIndex];
        }
    }

    private void SubmitL()
    {
        points.Add(square.PointL); squareIndex -= 1; squareSide = Side.Right;
    }

    private void SubmitR()
    {
        points.Add(square.PointR); squareIndex += 1; squareSide = Side.Left;
    }

    private void SubmitB()
    {
        points.Add(square.PointB); squareIndex -= squareWidth; squareSide = Side.Top;
    }

    private void SubmitT()
    {
        points.Add(square.PointT); squareIndex += squareWidth; squareSide = Side.Bottom;
    }

    public void Build(PlixelCollisionManager parent, RectInt bounds)
    {
        pathCount = 0;
        revision = bounds;

        for (var i = 0; i < activeSquares.Count; i++)
        {
            square = activeSquares[i];

            if (square.Mask != 0)
            {
                squareIndex = square.Index;

                points.Clear();

                Vector2[] newPoints = Trace(parent.quality);

                if (newPoints != null && newPoints.Length > 2)
                {
                    PlixelLine path = GetNextPath();

                    path.coords = newPoints;
                }
            }
        }

        if (pathCount > 0)
        {
            SortPaths();
        }

        for (var i = 0; i < pathCount; i++)
        {
            var path = paths[i];
            var shapeIndex = FindShapeIndex(path.start);

            if (shapeIndex >= 0)
            {
                var shape = Shapes[shapeIndex];

                shape.Gaps.Add(path);

                shape.Collider.pathCount += 1;

                shape.Collider.SetPath(shape.Collider.pathCount - 1, path.coords);
            }
            else
            {
                PlixelCollision shape = new();

                shape.Outside = path;
                shape.Gaps.Clear();

                shape.Collider = parent.comp.PoolComponent<PolygonCollider2D>(true);

                shape.Collider.SetPath(0, path.coords);

                Shapes.Add(shape);
            }
        }
    }

    private int FindShapeIndex(Vector2 point)
    {
        for (var i = 0; i < paths.Count; i++)
        {
            var shape = paths[i];

            if (shape.Contains(point) == true)
            {
                return i;
            }
        }

        return -1;
    }

    private void SortPaths()
    {
        for (var i = 0; i < pathCount; i++)
        {
            paths[i].Calc();
        }

        paths.Sort(0, pathCount, paths[0]);
    }

    private PlixelLine GetNextPath()
    {
        var path = default(PlixelLine);

        if (pathCount >= paths.Count)
        {
            path = new();
            paths.Add(path);
        }
        else
        {
            path = paths[pathCount];
        }

        pathCount += 1;

        return path;
    }
}

[System.Serializable]
public class PlixelLine : IComparer<PlixelLine>
{
    public Vector2[] coords;
    public Vector2 start;

    public void Calc()
    {
        start.x = float.PositiveInfinity;

        for (var i = coords.Length - 1; i >= 0; i--)
        {
            var point = coords[i];

            if (point.x < start.x)
            {
                start = point;
            }
        }
    }

    public static float Side(Vector2 a, Vector2 b, Vector2 p)
    {
        return (b.y - a.y) * (p.x - a.x) - (b.x - a.x) * (p.y - a.y);
    }

    public bool Contains(Vector2 point)
    {
        var total = 0;
        var pointA = coords[0];

        for (var j = coords.Length - 1; j >= 0; j--)
        {
            var pointB = coords[j];

            if (pointA.y <= point.y)
            {
                if (pointB.y > point.y && Side(pointA, pointB, point) > 0.0f) total += 1;
            }
            else
            {
                if (pointB.y <= point.y && Side(pointA, pointB, point) < 0.0f) total -= 1;
            }

            pointA = pointB;
        }

        return total != 0;
    }

    public int Compare(PlixelLine a, PlixelLine b)
    {
        return a.start.x.CompareTo(b.start.x);
    }
}

[System.Serializable]
public class PlixelCollision
{
    public PolygonCollider2D Collider;

    public PlixelLine Outside;

    public List<PlixelLine> Gaps = new List<PlixelLine>();

    public bool Contains(Vector2 point)
    {
        if (Outside.Contains(point) == true)
        {
            for (var i = 0; i < Gaps.Count; i++)
            {
                if (Gaps[i].Contains(point) == true)
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }
}

