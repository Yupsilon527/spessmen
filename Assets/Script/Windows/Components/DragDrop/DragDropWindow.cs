using UnityEngine;

public class DragDropWindow : PlayerWindow
{
    public AbilityDragDropInterface dragdrop;
    protected override void OnClosed()
    {
        base.OnClosed();
        dragdrop.ApplyChanges();
        dragdrop.Clear();
    }
}
