using System.Linq;
using UnityEngine;

public class GlossaryView : MonoBehaviour
{
    public GlossaryContainer list;
    public PartTooltip tooltip;


    public void OnOpened()
    {
        list.ResetFilters();
    }
}
