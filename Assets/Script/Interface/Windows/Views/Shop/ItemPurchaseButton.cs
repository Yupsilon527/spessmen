using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemPurchaseButton : PartButtonBase, IPointerEnterHandler
{
    public ShopView shop;
    public Button button;
    public DragDropSlot dropSlot;

    public PartScriptable partSO;
    public PurchaseData purchaseData;

    public TextMeshProUGUI priceLabel;
    public Toggle lockToggle;

    public override void Clear(bool draw)
    {
        if (priceLabel != null) priceLabel.text = "";
        purchaseData = null;
        partSO = null;
        button.interactable = false;
        SetLocked(false);
        base.Clear(draw);
    }
    public void AssignItem(DataItemPlayer purchasingPlayaer, PurchaseData purchase)
    {
        if (purchase == null)
        {
            Clear(true);
        }
        else
        {
            var customer = purchasingPlayaer;
            ShowItemAction(customer, purchase.part);
            purchaseData = purchase;
            UpdatePrice();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    PurchaseItem(customer);
                });
            }
        }
        UpdateEnableState(purchasingPlayaer, false);
    }
    protected override void Redraw()
    {
        outlineMask.gameObject?.SetActive(partSO != null);
        sprite.gameObject?.SetActive(partSO != null);
        if (partSO != null)
        DrawScriptable(partSO);
    }
    public virtual void ShowItemAction(DataItemPlayer player, PartScriptable newAction)
    {
        if (partSO != newAction)
        {
            Clear(false);
            partSO = newAction;
            DrawScriptable(partSO);
        }
        Redraw();
    }
    void UpdatePrice()
    {
        if (purchaseData != null)
        {
            string goldCost = purchaseData.purchaseCost == 0 ? LanguageController.main.Translate("UI Table", "Cost Free") : LanguageController.main.Translate("UI Table", "Cost Label");
            if (priceLabel != null) priceLabel.text = goldCost.Replace("%cost%",Mathf.Ceil( purchaseData.purchaseCost).ToString("F0")); ;
        }
        else
        {
            Debug.LogWarning("Warning! Purchasedata is null!");
        }
    }
    public void UpdateEnableState(DataItemPlayer purchasingPlayaer, bool recalcAction)
    {
        if (purchaseData == null)
        {
            SetLocked(false);
            button.interactable = false;
        }
        else
        {
            if (recalcAction)
            {
                AssignItem(purchasingPlayaer, purchaseData);
            }
            UpdatePrice();
            button.interactable = purchaseData != null && purchaseData.CanBePurchased(purchasingPlayaer);
        }
    }
    public void PurchaseItem() {
      PurchaseItem(DataItemPlayer.main);
    }
    public void PurchaseItem(DataItemPlayer purchasingPlayaer)
    {
        if (purchaseData.MakePurchase(purchasingPlayaer))
        {
           var newToken = shop.dragdrop.GenerateToken(purchaseData);
            newToken.AttachToSlot(dropSlot,true);
            
            Clear(true);
        }
    }
    bool SanityCheck()
    {
        return  purchaseData != null && purchaseData.CanBePurchased(DataItemPlayer.main);
    }
    public bool IsLocked()
    {
        return purchaseData?.playerLocked ?? false && SanityCheck();
    }
    public void ToggleLocked(bool value)
    {
        if (purchaseData!=null)
        purchaseData.playerLocked = value;
        if (value && !SanityCheck())
        {
            SetLocked(false);
        }
    }
    public void SetLocked(bool value)
    {
        ToggleLocked(value);
        lockToggle?.SetIsOnWithoutNotify(value);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (purchaseData!= null && shop != null && shop.tooltip!=null)
        {
            shop.tooltip.ShowPart(purchaseData.part,true);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (shop!=null && shop.tooltip!=null)
        {
            shop.tooltip.Clear();
        }
    }
}
