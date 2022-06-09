using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopComponent : MonoBehaviour
{
    public InventoryComponent inventory;
    private void Awake()
    {
        if (inventory == null)
            inventory = GetComponent<InventoryComponent>();
    }
    [System.Serializable]
    public class ShopEntry
    {
        public GameObject Item;
        public float Cost;
    }
    public ShopEntry[] Shop = new ShopEntry[0];
    public bool CanPlayerBuyItem(Player buyer, int item)
    {
        return CanPlayerBuyItem(buyer, Shop[item]);
    }
    public bool CanPlayerBuyItem(Player buyer, ShopEntry item)
    {
        return buyer.resources.GetResource(ResourceController.Resources.gold) >= item.Cost;
    }
    public void BuyItemForPlayer(Player buyer, int item)
    {
        BuyItemForPlayer(buyer, Shop[item]);
    }
    public void BuyItemForPlayer(Player buyer, ShopEntry item)
    {
        if (buyer.resources.ChargeValue(ResourceController.Resources.gold, item.Cost))
        {
            GameObject realObject = Instantiate(item.Item);
            realObject.transform.position = transform.position;

            if (realObject.TryGetComponent(out ItemMob itemComp))
            {
                itemComp.GoldValue = item.Cost;
                if (!buyer.backpack.LoadItem(itemComp))
                {
                    inventory.LoadItem(itemComp);
                }
            }

            if (realObject.TryGetComponent(out PowerupComponent PowerUp) && PowerUp.OnBuy(buyer))
            {
                realObject.SetActive(false);
            }
            }
    }
}
