using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShipGrid : MonoBehaviour
{
    DataItemShip myShip;
    public Image shipPreview;
    public GridPreview grid;

    [Header("Colors")]
    public Color colorEnabled = Color.white;
    public Color colorOccupied = Color.white;

    public Color colorValid = Color.white;
    public Color colorInvalid = Color.gray;

    public Color colorCanPlace = Color.green;
    public Color colorBloxked = Color.red;

    public void AssignShip(DataItemShip ship)
    {
        myShip = ship;
        shipPreview.sprite = ship?.scriptable?.blueprint;
        UpdateGrid();

    }
    public void UpdateGrid()
    {
        if (myShip == null) return;
        grid.Draw(myShip);
        UpdateVisual();
    }
    public void UpdatePlacementCondition(DataItemPart part, int rotation)
    {
        HashSet<Vector2Int> possible = new();
        for (int y = 0; y < myShip.height; y++)
            for (int x = 0; x < myShip.width; x++)
            {
                bool placeable = myShip.CanPlace(part, x, y, rotation);
                if (placeable) {
                    possible.Add(new Vector2Int(x, y));
                }
                var display = grid.tile[y * myShip.width + x];
                display.color = colorInvalid;
            }

        if (possible.Count == 0) return;
        bool[,] shape = part.RetrieveRotated(rotation);
        foreach (var slot in possible)
            HighlightShape(slot.x , slot.y, shape, shape.GetLength(0), shape.GetLength(1), colorValid);
    }
    public void UpdatePartPosition(DataItemPart part, Vector3 center, int rotation)
    {
        bool[,] shape = part.RetrieveRotated(rotation);
        Vector2Int slotCoords = grid.GetGridPosition(center);

        foreach (var d in ShipDefines.deltaPos)
        {
            if (DataItemPlayer.main.car.CanPlace(part, slotCoords.x + d.x, slotCoords.y + d.y, rotation))
            {
                HighlightShape(slotCoords.x + d.x, slotCoords.y + d.y, shape, shape.GetLength(0), shape.GetLength(1), colorCanPlace);
                return;
            }
        }
        HighlightShape(slotCoords.x, slotCoords.y, shape, shape.GetLength(0), shape.GetLength(1), colorBloxked);
    }
    void HighlightShape(int cx, int cy, bool[,] shape, int w, int h, Color color)
    {
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (!shape[x, y]) continue;
                int px = cx + x;
                int py = cy + y;

                int d = py * myShip.width + px;
                if (d < 0 || d >= grid.tile.Length) continue;
                var display = grid.tile[d];
                display.color = color;
            }
        }
    }
    public void UpdateVisual()
    {
        for (int y = 0; y < myShip.height; y++)
            for (int x = 0; x < myShip.width; x++)
            {
                bool empty = myShip.Valid(x, y);
                var display = grid.tile[y * myShip.width + x];
                display.color = empty ? colorEnabled : colorOccupied;
            }
    }
}
