
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;

public partial class ShopView : ViewBase
{
    int numResets = 0;

    public TextMeshProUGUI playerGold;
    public TextMeshProUGUI resetCost;

    public PlayerShipGrid playership;
    public PartTooltip tooltip;
    public RaceTooltip raceTooltip;
    public AbilityDragDropInterface dragdrop;
    public ItemPurchaseButton[] itemButtonSelection;

    public void PresentMultipleItems(PurchaseData[] items)
    {
        int iB = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (iB < itemButtonSelection.Length)
            {
                if (itemButtonSelection[iB].IsLocked())
                {
                    iB++;
                    continue;
                }
                else
                {
                    itemButtonSelection[iB].gameObject.SetActive(items[i] != null);
                    itemButtonSelection[iB++].AssignItem(DataItemPlayer.main, items[i]);
                }
            }
        }
    }
    public void ResetStore(bool hardReset)
    {
        if (hardReset)
        {
            numResets = 0;
        }
        else
        {
            numResets++;
            DataItemPlayer.main.score.GiveChaos(ItemDefines.chaosPerShopReset);
        }
        List<PurchaseData> itemActions = new();
        if (ResourceCache.main != null)
        {
            List<PartScriptable> playerparts = new();
            playerparts.AddRange(DataItemPlayer.main.ship.parts.Select(p => p.scriptable));
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
                valid.Add(new WeightPart(item, playerparts.Contains(item) ? 2 : 1));

            PurchaseData.AccountLuck(valid);

            for (int i = 0; i < itemButtonSelection.Sum(b => b.IsLocked() ? 0 : 1); i++)
            {
                var ia = new PurchaseData(valid);
                itemActions.Add(ia);
            }
            PresentMultipleItems(itemActions.ToArray());
            UpdateState();
        }
    }
    public void UpdateState()
    {
        if (playerGold != null && DataItemPlayer.main != null)
            playerGold.text = "Your gold: " + DataItemPlayer.main.econ.gold.GetValue();
        if (resetCost != null)
            resetCost.text = "Cost " + GetResetCost();

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
        if (numResets < DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.shop_resets))
            return 0;
        return (1 + (TourneyController.main.GetCurrentRaceIndex())) * (numResets + 1);
    }
    public override void OnOpened()
    {
        base.OnOpened();
        if (DataItemPlayer.main != null)
        {
            InitializeShop();
            ResetStore(true);
            raceTooltip?.ShowCurrentRace();

            DataItemPlayer.main.econ.gold.OnValueChanged.RemoveListener(UpdateState);
            DataItemPlayer.main.econ.gold.OnValueChanged.AddListener(UpdateState);
        }
    }
    public void InitializeShop()
    {
        if (DataItemPlayer.main == null) return;
        playership.AssignShip(DataItemPlayer.main.ship);
        dragdrop.InitSlots(DataItemPlayer.main.ship);
    }
    void Conclude()
    {
        dragdrop.ApplyChanges();
        dragdrop.Clear();
    }
    public void Proceed()
    {
        if (DataItemPlayer.main.ship.ValidateAll())
        {
            Conclude();
            TourneyController.main.ChangePhase(TourneyController.TourneyPhase.racing);
            ViewManager.Instance.ChangeView(ViewManager.Views.raceView);
        }
    }
}
