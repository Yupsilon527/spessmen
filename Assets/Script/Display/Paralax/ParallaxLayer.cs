using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public SpriteRenderer spriteTemplate;
    public float speed = 1f;
    public float desiredWidth = 1f;
    int extraCopies = 2;

    private SpriteRenderer[] copies;
    private float width;

    void Awake()
    {
        width = GetSpriteWidth(spriteTemplate);
        extraCopies = Mathf.CeilToInt(desiredWidth / width);
        copies = new SpriteRenderer[extraCopies ];
        copies[0] = spriteTemplate;

        for (int i = 1; i < copies.Length; i++)
        {
            SpriteRenderer copy = Instantiate(spriteTemplate, transform);
            copy.transform.localPosition = Vector3.right * width * i;
            copies[i] = copy;
        }
    }

    public void Scroll(float worldDelta)
    {
        float delta = worldDelta * speed;

        for (int i = 0; i < copies.Length; i++)
        {
            float width = GetSpriteWidth(copies[i]);
            copies[i].transform.localPosition += Vector3.right * delta;
            if (copies[i].transform.localPosition.x> width * extraCopies/2)
            {
                CycleSprite(copies[i]);
            }
        }

    }

    void CycleSprite(SpriteRenderer renderer)
    {
        float width = GetSpriteWidth(renderer);
        while (renderer.transform.localPosition.x > width * extraCopies / 2)
        {
            renderer.transform.localPosition += Vector3.left * extraCopies * width ;
        }
    }
    float GetSpriteWidth(SpriteRenderer renderer)
    {
        return renderer.sprite.rect.width / renderer.sprite.pixelsPerUnit * renderer.transform.localScale.x;
    }
}