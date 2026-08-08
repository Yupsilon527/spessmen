using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class PartTooltip : MonoBehaviour
{
    public bool toggleActive = false;
    public TextMeshProUGUI title, subtitle, description, value;
    public GridPreview grid;

    private void Start()
    {
        Clear();
    }
    public async void ShowPart(PartScriptable part, bool build)
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
                if (part.combos.Length > 0)
                {
                    string mergeLabel = await LocalizationSettings.StringDatabase .GetLocalizedStringAsync("UI Table", "CanMergeWith").Task;

                    var nameTasks = part.combos
                        .Select(combo => LocalizationSettings.StringDatabase
                            .GetLocalizedStringAsync("Parts", combo.other.InternalName).Task)
                        .ToArray();

                    string[] names = await Task.WhenAll(nameTasks);

                    description.text += "<br>" + mergeLabel + " " + string.Join(", ", names);
                }
                if (part.attach != ItemDefines.PartCondition.Anywhere)
                description.text += "<br>"+ await LocalizationSettings.StringDatabase .GetLocalizedStringAsync("Part Info", part.attach.ToString()).Task;
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
    public async void Clear()
    {
        var titleTask = LocalizationSettings.StringDatabase
            .GetLocalizedStringAsync("UI Table", "PartInfo").Task;
        var subtitleTask = LocalizationSettings.StringDatabase
            .GetLocalizedStringAsync("UI Table", "MouseOverHint").Task;

        await Task.WhenAll(titleTask, subtitleTask);

        if (title != null)
        {
            title.text = titleTask.Result;
        }
        if (subtitle != null)
        {
            subtitle.text = subtitleTask.Result;
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
