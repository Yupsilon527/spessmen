using UnityEngine;
using UnityEngine.UI;

public class PlayerShipGrid : MonoBehaviour
{
    
    public Image shipPreview;
    public GridPreview grid;

    public void AssignShip(DataItemShip ship)
    {
        shipPreview.sprite = ship.scriptable.blueprint;
        grid.Draw(ship.value,ship.width,ship.height);

    }
}
