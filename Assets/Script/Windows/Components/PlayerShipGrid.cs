using UnityEngine;
using UnityEngine.UI;

public class PlayerShipGrid : MonoBehaviour
{
    DataItemShip myShip;
    public Image shipPreview;
    public GridPreview grid;

    public void AssignShip(DataItemShip ship)
    {
        myShip = ship;
        shipPreview.sprite = ship.scriptable.blueprint;
        UpdateGrid();

    }
    public void UpdateGrid()
    {
        grid.Draw(myShip.mGrid, myShip.width, myShip.height);
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        for (int y = 0; y < myShip.height; y++)
            for (int x=0; x< myShip.width; x++)
            {
                bool empty = myShip.Valid(x,y);
                var display = grid.tile[y * myShip.width + x];
                display.color = empty ? Color.white : Color.red;
            }
    }
}
