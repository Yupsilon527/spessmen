using System.Collections.Generic;
using TMPro;
using UnityEngine;
/*
public class ItemShopWindow : Window
{
    int numResets = 0;
    public class PurchaseData
    {
        public float itemCost;
        public float discount;
        public ItemDefines.ItemAction itemAction;
        bool wasPurchased = false;

        public PurchaseData(ItemDefines.ItemAction action, float price = 1)
        {
            itemAction = action;
            SetDiscount(price);
        }

        public PurchaseData(BoonScriptable pickItem) : this(new ItemDefines.ItemAction(pickItem))
        {
        }

        public void SetDiscount(float price)
        {
            discount = price;
            itemCost = itemAction.GetCost() * price;
        }
        public bool CanPurchase(Player purchasingPlayer)
        {
            return !wasPurchased && itemAction.CanBePurchased(purchasingPlayer);
        }
        public bool MakePurchase(Player purchasingPlayaer)
        {
            if (!wasPurchased && itemAction != null && purchasingPlayaer.score.playerGold.GetValue() >= itemCost)
            {
                purchasingPlayaer.score.playerGold.SubstractedValue(itemCost);
                itemAction.Resolve(purchasingPlayaer);
                wasPurchased = true;
                return true;
            }
            return false;
        }
    }
    public TextMeshProUGUI playerGold;
    public TextMeshProUGUI resetCost;
    [Header("Multiple Items Element")]
    public ItemPurchaseButton[] itemButtonSelection;

    public void PresentMultipleItems(PurchaseData[] items)
    {
        int iB = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (iB < itemButtonSelection.Length)
            {
                itemButtonSelection[iB].gameObject.SetActive(items[i]?.itemAction?.action != ItemDefines.ActionType.Bounty);
                itemButtonSelection[iB++].AssignItem(Player.current, items[i]);
            }
        }
    }
    public void ResetStore(bool hardReset)
    {
        ItemDefines.ActionType[] actions = new ItemDefines.ActionType[0];
        if (hardReset)
        {
            actions = new ItemDefines.ActionType[]
            {
                             ItemDefines.ActionType.GenericItem,
                             ItemDefines.ActionType.GenericItem,
                             ItemDefines.ActionType.GenericItem,
                             ItemDefines.ActionType.GenericItem,
            };
            numResets = 0;
        }
        else
        {
            actions = new ItemDefines.ActionType[]
            {
                             ItemDefines.ActionType.GenericItem,
                             ItemDefines.ActionType.GenericItem,
                             ItemDefines.ActionType.GenericItem,
                             ItemDefines.ActionType.GenericItem,
            };
            numResets++;
            Player.current.score.GiveChaos(ItemDefines.chaosPerShopReset);
        }
        List<PurchaseData> itemActions = new();
        List<BoonScriptable> boons = new();

        for (int i = 0; i < itemButtonSelection.Length; i++)
        {
            var ia = new ItemDefines.ItemAction(Player.current, actions[i], boons);
            itemActions.Add(new(ia));
            boons.Add(ia.nItem);
        }
        PresentMultipleItems(itemActions.ToArray());
        UpdateState();
    }
    public void UpdateState()
    {
        if (playerGold != null && Player.current != null)
            playerGold.text = "Your gold: " + Player.current.score.playerGold.GetValueRounded(1);
        if (resetCost != null)
            resetCost.text = "Reset Cost " + GetResetCost();

        for (int i = 0; i < itemButtonSelection.Length; i++)
        {
            itemButtonSelection[i].UpdateEnableState(Player.current, true);
        }
    }
    public void ResetButtonFunction()
    {
        if (Player.current.score.playerGold.ChargeValue(GetResetCost()))
        {
            ResetStore(false);
        }
    }
    protected override void OnClosed()
    {
        base.OnClosed();
        WaveSpawner.main?.SetupNewWave();
    }
    float GetResetCost()
    {
        if (numResets < Player.current.GetPropertyAdditive(ModifierDefines.Property.shop_resets))
            return 0;
        return (1 + WaveSpawner.main.currentWave) * (numResets + 1);
    }
}
*/