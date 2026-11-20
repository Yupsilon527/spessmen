using UnityEngine.EventSystems;

public class PlayerTooltipButton : Initializable, IPointerEnterHandler, IPointerExitHandler
{
    public AbilityDragDropInterface parent;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if ( parent != null && parent.tooltip != null)
        {
            parent.tooltip.ShowCarData(DataItemPlayer.main.car);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (parent != null && parent.tooltip != null)
        {
            parent.tooltip.Clear();
        }
    }
}