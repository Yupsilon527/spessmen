using UnityEngine;

public class ArmyMergeWindow : DragDropWindow
{
    public DataItemBanner unitA, unitB;
    public void MergeUnits(DataItemBanner a, DataItemBanner b)
    {
        unitA = a;
        unitB = b;

        dragdrop. Clear();
        dragdrop.InitSlots(a,b);
    }
}
