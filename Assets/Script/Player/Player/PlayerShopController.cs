using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerShopController : PlayerComponent
{
    public int numRerolls = 0;


    public List<PurchaseData> itemActions = new();
    public override void Setup()
    {
        itemActions = new();
    }
    public void ResetShop(bool hardReset)
    {
        if (hardReset)
        {
            numRerolls = 0;
        }
        else
        {
            numRerolls++;
            player.score.GiveChaos(ItemDefines.chaosPerShopReset);
        }
        RegenerateShopItems(8);
        foreach (var item in itemActions)
        {
            var variable = PlayerConfig.main.globalScope.GetVariable("items_encountered_" + item.scriptable.InternalName) ;
            variable.SetFloatValue(variable.GetFloatValue() + 1);
        }
    }
    public void RegenerateShopItems(int total)
    {
        if (ResourceCache.main == null) return;

        List<PartScriptable> priorityParts = new();
        priorityParts.AddRange(DataItemPlayer.main.car.parts.Select(p => p.scriptable));
        var playerPartsArray = priorityParts.ToArray();
        priorityParts.Clear();
        foreach (var part in playerPartsArray)
        {
            foreach (var c in part.combos)
            {
                priorityParts.Add(c.other);
            }
        }
        int level = TourneyController.main.GetCurrentRaceIndex();
        int fullslots = DataItemPlayer.main.car.CountTilesTotal();
        float luckCoefficient = ItemDefines.LuckNumber(DataItemPlayer.main?.GetPropertySpeculative(ModifierDefines.Property.luck_bonus) ?? 0);

        List<WeightPart> valid = new();
        foreach (var item in ResourceCache.main.parts.Where((PartScriptable item) => item.IsUnlocked()))
        {
            if (item.boonRarity >= ItemDefines.BoonRarity.rare && level == 0)
                continue;
            else if (item.boonRarity >= ItemDefines.BoonRarity.epic && level < RaceDefines.SeasonRaces)
                continue;
            if (item.partType == ItemDefines.PartType.expansion && fullslots == 100)
                continue;


            int rarityBonus = (int)item.boonRarity + 1;
            valid.Add(new WeightPart(item, priorityParts.Contains(item) ? (3 * rarityBonus * luckCoefficient) : (2 + rarityBonus * (luckCoefficient-1))));

        }

        PurchaseData.AccountLuck(valid);


        for (int i = 0; i < total; i++)
        {
            var newItem = new PurchaseData(valid);
            if (i < itemActions.Count)
            {
                if (itemActions[i].wasPurchased)
                {
                    var variable = PlayerConfig.main.globalScope.GetVariable("items_purchased_" + itemActions[i].scriptable.InternalName);
                    variable.SetFloatValue(variable.GetFloatValue() + 1);
                }
                if (!itemActions[i].playerLocked || itemActions[i].wasPurchased)
                    itemActions[i] = newItem;
            }
            else
            {
                itemActions.Add(newItem);
            }
            player.score.GiveLuck(newItem.scriptable.boonRarity);
        }
    }
}
