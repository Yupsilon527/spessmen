using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmingSpotController : MonoBehaviour
{

    private void Start()
    {
        Nutriment = gameObject.AddComponent<PropertyController>();
        Nutriment.ResetLimit(100);

        PlantGrowth = gameObject.AddComponent<PropertyController>();
        PlantHealth = gameObject.AddComponent<PropertyController>();
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
    public PropertyController Nutriment;

    public bool FeedItem(ItemMob item)
    {
        if (Nutriment.GetValue()< Nutriment.GetLimit() &&  item.GetNutritionalValue()>0)
        {
            ChunkItem chunk = (ChunkItem)item;
            if (chunk != null)
            {
                float remaining = Nutriment.GetLimit() - Nutriment.GetValue();
                if (remaining> chunk.GetNutritionalValue())
                {
                    Nutriment.GiveValue(chunk.GetNutritionalValue());
                    chunk.Kill();
                }
                else
                {
                    float percentage = 1f - remaining / chunk.GetNutritionalValue();
                    chunk.SetQuantity(Mathf.CeilToInt(percentage * chunk.Quantity));

                    Nutriment.SetValue(Nutriment.GetLimit());
                }
            }
            else
            {
                Nutriment.GiveValue(item.GetNutritionalValue());
                chunk.Kill();
            }
        }
        return false;
    }
    #endregion
    #region Plants
    PlantData currentPlant;
    public PropertyController PlantGrowth;
    public PropertyController PlantHealth;
    public SpriteRenderer PlantSprite;
    public float UpdateInterval = .5f;
    float NextPlantUpdateTime;
    public PlantData GetCurrentPlant()
    {
        return currentPlant;
    }
    public bool TryPlant(PlantData plant)
    {
        if (currentPlant != null || PlantHealth.GetValue() <= 0)
            return false;

        GiveNewPlant( plant);
        return true;
    }
    void GiveNewPlant(PlantData plant)
    {
        currentPlant = plant;
        PlantGrowth.SetLimit(currentPlant.PlantGrowthTime);
        PlantGrowth.SetValue(0);
        PlantHealth.ResetLimit(currentPlant.PlantHealth);
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
        if (PlantGrowth.GetPercentage() <= .1f)
            return PlantPhase.seedling;
        if (PlantGrowth.GetPercentage() < 1)
            return PlantPhase.adult;
        return PlantPhase.middle;
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
                if (Nutriment.GetValue()>0)
                {
                    Nutriment.SubstractValue(currentPlant.PlantHunger * UpdateInterval);
                    PlantHealth.GiveValue(20f);
                    PlantGrowth.GiveValue(10f);
                    ChangePlantSprite();
                    if (PlantGrowth.GetPercentage() == 1)
                    {
                        PlantHealth.ResetLimit(currentPlant.PlantAdultCycles * 10);
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
                    PlantProduce();
                }
                    PlantHealth.SubstractValue(10f);
            }
            if (currentPlant.OxygenProduction>0)
                AtmosphereController.oxygen.GiveValue(currentPlant.OxygenProduction);
        }
        else
        {
            DestroyPlant();
        }
    }
void     PlantProduce()
    {

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
            if (currentPlant == null)
                NutrimentDisplayColor.a = 0;
            else
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