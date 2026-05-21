using UnityEngine;
using UnityEditor;


public class PixelColliderSegment
{
    public Vector2 start;
    public Vector2 end;

    public override string ToString()
    {
        return $"Segment {start} {end}";
    }
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
        return false;
        if (isNeighboring(other) == 0 || GetDirection() != other.GetDirection())
        {
            return false;
        }

        // Helper function to swap points if they match
        start = SwapPoints(start, other.start, other.end);
        end = SwapPoints(end, other.start, other.end);

        return true;
    }

    // Helper method to swap points
    private Vector2 SwapPoints(Vector2 point, Vector2 otherStart, Vector2 otherEnd)
    {
        if (point == otherStart) return otherEnd;
        if (point == otherEnd) return otherStart;
        return point;
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
    public override bool Equals(object obj)
    {
        if (obj is PixelColliderSegment segment)
        {
            return segment.start == start && segment.end == end
                || segment.end == start && segment.start == end;
        }
        return base.Equals(obj);
    }
}
