using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UI;

public partial class ShopView : ViewBase
{
    public TextMeshProUGUI playerGold;
    public TextMeshProUGUI resetCost;

    public PlayerShipGrid playership;
    public PartTooltip tooltip;
    public RaceTooltip raceTooltip;
    public Button undoButton;
    public AbilityDragDropInterface dragdrop;
    public ItemPurchaseButton[] itemButtonSelection;

    public void PresentMultipleItems(PurchaseData[] items)
    {
        for (int i =0; i<items.Length; i++)
        {
            var button = itemButtonSelection[i];
            button?.dropSlot?.attachedToken?.GoToStash();

            button.gameObject.SetActive(i < items.Length && items[i] != null);
            button.AssignItem(DataItemPlayer.main, items[i]);
        }
    }
    public void ResetStore(bool hardReset, bool gameStart)
    {
        if (DataItemPlayer.main?.car == null) return;
        if (gameStart)
        {
            dragdrop.Clear();
            foreach (var b in itemButtonSelection)
            {
                b.ToggleLocked(false);
            }
        }
        DataItemPlayer.main.shop.ResetShop(hardReset);
        if (ResourceCache.main != null)
        {
            ShowShop();
        }
    }
    public void ShowShop()
    {
        PresentMultipleItems(DataItemPlayer.main.shop.itemActions.ToArray());
        Refresh();
    }
    public override void Refresh()
    {
        if (playerGold != null && DataItemPlayer.main != null)
        {
            string goldLabel = LanguageController.main.Translate("UI Table", "Gold Label");
            playerGold.text = goldLabel.Replace("%gold%", DataItemPlayer.main?.econ?.gold?.GetValue().ToString("F0") ?? "");
        }

        if (resetCost != null)
        {
            string costLabel = LanguageController.main.Translate("UI Table", "Cost Label");
            resetCost.text = costLabel.Replace("%cost%", GetResetCost().ToString("F0"));
        }

        for (int i = 0; i < itemButtonSelection.Length; i++)
        {
            itemButtonSelection[i].UpdateEnableState(DataItemPlayer.main, true);
        }

        undoButton?.gameObject?.SetActive(DataItemPlayer.main.car.CanUndoExpansion());
    }
    public void ResetButtonFunction()
    {
        if (DataItemPlayer.main.econ.gold.ChargeValue(GetResetCost()))
        {
            ResetStore(false, false);
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
        if (newGame)
        {
            ResetStore(true, true);
        }
        else
        {
            ShowShop();
        }
        dragdrop.InitSlots(DataItemPlayer.main.car);
        raceTooltip?.ShowCurrentRace();

        DataItemPlayer.main?.econ?.gold?.OnValueChanged.RemoveListener(Refresh);
        DataItemPlayer.main?.econ?.gold?.OnValueChanged.AddListener(Refresh);
    }
    void Conclude()
    {
        dragdrop.ApplyChanges();
        dragdrop.Clear();
        DataItemPlayer.main.car.ClearUndo();
    }
    public void Proceed()
    {
        if (DataItemPlayer.main.car.ValidateAll())
        {
            Conclude();
            TourneyController.main.ChangePhase(TourneyController.TourneyPhase.setup);
            ViewManager.Instance.ChangeView(ViewManager.Views.raceView);
        }
    }
    public void HandleUndo()
    {
        if (DataItemPlayer.main.car.CanUndoExpansion())
        {
            var lastExp = DataItemPlayer.main.car.lastAppliedExpansion;
            DataItemPlayer.main.car.UndoLastExpansion();

            var undoToken = dragdrop.GenerateToken(lastExp);
            undoToken.GoToStash();
            DataItemPlayer.main.car.stash.Add(lastExp);

            Refresh();
            playership.UpdateGrid();
            DataItemPlayer.main.car.ValidateAll();
        }
    }
}
