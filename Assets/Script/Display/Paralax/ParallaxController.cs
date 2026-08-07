using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    public ParallaxLayer[] layers;
    public float scrollSpeed = 5f;

    public void SetWorldDelta(float worldDelta)
    {
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].Scroll(worldDelta);
        }
    }

    public void SetScrollSpeed(float value)
    {
        scrollSpeed = value;
    }
}