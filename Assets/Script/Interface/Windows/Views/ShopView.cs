using System.Linq;
using TMPro;

public partial class ShopView : ViewBase
{
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

            if (!button.IsLocked())
            {
                button.gameObject.SetActive(ii < items.Length && items[ii] != null);
                button.AssignItem(DataItemPlayer.main, items[ii]);
                ii++;
            }

            iB++;
        }
    }
    public void ResetStore(bool hardReset,bool gameStart)
    {
        if (DataItemPlayer.main?.car == null) return;
        if (hardReset)
        {
            DataItemPlayer.main.shop.numRerolls = 0;
            if (gameStart)
            {
                dragdrop.Clear();
                foreach (var b in itemButtonSelection)
                {
                    b.ToggleLocked(false);
                }
            }
        }
        else
        {
            DataItemPlayer.main.shop.numRerolls++;
            DataItemPlayer.main.score.GiveChaos(ItemDefines.chaosPerShopReset);
        }
        if (ResourceCache.main != null)
        {
            DataItemPlayer.main.shop.RegenerateShop(itemButtonSelection.Sum(b => b.IsLocked() ? 0 : 1));
            PresentMultipleItems(DataItemPlayer.main.shop.itemActions.ToArray());
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
            ResetStore(false,false);
        }
    }
    float GetResetCost()
    {
        if (DataItemPlayer.main.shop.numRerolls < DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.shop_resets))
            return 0;
        return (1 + (TourneyController.main.GetCurrentRaceIndex())) * (DataItemPlayer.main.shop.numRerolls + 1) * .6f;
    }
    public override void OnOpened()
    {
        base.OnOpened();
        if (DataItemPlayer.main != null)
        {
            InitializeShop(false);
        }
    }
    public void InitializeShop(bool newGame)
    {
        if (DataItemPlayer.main == null) return;

        playership.AssignShip(DataItemPlayer.main.car);
        ResetStore(true,newGame);
        dragdrop.InitSlots(DataItemPlayer.main.car);
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
