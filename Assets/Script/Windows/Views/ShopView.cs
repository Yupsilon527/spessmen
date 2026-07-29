
using System.Collections.Generic;
using TMPro;

public partial class ShopView : ViewBase
{
    int numResets = 0;
    
    public TextMeshProUGUI playerGold;
    public TextMeshProUGUI resetCost;

    public PlayerShipGrid playership;
    public AbilityDragDropInterface dragdrop;
    public ItemPurchaseButton[] itemButtonSelection;

    public void PresentMultipleItems(PurchaseData[] items)
    {
        int iB = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (iB < itemButtonSelection.Length)
            {
                itemButtonSelection[iB].gameObject.SetActive(items[i]!=null);
                itemButtonSelection[iB++].AssignItem(DataItemPlayer.main, items[i]);
            }
        }
    }
    public void ResetStore(bool hardReset)
    {
        if (hardReset)
        {
            playership.AssignShip(DataItemPlayer.main.ship);
            dragdrop.InitSlots(DataItemPlayer.main.ship);
            numResets = 0;
        }
        else
        {
            numResets++;
            DataItemPlayer.main.score.GiveChaos(ItemDefines.chaosPerShopReset);
        }
        List<PurchaseData> itemActions = new();

        for (int i = 0; i < itemButtonSelection.Length; i++)
        {
            var ia = new PurchaseData();
            itemActions.Add(ia);
        }
        PresentMultipleItems(itemActions.ToArray());
        UpdateState();
    }
    public void UpdateState()
    {
        if (playerGold != null && DataItemPlayer.main != null)
            playerGold.text = "Your gold: " + DataItemPlayer.main.econ.gold.GetValue();
        if (resetCost != null)
            resetCost.text = "Reset Cost " + GetResetCost();

        for (int i = 0; i < itemButtonSelection.Length; i++)
        {
            itemButtonSelection[i].UpdateEnableState(DataItemPlayer.main, true);
        }
    }
    public void ResetButtonFunction()
    {
        if (DataItemPlayer.main.econ.gold.ChargeValue(GetResetCost()))
        {
            ResetStore(false);
        }
    }
    float GetResetCost()
    {
        //TODO if (numResets < DataItemPlayer.main.GetPropertyAdditive(ModifierDefines.Property.shop_resets))
            return 0;
        //return (1 + WaveSpawner.main.currentWave) * (numResets + 1);
    }
    public override void OnOpened()
    {
        base.OnOpened();
        InitializeShop();
    }
    public void InitializeShop()
    {
        if (DataItemPlayer.main == null) return;
        foreach (var part in DataItemPlayer.main.ship.parts)
        {
            var token = dragdrop.GenerateToken(part);
            token.AttachToSlot(dragdrop.buildSlot,true);
        }
    }
    void Conclude()
    {
        dragdrop.ApplyChanges();
        dragdrop.Clear();
    }
    public void BeginRace()
    {
        if (DataItemPlayer.main.ship.ValidateAll())
        {
            Conclude();
            TourneyController.main.ChangePhase(TourneyController.TourneyPhase.racing);
            ViewManager.Instance.ChangeView(ViewManager.Views.raceView);
        }
    }
}
