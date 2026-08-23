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
        ResetShop(true);
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
        int amt = total - itemActions.Sum(b => (b.playerLocked && !b.wasPurchased) ? 1 : 0);
        itemActions = itemActions.Where(i => i.playerLocked).ToList() ;

        if (ResourceCache.main != null)
        {
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

            List<WeightPart> valid = new();
            foreach (var item in ResourceCache.main.parts.Where((PartScriptable item) => item.IsUnlocked()))
            {
                if (item.boonRarity >= ItemDefines.BoonRarity.rare && TourneyController.main.GetCurrentRaceIndex() == 0)
                {
                    continue;
                }
                else if (item.boonRarity >= ItemDefines.BoonRarity.epic && TourneyController.main.GetCurrentRaceIndex() < RaceDefines.SeasonRaces)
                {
                    continue;
                }
                valid.Add(new WeightPart(item, (playerparts.Contains(item) ? (15 - (int)item.boonRarity) : 10)));

            }

            PurchaseData.AccountLuck(valid);
            for (int i = 0; i < amt; i++)
            {
                var ia = new PurchaseData(valid);
                itemActions.Add(ia);
            }
        }
    }
}
