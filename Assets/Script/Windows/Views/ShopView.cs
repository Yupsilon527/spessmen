
using System.Collections.Generic;
using System.Linq;
using TMPro;

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
        int ii = 0;

        while (ii < items.Length)
        {
            if (iB >= itemButtonSelection.Length)
                return;

            var button = itemButtonSelection[iB];
            button?.dropSlot?.attachedToken?.DiscardToken();

            if (button.IsLocked())
            {
                iB++;
                continue;
            }

            button.gameObject.SetActive(ii<items.Length && items[ii] != null);
            button.AssignItem(DataItemPlayer.main, items[ii]);

            iB++;
            ii++;
        }
    }
    public void ResetStore(bool hardReset)
    {
        if (DataItemPlayer.main?.car == null) return;
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
        {
            string goldLabel =  LanguageController.main.Translate("UI Table", "Gold Label");
            playerGold.text = goldLabel.Replace("%gold%",DataItemPlayer.main.econ.gold.GetValue().ToString("F0"));
        }

        if (resetCost != null)
        {
            string costLabel =  LanguageController.main.Translate("UI Table", "Cost Label");
            resetCost.text = costLabel.Replace("%cost%", GetResetCost().ToString("F0"));
        }

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
        return (1 + (TourneyController.main.GetCurrentRaceIndex())) * (numResets + 1) * .6f;
    }
    public override void OnOpened()
    {
        base.OnOpened();
        if (DataItemPlayer.main != null)
        {
            InitializeShop();
        }
    }
    public void InitializeShop()
    {
        if (DataItemPlayer.main == null) return;
        playership.AssignShip(DataItemPlayer.main.car);
        dragdrop.InitSlots(DataItemPlayer.main.car);

        ResetStore(true);
        raceTooltip?.ShowCurrentRace();

        DataItemPlayer.main?.econ?.gold?.OnValueChanged.RemoveListener(UpdateState);
        DataItemPlayer.main?.econ?.gold?.OnValueChanged.AddListener(UpdateState);
    }
    void Conclude()
    {
        dragdrop.ApplyChanges();
        dragdrop.Clear();
    }
    public void Proceed()
    {
        if (DataItemPlayer.main.car.ValidateAll())
        {
            Conclude();
            TourneyController.main.ChangePhase( TourneyController.TourneyPhase.setup);
            ViewManager.Instance.ChangeView(ViewManager.Views.raceView);
        }
    }
}
