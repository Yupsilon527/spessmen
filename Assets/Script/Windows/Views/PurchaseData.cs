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
        List<PartScriptable> valid = new();
        foreach (var item in ResourceCache.main.parts.Where((PartScriptable item) => item.IsUnlocked()))
            valid.Add(item);

        //TODO AccountLuck(valid);

        var pickedItem = (valid.Count > 0) ? valid[Mathf.FloorToInt(valid.Count * UnityEngine.Random.value)] : null;
        if (pickedItem != null)
        {
            part = pickedItem;
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
        itemCost = part.boonValue * price;
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
