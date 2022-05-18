using UnityEngine;
using UnityEditor;


public class PixelColliderSegment
{
    public Vector2 start;
    public Vector2 end;

    public PixelColliderSegment(Vector2 A, Vector2 B)
    {
        start = A; end = B;
    }
    public int isNeighboring(PixelColliderSegment other)
    {
        if (start == other.start)
        {
            return -2;
        }
        else if (start == other.end)
        {
            return -1;
        }
        else if (end == other.start)
        {
            return 1;
        }
        else if (end == other.end)
        {
            return 2;
        }
        return 0;
    }
    public bool Merge(PixelColliderSegment other)
    {
        int isNeighbor = isNeighboring(other);

        if (isNeighbor == 0 || (GetDirection() != other.GetDirection()))
        { return false; }


        if (start == other.start)
        {
            start = other.end;
        }
        else if (start == other.end)
        {
            start = other.start;
        }


        if (end == other.start)
        {
            end = other.end;
        }
        else if (end == other.end)
        {
            end = other.start;
        }


        return true;
    }
    public void FlipDirection()
    {
        Vector2 oStart = start;
        Vector2 oEnd = end;

        start = oEnd;
        end = oStart;
    }
    public int GetDirection()
    {
        Vector2 dir = (end - start);
        if (dir.x == 0)
        {
            return 1;
        }
        return 0;
    }
}
