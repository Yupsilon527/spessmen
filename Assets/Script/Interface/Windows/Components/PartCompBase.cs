using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UI;

public class PartCompBase : Initializable
{
    public bool toggleActive = false;
    public Image partIcon;
    public TextMeshProUGUI title, subtitle, description, value;
    public GridPreview grid;
    public virtual void ShowPart(PartScriptable part)
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
        if (toggleActive) gameObject.SetActive(true);
    }
    private void Start()
    {
        Clear();
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
        if (toggleActive) gameObject.SetActive(false);
    }
}
