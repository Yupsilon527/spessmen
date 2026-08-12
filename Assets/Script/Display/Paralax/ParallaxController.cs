using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    public SpriteRenderer background;
    public ParallaxLayer groundLayer;
    public ParallaxLayer frontLayer;
    public ParallaxLayer backLayer;
    public ColorParallaxLayer farLayer;

    public void FromEnvironment(EnvironmentScriptable env)
    {
        background.sprite = env.background;
        groundLayer.ChangeEnvironment( env.groundSprites);
        frontLayer.ChangeEnvironment( env.frontSprites);
        backLayer.ChangeEnvironment( env.backSprites);
        farLayer.possibleColors = env.bgColors;
        farLayer.ChangeEnvironment( env.farSprites);
    }

    public ParallaxLayer[] layers;

    public void SetWorldDelta(float worldDelta)
    {
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].Scroll(worldDelta );
        }
    }
}