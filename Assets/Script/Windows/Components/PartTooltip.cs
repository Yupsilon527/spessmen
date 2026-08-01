using TMPro;
using UnityEngine;

public class PartTooltip : MonoBehaviour
{
    public TextMeshProUGUI title, subtitle, description;

    private void Start()
    {
        Clear();
    }
    public void ShowPart(PartScriptable part)
    {
        if (title!=null)
        {
            title.text = part.InternalName;
        }
        if (subtitle != null)
        {
            subtitle.text = part.boonRarity + " " + part.partType;
        }
    }
    public void Clear()
    {
        if (title != null)
        {
            title.text = "";
        }
        if (subtitle != null)
        {
            subtitle.text = "";
        }
        if (description != null)
        {
            description.text = "";
        }
    }
}
