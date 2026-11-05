using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GlossaryContainer : ContainerMenu
{
    public GlossaryView parent;
    public List<PartScriptable> filtered = new();
    public List<GlossaryButton> buttons = new();
    public bool rarityCommom { get; set; }
    public bool rarityRare { get; set; }
    public bool rarityEpic { get; set; }
    public bool rarityLegendary { get; set; }
    public bool partWheel { get; set; }
    public bool partTank { get; set; }
    public bool partEngine { get; set; }
    public bool partGadget { get; set; }
    public bool partNitro { get; set; }
    public bool partDecal { get; set; }
    public bool partExpansion { get; set; }
    public bool setRarityCommong { get; set; }
    public Toggle commonToggle, rareToggle, epicToggle, legendaryToggle, wheelToggle, tankToggle, engineToggle, gadgetToggle, nitroToggle, decalToggle, expansionToggle;

    public void FilterParts()
    {
        filtered = ResourceCache.main.parts.Where(p =>
#if UNITY_EDITOR
    MatchesRarity(p)
     && MatchesPartType(p)
#else
        p.IsUnlocked() 
      && MatchesRarity(p)
     && MatchesPartType(p)
#endif
    ).ToList();
        Refresh();
    }

    private bool MatchesRarity(PartScriptable p)
    {
        return (rarityCommom && p.boonRarity == ItemDefines.BoonRarity.common)
            || (rarityRare && p.boonRarity == ItemDefines.BoonRarity.rare)
            || (rarityEpic && p.boonRarity == ItemDefines.BoonRarity.epic)
            || (rarityLegendary && p.boonRarity == ItemDefines.BoonRarity.legendary);
    }

    private bool MatchesPartType(PartScriptable p)
    {
        return (partWheel && p.partType == ItemDefines.PartType.wheel)
            || (partTank && p.partType == ItemDefines.PartType.tank)
            || (partEngine && p.partType == ItemDefines.PartType.engine)
            || (partGadget && p.partType == ItemDefines.PartType.gadget)
            || (partNitro && p.partType == ItemDefines.PartType.nitro)
            || (partDecal && p.partType == ItemDefines.PartType.decal)
            || (partExpansion && p.partType == ItemDefines.PartType.expansion);
    }
    public void ResetFilters()
    {
        rarityCommom = commonToggle == null ? true : commonToggle.isOn;
        rarityRare = rareToggle == null ? true : rareToggle.isOn;
        rarityEpic = epicToggle == null ? true : epicToggle.isOn;
        rarityLegendary = legendaryToggle == null ? true : legendaryToggle.isOn;

        partWheel = wheelToggle == null ? true : wheelToggle.isOn;
        partTank = tankToggle == null ? true : tankToggle.isOn;
        partEngine = engineToggle == null ? true : engineToggle.isOn;
        partGadget = gadgetToggle == null ? true : gadgetToggle.isOn;
        partNitro = nitroToggle == null ? true : nitroToggle.isOn;
        partDecal = decalToggle == null ? true : decalToggle.isOn;
        partExpansion = expansionToggle == null ? true : expansionToggle.isOn;

        FilterParts();
    }

    protected override bool PopulateList()
    {
        foreach (var item in filtered)
        {
            var gObject = PoolEmptyContainer();
            if (gObject.TryGetComponent(out GlossaryButton gloBtn))
            {
                gObject.SetActive(true);
                gloBtn.glossaryParent = parent;
                gloBtn.AssignItem(item);
                buttons.Add(gloBtn);
            }
        }
        Sort(sortListByName);
        return true;
    }
    public static Action<List<GameObject>> sortListByName = (list) =>
    {
        list?.Sort((a, b) =>
        {
            if (a.TryGetComponent(out GlossaryButton tA) && b.TryGetComponent(out GlossaryButton tB))
            {
                return tA.title.text.CompareTo(tB.title.text);
            }
            else return 0;
        });
    };
}
