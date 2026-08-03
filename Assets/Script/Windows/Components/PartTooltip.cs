using TMPro;
using UnityEngine;

public class PartTooltip : MonoBehaviour
{
    public bool toggleActive = false;
    public TextMeshProUGUI title, subtitle, description, value;

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
        if (description != null)
        {
            description.text = part.GetEffectDescription() ;
        }
        if (value != null)
        {
            value.text = (part.GetBasePrice() * EconomyDefines.partResellPrice)+"g";
        }
        if (toggleActive) gameObject.SetActive(true);
    }
    public void Clear()
    {
        if (title != null)
        {
            title.text = "Part Info";
        }
        if (subtitle != null)
        {
            subtitle.text = "Mouse over part to show info";
        }
        if (description != null)
        {
            description.text = "";
        }
        if (value != null)
        {
            value.text = "";
        }
        if (toggleActive) gameObject.SetActive(false);
    }
}
