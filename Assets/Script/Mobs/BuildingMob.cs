using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMob : Mob
{
    public BoxCollider2D ConstructionCollider;
    public SpriteRenderer BuildingTexture;

    public bool CanBeBuildThere(Vector2 center)
    {
        foreach (RaycastHit2D collision in Physics2D.BoxCastAll(center+ ConstructionCollider.offset, ConstructionCollider.size,0,Vector2.zero,0, LayerMask.GetMask(new string[] { "Foreground" }) ))
        {
            if (collision.transform.tag == "Building")
            {
                Debug.Log("Building " + collision.transform.name + " In The Way");
                return false;
            }
            if (collision.transform.tag == "Terrain")
            {
                Debug.Log("Terrain In The Way");
                return false;
            }
        }
        return true;
    }
    public void BuildCopy(Vector2 center,float percent)
    {
            GameObject bui = GameObject.Instantiate(gameObject);
            bui.transform.position = center;
        if (bui.TryGetComponent(out BuildingMob building))
        {
            building.SetBuildPercentage(percent);
        }
    }
    float buildPercentage = 100f;
    public void IncreaseBuildPercentage(float percentage)
    {
        SetBuildPercentage(buildPercentage + percentage);
    }
    public virtual void SetBuildPercentage(float percent)
    {
        if (BuildingTexture!=null)
        {
            BuildingTexture.size = new Vector2(1, percent);
        }
    }
}
