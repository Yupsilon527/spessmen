using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmingSpotController : MonoBehaviour
{

    private void Start()
    {
        Nutriment = new Resource(null, 100, name + " nutriment", false, true);
        PlantGrowth = new Resource(null, 100, name + " growth", false, true);
        PlantHealth = new Resource(null, 100, name + " health", false, true);

        ChangePlantSprite();
    }
    private void Update()
    {
        HandlePlantGrowth();
        UpdateInterface();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out PlayerFarmingComponent player))
        {
            player.FarmingSpot = this;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out PlayerFarmingComponent player))
        {
            if (player.FarmingSpot == this)
            {
                player.FarmingSpot = null;
            }
        }
    }
    #region Nutriment
    public Resource Nutriment;

    public bool FeedItem(ItemMob item)
    {
        if (Nutriment.GetValue()< Nutriment.GetLimit(false) &&  item.GetNutritionalValue()>0)
        {
            ChunkItem chunk = (ChunkItem)item;
            if (chunk != null)
            {
                float remaining = Nutriment.GetLimit(false) - Nutriment.GetValue();
                if (remaining> chunk.GetNutritionalValue())
                {
                    Nutriment.GiveValue(chunk.GetNutritionalValue());
                    chunk.Kill();
                }
                else
                {
                    float percentage = 1f - remaining / chunk.GetNutritionalValue();
                    chunk.SetQuantity(Mathf.CeilToInt(percentage * chunk.Quantity));

                    Nutriment.SetValue(Nutriment.GetLimit(false));
                }
            }
            else
            {
                Nutriment.GiveValue(item.GetNutritionalValue());
                chunk.Kill();
            }
            return true;
        }
        return false;
    }
    #endregion
    #region Plants
    PlantData currentPlant;
    public Resource PlantGrowth;
    public Resource PlantHealth;
    public SpriteRenderer PlantSprite;
    public float UpdateInterval = .5f;
    float NextPlantUpdateTime;
    public PlantData GetCurrentPlant()
    {
        return currentPlant;
    }
    public bool TryPlant(PlantData plant)
    {
        if (currentPlant != null && PlantHealth.GetValue() >  0)
            return false;

        GiveNewPlant( plant);
        return true;
    }
    void GiveNewPlant(PlantData plant)
    {
        currentPlant = plant;
        PlantGrowth.SetLimit(currentPlant.PlantGrowthTime, Resource.LimitRule.empty_value);
        PlantHealth.SetLimit(currentPlant.PlantHealth, Resource.LimitRule.fullheal_value);
        NextPlantUpdateTime = Time.time + 1f;
        ChangePlantSprite();
    }
    void DestroyPlant()
    {
        currentPlant = null;
        ChangePlantSprite();
    }
    public enum PlantPhase
    {
        seedling,
        middle,
        adult,
        dry,
        gone
    }
    public PlantPhase GetPlantPhase()
    {
        if (currentPlant==null)
        {
            return PlantPhase.gone;
        }
        if (PlantHealth.GetValue() <= 0)
            return PlantPhase.dry;
        else if (PlantGrowth.GetPercentage() <= .15f)
            return PlantPhase.seedling;
        else if (PlantGrowth.GetPercentage() < 1)
            return PlantPhase.middle;
        return PlantPhase.adult;
    }
    void ChangePlantSprite()
    {
        if (currentPlant==null)
        {
            PlantSprite.enabled = false;
            return;
        }
        PlantPhase pPhase = GetPlantPhase();
        PlantSprite.enabled = pPhase != PlantPhase.gone;
        switch (pPhase)
        {
            case PlantPhase.seedling:
                PlantSprite.sprite = currentPlant.SpriteSeedling ;
                break;
            case PlantPhase.middle:
                PlantSprite.sprite = currentPlant.SpriteMiddle;
                break;
            case PlantPhase.adult:
                PlantSprite.sprite = currentPlant.SpriteAdult;
                break;
            case PlantPhase.dry:
                PlantSprite.sprite = currentPlant.SpriteDry;
                break;
        }
    }
    void HandlePlantGrowth()
    {
        if (currentPlant!=null && NextPlantUpdateTime < Time.time)
        {
            PlantCycle();
            if (PlantHealth.GetValue() > 0)
                NextPlantUpdateTime = Time.time + UpdateInterval;
            else
                NextPlantUpdateTime = Time.time + 10;
        }
    }
    void PlantCycle()
    {
        if (PlantHealth.GetValue() > 0)
        {
            if (PlantGrowth.GetPercentage() < 1)
            {
                if (Nutriment.GetValue() > 0)
                {
                    Nutriment.SubstractValue(currentPlant.PlantHunger * UpdateInterval);
                    PlantHealth.GiveValue(20f);
                    PlantGrowth.GiveValue(5f);
                    ChangePlantSprite();
                    if (PlantGrowth.GetPercentage() == 1)
                    {
                        PlantHealth.SetLimit(currentPlant.PlantAdultCycles * 10, Resource.LimitRule.fullheal_value);
                    }
                }
                else
                {
                    PlantHealth.SubstractValue(10f);
                    ChangePlantSprite();
                }
            }
            else
            {
                if (Nutriment.GetValue() > 0)
                {
                    Nutriment.SubstractValue(currentPlant.PlantHunger * UpdateInterval);
                    ProduceFruit();
                }
                DropRandomFruit();
                PlantHealth.SubstractValue(10f);
                if (PlantHealth.GetPercentage() == 0)
                    ProduceSeeds();
                ChangePlantSprite();
            }
            if (currentPlant.OxygenProduction > 0)
                AtmosphereController.oxygen.GiveValue(currentPlant.OxygenProduction * PlantGrowth.GetPercentage());
        }
        else
        {
            DropAllFruits();
            DestroyPlant();
        }
    }
    public float PlantHeight = 2;
    public float PlantRadius = 1;
    List<ItemMob> Fruits = new List<ItemMob>();
void     ProduceFruit()
    {
        if (currentPlant.FruitPrefab != null && Random.value * 100 < 5)
        {
            GameObject seed = GameObject.Instantiate(currentPlant.FruitPrefab);
            seed.transform.position = transform.position + transform.up * PlantHeight + new Vector3(Random.Range(-PlantRadius, PlantRadius), Random.Range(-PlantRadius, PlantRadius) * .5f, 0);
            if (seed.TryGetComponent(out ItemMob fruit))
            {
                fruit.SetSuspended(true);
                Fruits.Add(fruit);
            }
        }
    }
    void DropRandomFruit()
    {
        Fruits.RemoveAll((ItemMob fruit) =>
        {
            return fruit.container != null && fruit.IsSuspended();
        });
        if (Fruits.Count > 0 && Random.value * 100 < 15)
        {
            int iF = Mathf.RoundToInt(Random.Range(0, Fruits.Count - 1));
            Fruits[iF].SetSuspended(false);
            Fruits.RemoveAt(iF);
        }
    }
    void DropAllFruits()
    {
        foreach (ItemMob item in Fruits)
            item.SetSuspended(false);
        Fruits.Clear();
    }
    void ProduceSeeds()
    {
        if (currentPlant.SeedPrefab != null)
        {
            for (float I = Random.value * currentPlant.SeedCount; I>=1 ; I--)
            {
                GameObject seed = GameObject.Instantiate(currentPlant.SeedPrefab);
                seed.transform.position = transform.position + transform.up * PlantHeight + new Vector3(Random.Range(-PlantRadius, PlantRadius), Random.Range(-PlantRadius, PlantRadius) * .5f, 0);
               
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position + transform.up * PlantHeight, Vector3.one * PlantRadius);
    }
    #endregion
    #region Interface
    public Color NutrimentDisplayColor;
    public Color HealthDisplayColor;
    public SpriteRenderer NutriDisplay;
    public SpriteRenderer HealthDisplay;
    void UpdateInterface()
    {
        if (NutriDisplay != null)
        {
                NutrimentDisplayColor.a = Nutriment.GetPercentage();
            NutriDisplay.color = NutrimentDisplayColor;
        }
        if (HealthDisplay != null)
        {
            if (currentPlant == null)
                HealthDisplayColor.a = 0;
            else
                HealthDisplayColor.a = PlantHealth.GetPercentage();
            HealthDisplay.color = HealthDisplayColor;
        }
    }
    #endregion
}