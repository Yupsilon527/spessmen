using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemPurchaseButton : MonoBehaviour, IPointerEnterHandler
{
    public ShopView shop;
    public DragDropSlot dropSlot;
    public Button button;
    public PurchaseData purchaseData;
    public Image icon;
    public Outline outline;
    public TextMeshProUGUI priceLabel;
    public Image coinImage;
    public Toggle lockToggle;
    bool locked = false;

    public virtual void Clear()
    {
        if (priceLabel != null) priceLabel.text = "";
        if (coinImage != null) coinImage.gameObject.SetActive(false);
        if (outline != null) outline.effectColor = Color.clear;
        purchaseData = null;
        button.interactable = false;
        icon.enabled = false;
        SetLocked(false);
    }
    public void AssignItem(DataItemPlayer purchasingPlayaer, PurchaseData purchase)
    {
        if (purchase == null)
        {
            Clear();
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
    public PartScriptable itemAction;
    public virtual void ShowItemAction(DataItemPlayer player, PartScriptable newAction)
    {
        if (itemAction != newAction)
        {
            Clear();
            itemAction = newAction;
        }
        if (icon != null)
        {
            icon.sprite = newAction.icon;
            icon.enabled = true;
        }
        if (outline != null) outline.effectColor = ItemDefines.GetColorForRarity(newAction.boonRarity);
    }
    void UpdatePrice()
    {
        if (purchaseData != null)
        {
            string goldCost = purchaseData.itemCost == 0 ? LanguageController.main.Translate("UI Table", "Cost Free") : LanguageController.main.Translate("UI Table", "Cost Label");
            if (coinImage != null) coinImage.gameObject.SetActive(purchaseData.itemCost > 0);
            if (priceLabel != null) priceLabel.text = goldCost.Replace("%cost%", purchaseData.itemCost.ToString("F0")); ;
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
            
            Clear();
        }
    }
    bool SanityCheck()
    {
        return  purchaseData != null && purchaseData.CanBePurchased(DataItemPlayer.main);
    }
    public bool IsLocked()
    {
        return locked && SanityCheck();
    }
    public void ToggleLocked(bool value)
    {
        locked = value;
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
