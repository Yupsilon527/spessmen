using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PurchaseData
{
    public PartScriptable part;
    public float itemCost;
    public float discount;
    bool wasPurchased = false;

    public PurchaseData()
    {
        List<WeightPart> valid = new();
        foreach (var item in ResourceCache.main.parts.Where((PartScriptable item) => item.IsUnlocked()))
            valid.Add(new WeightPart(item, 10));

        AccountLuck(valid);

        var pickedItem = WeightList.PickWeight(valid);
        if (pickedItem != null)
        {
            part = pickedItem.part;
        }
    }
    void AccountLuck(List<WeightPart> valid)
    {
        float chaosCoefficient = 1;
        float luckCoefficient = 1;

        float luck = DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.luck_bonus);
        float chaos = DataItemPlayer.main.score.playerChaos;

        if (chaos > 0)
            chaosCoefficient = (chaos + ItemDefines.chaosPlus) / ItemDefines.chaosPlus;
        else
            chaosCoefficient = ItemDefines.chaosMinus / (chaos + ItemDefines.chaosMinus);
        if (luck > 0)
            luckCoefficient = ItemDefines.luckPlus / (luck + ItemDefines.luckPlus);
        else
            luckCoefficient = (Random.value + Random.value * ItemDefines.luckPlus / (ItemDefines.luckPlus + luck)) * -.5f;

        foreach (var item in valid)
        {
            item.Weight = ItemDefines.baseSpawnWeight * luckCoefficient + (int)item.part.boonRarity * ItemDefines.raritySpawnWeight + item.Weight * chaosCoefficient;
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
        itemCost = part.GetBasePrice() * price;
    }
    public bool CanBePurchased(DataItemPlayer purchasingPlayer)
    {
        return !wasPurchased;
    }
    public bool MakePurchase(DataItemPlayer purchasingPlayaer)
    {
        if (!wasPurchased && part != null && purchasingPlayaer.econ.gold.GetValue() >= itemCost)
        {
            purchasingPlayaer.econ.gold.SubstractedValue(itemCost);
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
}