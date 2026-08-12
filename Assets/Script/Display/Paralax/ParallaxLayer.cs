using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public WeightSprite[] spriteVariants;
    public SpriteRenderer spriteTemplate;
    public float startingSpeed = 1f;
    public float speed = 1f;
    public float desiredWidth = 1f;
    public float spacing = 0f;
    public float deltaPosition = 0f;

    private HashSet<SpriteRenderer> activeRenderers = new HashSet<SpriteRenderer>();
    private HashSet<SpriteRenderer> disabledRenderers = new HashSet<SpriteRenderer>();

    private float step;
    private int requiredCopies;

    void Awake()
    {
        // The template itself counts as the first active renderer.
        activeRenderers.Add(spriteTemplate);
    }

    public void ChangeEnvironment(WeightSprite[] svars)
    {
        spriteVariants = svars;

        step = GetSpriteWidth(spriteTemplate, spriteVariants[0]) + spacing;
        requiredCopies = Mathf.CeilToInt(desiredWidth / step) + 1;

        ResizePool();
        DrawFresh();
    }

    void ResizePool()
    {
        // Grow: pull from disabled pool first, only instantiate if pool is empty.
        while (activeRenderers.Count < requiredCopies)
        {
            SpriteRenderer renderer = disabledRenderers.FirstOrDefault();
            if (renderer != null)
            {
                disabledRenderers.Remove(renderer);
                renderer.gameObject.SetActive(true);
            }
            else
            {
                renderer = Instantiate(spriteTemplate, transform);
            }
            activeRenderers.Add(renderer);
        }

        // Shrink: park excess renderers in the disabled pool instead of destroying them.
        while (activeRenderers.Count > requiredCopies)
        {
            SpriteRenderer renderer = activeRenderers.First();
            activeRenderers.Remove(renderer);
            renderer.gameObject.SetActive(false);
            disabledRenderers.Add(renderer);
        }

        // Re-layout evenly now that membership is settled.
        int index = 0;
        foreach (var renderer in activeRenderers)
        {
            renderer.transform.localPosition = Vector3.left * step * (requiredCopies / 2f - index);
            index++;
        }
    }

    public void DrawFresh()
    {
        foreach (var renderer in activeRenderers)
        {
            CycleSprite( renderer);
        }
        ScrollRaw(deltaPosition);
    }

    public void Scroll(float worldDelta)
    {
        ScrollRaw(worldDelta * speed);
    }

    public void ScrollRaw(float delta)
    {
        foreach (var renderer in activeRenderers)
        {
            renderer.transform.localPosition += Vector3.left * delta;
            if (renderer.transform.localPosition.x < -step * requiredCopies / 2f)
            {
                CycleSprite(renderer);
            }
        }
    }

   protected virtual void CycleSprite(SpriteRenderer renderer)
    {
        renderer.sprite = WeightList.PickWeight(spriteVariants)?.value ?? null;
        while (renderer.transform.localPosition.x < -step * requiredCopies / 2f)
        {
            renderer.transform.localPosition += Vector3.right * (requiredCopies - 1) * step;
        }
    }

    float GetSpriteWidth(SpriteRenderer renderer, WeightSprite spritelib)
    {
        return spritelib.value.rect.width / spritelib.value.pixelsPerUnit * renderer.transform.localScale.x;
    }
}