using UnityEngine;

public class ColorParallaxLayer : ParallaxLayer
{
    public Color[] possibleColors;

    protected override void CycleSprite(SpriteRenderer renderer)
    {
        base.CycleSprite(renderer);
        if (possibleColors != null && possibleColors.Length > 0)
        {
            renderer.color = possibleColors[Mathf.FloorToInt(Random.value * possibleColors.Length)];
        }
        else renderer.color = Color.white;
    }
}
