using System.Linq;
using TMPro;
using UnityEngine.UI;

public class PartCompBase : Initializable
{
    public bool toggleActive = false;
    public Image partIcon;
    public TextMeshProUGUI title, subtitle, description, value;
    public GridPreview grid;
    public Graphic[] rarityColors;
    public virtual void ShowPart(PartScriptable part, bool showCombos)
    {
        if (title != null)
        {
            title.text = LanguageController.main.Translate("Parts", part.InternalName);
        }
        if (partIcon != null)
        {
            partIcon.enabled = part.icon!=null;
            partIcon.sprite = part.icon;
        }
        if (subtitle != null)
        {
            subtitle.text = LanguageController.main.Translate("Abilities", "rarity_" + part.boonRarity) + " " + LanguageController.main.Translate("Abilities", "class_" + part.partType);
        }
        if (description != null)
        {
            description.text = part.GetEffectDescription();
        }
        if (grid != null)
        {
            grid.Draw(part.grid);
        }
        if (rarityColors != null)
        {
            foreach (var g in rarityColors)
                g.color = ItemDefines.GetColorForRarity(part.boonRarity);
        }
        if (showCombos && part.combos.Length > 0)
        {
            string mergeLabel = LanguageController.main.Translate("UI Table", "CanMergeWith");
            string[] names = part.combos.Select(combo => LanguageController.main.Translate("Parts", combo.other.InternalName)).ToArray();
            description.text += "<br>" + mergeLabel + " " + string.Join(", ", names);
        }
        if (toggleActive) gameObject.SetActive(true);
    }
    public virtual void Clear()
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
        if (partIcon != null)
        {
            partIcon.enabled = false;
        }
        if (rarityColors != null)
        {
            foreach (var g in rarityColors)
                g.color = UnityEngine.Color.clear;
        }
        if (toggleActive) gameObject.SetActive(false);
    }
}
