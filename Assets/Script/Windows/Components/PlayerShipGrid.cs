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

    public void UpdateVisual()
    {
        for (int y = 0; y < myShip.height; y++)
            for (int x=0; x< myShip.width; x++)
            {
                bool empty = myShip.Valid(x,y);
                var display = grid.tile[y * myShip.width + x];
                display.color = empty ? colorEnabled : colorOccupied;
            }
    }
}
