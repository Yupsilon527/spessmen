using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlixelChunk
{
    PlixelMapMob body;
    public Vector3Int start;
    public PolygonCollider2D foreground;
    public PolygonCollider2D background;
    public bool revised = false;
    public int nTiles = 1;

    public PlixelChunk (PlixelMapMob body, Vector2Int start,int bounds, GameObject[] parents)
    {
        this.body = body;
        this.start = new Vector3Int(start.x, start.y, bounds);
        
        foreground = parents[0].AddComponent<PolygonCollider2D>();
        background = parents[1].AddComponent<PolygonCollider2D>();
    }

    public void ReviseCollision()
    {
        Rect borders = new Rect(start.x * start.z, start.y * start.z, start.z, start.z);

        background.transform.rotation = Quaternion.Euler(Vector3.zero);
        List<List<Vector2>> bpaths = body.ReviseCollision(borders, false);
        background.pathCount = bpaths.Count;
        for (int p = 0; p < bpaths.Count; p++)
        {
            background.SetPath(p, bpaths[p].ToArray());
        }
        background.transform.localRotation = Quaternion.Euler(Vector3.zero);

        foreground.transform.rotation = Quaternion.Euler(Vector3.zero);
        List<List<Vector2>> fpaths = body.ReviseCollision(borders, true);
        foreground.pathCount = fpaths.Count;
        for (int p = 0; p < fpaths.Count; p++)
        {
            foreground.SetPath(p, fpaths[p].ToArray());
        }
        foreground.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    public int countTiles()
    {
        Vector2Int real_start = (Vector2Int)start * start.z;
        int n = 0;
        
        for (int iY = real_start.y; iY <= real_start.y + start.z; iY++)
        {
            for (int iX = real_start.x; iX <= real_start.x + start.z; iX++)
            {
                nTiles += (body.GetTileAt(iX, iY, true).IsSolid()) ? 0 : 1;
            }
        }
        return n;
    }

    public void Revise()
    {
        ReviseCollision();
        revised = true;
    }
}