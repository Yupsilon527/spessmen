using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public void FilterParts()
    {
        filtered = ResourceCache.main.parts.Where(p => 
        p.IsUnlocked() 
      && MatchesRarity(p)
     && MatchesPartType(p)
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
        rarityCommom = rarityRare = rarityEpic = rarityLegendary = true;
        partWheel = partTank = partEngine = partGadget = partNitro = partDecal = partExpansion = true;

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
