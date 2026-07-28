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

        int i = 0;
        foreach (var slot in GetComponentsInChildren<DragDropSlot>())
        {
            if (slot.slot == DragDropSlot.TokenSlot.setup)
            {
                slot.ship = ship;
                slot.position = i++;
            }
        }
    }
}
