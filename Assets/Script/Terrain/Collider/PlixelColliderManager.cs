using System.Collections;
using System.Xml.Linq;
using UnityEngine;

public class PlixelCollisionManager : PlixelManager
{

    public static bool collisionDebug = true;
    public bool background = false;
    public ComponentPool comp;
    public int cellSize = 16;
    public float quality = 0;
    public PlixelChunk[,] chunks;

    public int cellCol, cellRow;
    public override void OnCreated()
    {
        cellCol = (parent.GetWidth() + cellSize - 1) / cellSize + 1;
        cellRow = (parent.GetHeight() + cellSize - 1) / cellSize + 1;

        chunks = new PlixelChunk[cellCol, cellRow];

        for (var cellY = 0; cellY < cellRow; cellY++)
        {
            for (var cellX = 0; cellX < cellCol; cellX++)
            {
                var cell = new PlixelChunk(parent, background);
                chunks[cellX, cellY] = cell;
                RebuildCell(cell, cellX, cellY);
            }
        }
        var w = parent.AlphaScale.x / 500 / TerrainDefines.terrain_PPU;
        var h = parent.AlphaScale.y / 500 / TerrainDefines.terrain_PPU;

        var offsetX = -parent.GetWidth() / TerrainDefines.terrain_PPU * 0.5f;
        var offsetY = -parent.GetHeight() / TerrainDefines.terrain_PPU * 0.5f;

        transform.localScale = new Vector3(w, h, 1.0f) * 500 / 255f;
        transform.localPosition = new Vector3(offsetX, offsetY, 0.0f);

        UpdateSolidState();
    }
    private void FixedUpdate()
    {
        Modify();
    }

    protected override IEnumerator ModifyCoroutine()
    {
        var cellXMin = workRect.xMin / cellSize;
        var cellYMin = workRect.yMin / cellSize;
        var cellXMax = (workRect.xMax + 1) / cellSize;
        var cellYMax = (workRect.yMax + 1) / cellSize;

        cellXMin = Mathf.Clamp(cellXMin, 0, chunks.GetLength(0) - 1);
        cellXMax = Mathf.Clamp(cellXMax, 0, chunks.GetLength(0) - 1);
        cellYMin = Mathf.Clamp(cellYMin, 0, chunks.GetLength(1) - 1);
        cellYMax = Mathf.Clamp(cellYMax, 0, chunks.GetLength(1) - 1);

        for (var cellY = cellYMin; cellY <= cellYMax; cellY++)
        {
            for (var cellX = cellXMin; cellX <= cellXMax; cellX++)
            {
                ClearCell(chunks[cellX, cellY]);
            }
        }

        for (var cellY = cellYMin; cellY <= cellYMax; cellY++)
        {
            for (var cellX = cellXMin; cellX <= cellXMax; cellX++)
            {
                var cell = chunks[cellX, cellY];
                RebuildCell(cell, cellX, cellY);
                if (Step()) yield return null;
            }
        }
        EndWork();
    }

    private void RebuildCell(PlixelChunk cell, int cellX, int cellY)
    {
        var x = cellSize * (cellX - 1);
        var y = cellSize * (cellY - 1);
        var xMin = x - 1;
        var yMin = y - 1;
        var xMax = Mathf.Min(x + cellSize, parent.GetWidth());
        var yMax = Mathf.Min(y + cellSize, parent.GetHeight());

        cell.revision = new RectInt((xMin + xMax) / 2, (yMin + yMax) / 2, xMax - xMin, yMax - yMin);
        cell.CalculateCells();
        cell.Build(this, cell.revision);
    }

    private void ClearCell(PlixelChunk chunk)
    {
        if (chunk.Shapes != null)
        {
            for (var j = chunk.Shapes.Count - 1; j >= 0; j--)
            {
                var ring = chunk.Shapes[j];

                if (ring.Collider != null)
                {
                    ring.Collider.pathCount = 0;
                    comp.DeactivateComponent(ring.Collider);
                }
            }
            chunk.Shapes.Clear();
        }
    }
    public void UpdateSolidState()
    {
        if (collisionDebug)
            Debug.Log("[entityTerrain] " + name + " tryupdate collision state");

        if (parent.tilesTotal == 0)
        {
            parent.Kill();
        }
        else
        {
            parent.rigidbody.bodyType = (parent.tilesSolid > 0) ? RigidbodyType2D.Static : RigidbodyType2D.Dynamic;
        }
    }
}
