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
    }
    public void RegenerateShopItems(int total)
    {
        if (ResourceCache.main == null) return;

      //  int amt = total - itemActions.Sum(b => (b.playerLocked && !b.wasPurchased) ? 1 : 0);
       // itemActions = itemActions.Where(i => i.playerLocked && !i.wasPurchased).ToList();

        List<PartScriptable> playerparts = new();
        playerparts.AddRange(DataItemPlayer.main.car.parts.Select(p => p.scriptable));
        var playerPartsArray = playerparts.ToArray();
        playerparts.Clear();
        foreach (var part in playerPartsArray)
        {
            foreach (var c in part.combos)
            {
                playerparts.Add(c.other);
            }
        }
        int level = TourneyController.main.GetCurrentRaceIndex();

        List<WeightPart> valid = new();
        foreach (var item in ResourceCache.main.parts.Where((PartScriptable item) => item.IsUnlocked()))
        {
            if (item.boonRarity >= ItemDefines.BoonRarity.rare && level == 0)
            {
                continue;
            }
            else if (item.boonRarity >= ItemDefines.BoonRarity.epic && level < RaceDefines.SeasonRaces)
            {
                continue;
            }
            valid.Add(new WeightPart(item, (playerparts.Contains(item) ? (3 * ((int)item.boonRarity+1)) : 2)));

        }

        PurchaseData.AccountLuck(valid);


        for (int i = 0; i < total; i++)
        {
            var newItem = new PurchaseData(valid);
            if (i < itemActions.Count)
            {
                if (!itemActions[i].playerLocked || itemActions[i].wasPurchased)
                    itemActions[i] = newItem;
            }
            else
            {
                itemActions.Add(newItem);
            }
            player.score.GiveLuck(newItem.part.boonRarity);
        }
    }
}
