using System.Linq;
using UnityEngine;

public class GlossaryView : MonoBehaviour
{
    public GlossaryContainer list;
    public PartCompBase tooltip;


    public void OnOpened()
    {
        list.ResetFilters();
    }
}
