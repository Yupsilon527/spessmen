using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PurchaseData
{
    public PartScriptable part;
    public float purchaseCost;
    public float discount;
    public bool playerLocked = false;
    public bool wasPurchased = false;

    public PurchaseData(List<WeightPart> valid)
    {

        var pickedItem = WeightList.PickWeight(valid);
        if (pickedItem != null)
        {
            part = pickedItem.part;
        }

        float price = DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.shop_prices);
        if (part.abilities.Any(a => a.function == ShipDefines.PartEvent.OnActivated))
            price *= DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.active_prices);
        switch (part.partType)
        {
            case ItemDefines.PartType.engine:
                price *= DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.engine_prices);
                break;
            case ItemDefines.PartType.gadget:
                price *= DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.gadget_prices);
                break;
            case ItemDefines.PartType.nitro:
                price *= DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.nitro_prices);
                break;
            case ItemDefines.PartType.wheel:
                price *= DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.wheel_prices);
                break;
            case ItemDefines.PartType.decal:
                price *= DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.trinket_prices);
                break;
            case ItemDefines.PartType.expansion:
                price *= DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.expansion_prices);
                break;
        }
        SetDiscount(price);
    }
    public static void AccountLuck(List<WeightPart> valid)
    {
        float level = TourneyController.main.GetCurrentRaceIndex() + 1;
        float chaosCoefficient = ItemDefines.ChaosNumber(DataItemPlayer.main.score.playerChaos);
        float luckCoefficient = ItemDefines.LuckNumber(DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.luck_bonus));

        foreach (var item in valid)
        {
            if (item.part.boonRarity > ItemDefines.BoonRarity.common)
                item.Weight *= Mathf.Clamp( ( level * ItemDefines.raritySpawnWeight / Mathf.Pow(10, (int)item.part.boonRarity)) * luckCoefficient * chaosCoefficient * item.part.weightMultiplier, 0 ,  ItemDefines.raritySpawnWeight*2);
            else
                item.Weight *= ItemDefines.commonSpawnWeight + ItemDefines.raritySpawnWeight;

            if (DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.item_rarity) != 1)
                item.Weight *= Mathf.Pow(DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.luck_bonus), (int)item.part.boonRarity);

            if (item.part.abilities.Any(a => a.function == ShipDefines.PartEvent.OnActivated))
                item.Weight *= DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.active_weight);

            switch (item.part.partType)
            {
                case ItemDefines.PartType.engine:
                    item.Weight *= DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.engine_weight);
                    break;
                case ItemDefines.PartType.gadget:
                    item.Weight *= DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.gadget_weight);
                    break;
                case ItemDefines.PartType.nitro:
                    item.Weight *= DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.nitro_weight);
                    break;
                case ItemDefines.PartType.wheel:
                    item.Weight *= DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.wheel_weight);
                    break;
                case ItemDefines.PartType.decal:
                    item.Weight *= DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.trinket_weight);
                    break;
                case ItemDefines.PartType.expansion:
                    item.Weight *= DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.expansion_rarity);
                    break;
            }

        }
    }

    public PurchaseData(PartScriptable action, float price = 1)
    {
        part = action;
        SetDiscount(price);
    }


    public void SetDiscount(float price)
    {
        discount = price;
        purchaseCost = part.GetBasePrice() * discount;
    }
    public bool CanBePurchased(DataItemPlayer purchasingPlayer)
    {
        return !wasPurchased && part != null;
    }
    public bool MakePurchase(DataItemPlayer purchasingPlayaer)
    {
        if (CanBePurchased(purchasingPlayaer) && purchasingPlayaer.econ.gold.GetValue() >= purchaseCost)
        {
            purchasingPlayaer.econ.gold.SubstractedValue(purchaseCost);
            wasPurchased = true;
            return true;
        }
        return false;
    }
}

public class WeightPart : WeightList.WeightItem<PartScriptable>
{
    public PartScriptable part;

    public WeightPart(PartScriptable value, float weight) : base(value, weight)
    {
        part = value;
    }
    public override string ToString()
    {
        return part.InternalName + " " + Weight;
    }
}