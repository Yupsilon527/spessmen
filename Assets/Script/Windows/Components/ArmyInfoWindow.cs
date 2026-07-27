using UnityEngine;

public class ArmyInfoWindow : DragDropWindow
{
    public DataItemBanner assignedArmy;
    public void ForSingleArmy(DataItemBanner a)
    {
        assignedArmy = a;
        AssignPlayer(assignedArmy.GetPlayerOwner());
        dragdrop.InitSlots(a);
    }
}
