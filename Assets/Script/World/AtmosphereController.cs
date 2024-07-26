using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphereController : MonoBehaviour
{
    public static Resource oxygen;

    public float TotalValue = 100;
    public SpriteRenderer circleSprite;

    private void Awake()
    {
        oxygen = new Resource(null, TotalValue, "Atmosphere",false,false );
    }
    private void Update()
    {
        float atmoScale = oxygen.GetPercentage();
        if (circleSprite!=null)
        circleSprite.transform.localScale = Vector3.one * (atmoScale < 1 ? atmoScale : (1 + (atmoScale - 1) * .01f)) ;
    }
}
