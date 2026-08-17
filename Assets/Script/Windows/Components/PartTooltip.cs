using System.Linq;
using TMPro;
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
    public  void ShowPart(PartScriptable part, bool build)
    {
        if (title!=null)
        {
            title.text = LanguageController.main.Translate("Parts", part.InternalName);
        }
        if (subtitle != null)
        {
            subtitle.text = LanguageController.main.Translate("Abilities", "rarity_"+part.boonRarity) + " " + LanguageController.main.Translate("Abilities", "class_" + part.partType);
        }
        if (description != null)
        {
            description.text = part.GetEffectDescription() ;
            if (build)
            {
                if (part.combos.Length > 0)
                {
                    string mergeLabel = LanguageController.main.Translate("UI Table", "CanMergeWith");
                    string[] names = part.combos.Select(combo => LanguageController.main.Translate("Parts", combo.other.InternalName))  .ToArray();
                    description.text += "<br>" + mergeLabel + " " + string.Join(", ", names);
                }
                if (part.attach != ItemDefines.PartCondition.Anywhere)
                description.text += "<br>"+ LanguageController.main.Translate("UI Table", "condition_"+part.attach);
            }
        }
        if (grid!=null)
        {
            grid.Draw(part.grid);
        }
        if (value != null)
        {
            value.text = "$"+(part.GetBasePrice() * EconomyDefines.partResellPrice);
        }
        if (toggleActive) gameObject.SetActive(true);
    }
    public  void Clear()
    {
        if (title != null)
        {
            title.text = LanguageController.main.Translate("UI Table", "PartInfo");
        }
        if (subtitle != null)
        {
            subtitle.text = LanguageController.main.Translate("UI Table", "MouseOverHint");
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
