using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    public SpriteRenderer background;
    public ParallaxLayer groundLayer;
    public ParallaxLayer frontLayer;
    public ParallaxLayer backLayer;
    public ParallaxLayer farLayer;

    public void FromEnvironment(EnvironmentScriptable env)
    {
        background.sprite = env.background;
        groundLayer.ChangeEnvironment( env.groundSprites);
        frontLayer.ChangeEnvironment( env.frontSprites);
        backLayer.ChangeEnvironment( env.backSprites);
        farLayer.ChangeEnvironment( env.farSprites);
    }

    public ParallaxLayer[] layers;
    public float scrollSpeed = 5f;

    public void SetWorldDelta(float worldDelta)
    {
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].Scroll(worldDelta );
        }
    }

    public void SetScrollSpeed(float value)
    {
        scrollSpeed = value;
    }
}