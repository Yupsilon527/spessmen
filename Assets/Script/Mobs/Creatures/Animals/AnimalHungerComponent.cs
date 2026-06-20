using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalHungerComponent : AnimalComponent, iItemToucher
{
    public ResourceFloat Hunger;
    public float HungerDecay = 5;
    public override void Awake()
    {
        base.Awake();
        Hunger = new ResourceFloat( 100, name + " hunger", false, false);
    }
    private void Update()
    {
        HandleMetabolism();
        TryEat();

        //SFX Maybe creature noise?
    }
    void HandleMetabolism()
    {
        Hunger.SubstractedValue(HungerDecay * Time.deltaTime);
    }
    [System.Serializable]
    public class DigestResult
    {
        public ItemMob.Edibility eatRequirement;
        public float hungerResult;
        public GameObject resultingItem;

        public TerrainDefines.Element chunkElement;
        public int StackCount;
    }
    public DigestResult[] Diet;

    #region Toucher

    public List<ItemMob> TouchedItems = new List<ItemMob>();
    public void OnTouchEnter(ItemMob item)
    {
        if (!TouchedItems.Contains(item))
            TouchedItems.Add(item);

    }
    public void OnTouchExit(ItemMob item)
    {
        if (!TouchedItems.Contains(item))
            TouchedItems.Remove(item);

    }
    public ItemMob GetTouchedItem()
    {
        if (TouchedItems.Count > 0)
            return TouchedItems[0];
        return null;
    }
    #endregion

    public void TryEat()
    {
        if (Hunger.GetPercentage() < 1)
        {
            foreach (ItemMob food in TouchedItems)
            {
                if (TryEatItem(food))
                {
                    return;
                }
            }
        }
    }
         bool TryEatItem(ItemMob food)
    {
            foreach (DigestResult diet in Diet)
            {
                if (diet.eatRequirement == food.ediblecategory)
                {
                PoopItem(diet);
                food.Erase();
                return true;
                }
        }
        return false;
    }
    void PoopItem(DigestResult diet)
    {
        if (diet != null)
        {
            Hunger.GiveValue(diet.hungerResult);

            if (diet.chunkElement != TerrainDefines.Element.nothing)
                ResourceHarvestController.TrySpawnChunk(diet.resultingItem, transform.position, diet.chunkElement, diet.StackCount);
            else
                Instantiate(diet.resultingItem,transform.position, transform.rotation);
        }
    }
}
