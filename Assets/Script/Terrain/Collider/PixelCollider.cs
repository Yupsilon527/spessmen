using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PixelCollider : MonoBehaviour
{
    public float aTreshold = 0.5f;
    PolygonCollider2D polycollider;
    SpriteRenderer spriterenderer;
    private void Awake()
    {
        polycollider = GetComponent<PolygonCollider2D>();
        spriterenderer = GetComponent<SpriteRenderer>();
    }
    public void GenBounds()
    {
        if (GetComponent<SpriteRenderer>() == null || GetComponent<SpriteRenderer>().sprite == null)
        {
            polycollider.pathCount = 0;
            return;
        }

        List<Segment> segs;
        segs = CalcSegments(spriterenderer.sprite.texture);
        
        List<List<Vector2>> paths;
        paths = Find(segs);
        paths = Convert(paths, spriterenderer.sprite);

        paths = CalcPivot(paths, spriterenderer.sprite);
        polycollider.pathCount = paths.Count;
        for (int p = 0; p < paths.Count; p++)
        {
            polycollider.SetPath(p, paths[p].ToArray());
        }
    }

    private List<List<Vector2>> CalcPivot(List<List<Vector2>> paths, Sprite sprite)
    {
        Vector2 pivot = sprite.pivot;
        pivot.x *= Mathf.Abs(sprite.bounds.max.x - sprite.bounds.min.x) / sprite.texture.width;
        pivot.y *= Mathf.Abs(sprite.bounds.max.y - sprite.bounds.min.y) / sprite.texture.height;

        for (int x = 0; x < paths.Count; x++)
        {
            for (int y = 0; y < paths[x].Count; y++)
            {
                paths[x][y] -= pivot;
            }
        }
        return paths;
    }

    private struct Segment
    {
        public Vector2 startPoint;
        public Vector2 endPoint;
        public Segment(Vector2 start, Vector2 end)
        {
            this.startPoint = start;
            this.endPoint = end;
        }
    }

    private List<List<Vector2>> Convert(List<List<Vector2>> paths, Sprite sprite)
    {
        for (int x = 0; x < paths.Count; x++)
        {
            for (int y = 0; y < paths[x].Count; y++)
            {
                Vector2 currentpoint = paths[x][y];
                currentpoint.x *= Mathf.Abs(sprite.bounds.max.x - sprite.bounds.min.x) / sprite.texture.width;
                currentpoint.y *= Mathf.Abs(sprite.bounds.max.y - sprite.bounds.min.y) / sprite.texture.height;
                paths[x][y] = currentpoint;
            }
        }
        return paths;
    }

    List<List<Vector2>> Find(List<Segment> segments)
    {
        List<List<Vector2>> output = new List<List<Vector2>>();
        while (segments.Count > 0)
        {
            Vector2 point = segments[0].endPoint;
            List<Vector2> path = new List<Vector2> { segments[0].startPoint, segments[0].endPoint };
            segments.Remove(segments[0]);

            bool complete = false;
            while (complete == false)
            {
                complete = true;
                for (int s = 0; s < segments.Count; s++)
                {
                    if (segments[s].startPoint == point)
                    {
                        complete = false;
                        path.Add(segments[s].endPoint);
                        point = segments[s].endPoint;
                        segments.Remove(segments[s]);
                    }
                    else if (segments[s].endPoint == point)
                    {
                        complete = false;
                        path.Add(segments[s].startPoint);
                        point = segments[s].startPoint;
                        segments.Remove(segments[s]);
                    }
                }
            }
            output.Add(path);
        }
        return output;
    }

    List<Segment> CalcSegments(Texture2D texture)
    {
        List<Segment> final = new List<Segment>();
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                if (texture.GetPixel(x, y).a >= aTreshold)
                {
                    if (y + 1 >= texture.height || texture.GetPixel(x, y + 1).a < aTreshold)
                    {
                        final.Add(new Segment(new Vector2(x, y + 1), new Vector2(x + 1, y + 1)));
                    }
                    if (y - 1 < 0 || texture.GetPixel(x, y - 1).a < aTreshold)
                    {
                        final.Add(new Segment(new Vector2(x, y), new Vector2(x + 1, y)));
                    }
                    if (x + 1 >= texture.width || texture.GetPixel(x + 1, y).a < aTreshold)
                    {
                        final.Add(new Segment(new Vector2(x + 1, y), new Vector2(x + 1, y + 1)));
                    }
                    if (x - 1 < 0 || texture.GetPixel(x - 1, y).a < aTreshold)
                    {
                        final.Add(new Segment(new Vector2(x, y), new Vector2(x, y + 1)));
                    }
                }
            }
        }
        return final;
    }

}
