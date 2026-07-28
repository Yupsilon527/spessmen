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
        grid.Draw(ship._grid,ship.width,ship.height);

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
