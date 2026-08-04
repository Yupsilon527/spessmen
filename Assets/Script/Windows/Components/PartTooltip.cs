using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PartTooltip : MonoBehaviour
{
    public bool toggleActive = false;
    public TextMeshProUGUI title, subtitle, description, value;
    public GridPreview grid;

    private void Start()
    {
        Clear();
    }
    public void ShowPart(PartScriptable part, bool build)
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
            if (build)
            {
                if (part.combos.Length >0)
                description.text += "<br>Can merge with "+string.Join(',', part.combos.Select(combo =>combo.other.InternalName));
                if (part.attach != ItemDefines.PartCondition.Anywhere)
                description.text += "<br>"+part.attach ;
            }
        }
        if (grid!=null)
        {
            grid.Draw(part.grid);
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
        if (grid != null)
        {
            grid.Clear();
        }
        if (value != null)
        {
            value.text = "";
        }
        if (toggleActive) gameObject.SetActive(false);
    }
}
