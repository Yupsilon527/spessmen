using UnityEngine;
using UnityEngine.U2D;

public class ParallaxLayer : MonoBehaviour
{
    public Sprite[] spriteVariants;
    public SpriteRenderer spriteTemplate;
    public float speed = 1f;
    public float desiredWidth = 1f;
    public float spacing = 0f;
    int extraCopies = 2;
    private SpriteRenderer[] copies;
    private float step;
    void Awake()
    {
        step = GetSpriteWidth(spriteTemplate) + spacing;
        extraCopies = Mathf.CeilToInt(desiredWidth / step);
        copies = new SpriteRenderer[extraCopies];
        copies[0] = spriteTemplate;
        for (int i = 1; i < copies.Length; i++)
        {
            SpriteRenderer copy = Instantiate(spriteTemplate, transform);
            copy.transform.localPosition = Vector3.left * step * (copies.Length /2 -i);
            copies[i] = copy;
        }
    }
    public void ChangeEnvironment(Sprite[] svars )
    {
        spriteVariants = svars;
        DrawFresh();
    }
    public void DrawFresh()
    {
        foreach (var sprite in copies)
        {
            sprite.sprite = spriteVariants[Mathf.FloorToInt(Random.value * spriteVariants.Length)]; 
        }
    }
    public void Scroll(float worldDelta)
    {
        float delta = worldDelta * speed;
        for (int i = 0; i < copies.Length; i++)
        {
            copies[i].transform.localPosition += Vector3.left * delta;
            if (copies[i].transform.localPosition.x < -step * extraCopies / 2)
            {
                CycleSprite(copies[i]);
            }
        }
    }
    void CycleSprite(SpriteRenderer renderer)
    {
        renderer.sprite = spriteVariants[Mathf.FloorToInt(Random.value * spriteVariants.Length)];
        while (renderer.transform.localPosition.x < -step * extraCopies / 2)
        {
            renderer.transform.localPosition += Vector3.right * extraCopies * step;
        }
    }
    float GetSpriteWidth(SpriteRenderer renderer)
    {
        return renderer.sprite.rect.width / renderer.sprite.pixelsPerUnit * renderer.transform.localScale.x;
    }
}