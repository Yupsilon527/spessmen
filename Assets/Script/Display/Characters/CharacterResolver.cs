using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class CharacterResolver : MonoBehaviour
{
    public enum ToonType
    {
        complete,
        character,
        shadows,
    }
    public ToonType toonType = ToonType.complete;
    public void ChangeDisplayType(ToonType ntype)
    {
        toonType = ntype;
        ToggleRenderers();
    }


    public CharacterSheet activeCharacter;
    public bool alwaysClear = true;
    public bool canScale = false;
    public Dictionary<string, Renderer> SpriteRenderers;
    public SpriteLibrary library;
    protected Renderer[] renderers;
    public bool autoUpdateCenter = false;
    public Transform centerBone;

    public float scale;

    private void Awake()
    {
        InitSpriteRenderers();
        InitAttachPoints();
        InitHead();
    }
    public void Recenter()
    {
        if (autoUpdateCenter && centerBone != null)
        {
            transform.localPosition = new(0, (transform.position.y - centerBone.transform.position.y) * 1.6f, 0);
        }
    }
    void ToggleRenderer(Renderer renderer, bool enabled)
    {
        renderer.enabled = enabled;
        if (renderer.TryGetComponent(out SpriteSkin skin)) skin.enabled = enabled;
    }
    void InitSpriteRenderers()
    {
        var parent = library == null ? transform : library.transform;

        renderers = parent.GetComponentsInChildren<Renderer>(true).Where(r => r is SpriteRenderer || r is SpriteMask).ToArray();
        SpriteRenderers = new Dictionary<string, Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (!SpriteRenderers.ContainsKey(renderer.name.ToLower()))
            {
                SpriteRenderers.Add(renderer.name.ToLower(), renderer);
            }
        }
    }
    public virtual void LoadCharacter(CharacterSheet character, bool additive = false, float scaleOverride = 1)
    {
        if (!additive && character != null)
        {
            if (canScale)
                scale = character.scale * scaleOverride;
            else
                scale = scaleOverride;

            if (alwaysClear)
                ClearSprites();
            activeCharacter = character;
        }
        if (character != null)
        {
            if (character.spriteLibrary != null && library != null)
            {
                library.spriteLibraryAsset = character.spriteLibrary;
            }
            else
            {
                foreach (var replaced in character.SpriteReplacements)
                {
                    if (replaced.multiple)
                    {
                        SetSpriteMultiple(replaced.bodyPartName, replaced.sprite);
                        SetSpriteMultiple(replaced.bodyPartName + " Shadow", replaced.sprite);
                    }
                    else
                    {
                        SetSprite(replaced.bodyPartName, replaced.sprite);
                        SetSprite(replaced.bodyPartName + " Shadow", replaced.sprite);
                    }
                }
            }
            ToggleRenderers();
        }
    }
    public void ChangeMaterial(Material body)
    {
        ChangeMaterial(body, body, null);
    }
    public void PreChangeCharacter()
    {
        foreach (Renderer renderer in renderers)
        {
            ToggleRenderer(renderer, true);
        }
    }
    public void PostChangeCharacter()
    {
        foreach (Renderer renderer in renderers)
        {
            ToggleRenderer(renderer,
                renderer.material != null &&
                ((renderer is SpriteRenderer sr && sr.sprite != null) ||
                (renderer is SpriteMask sm && sm.sprite != null))
                 );
        }
    }
    public void ChangeMaterial(Material body, Material eyes, Material shadow, bool settingsOverride = false)
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = true;
            if (renderer.CompareTag("Eyes"))
            {
                if (toonType == ToonType.shadows)
                    renderer.enabled = false;
                else
                    renderer.material = eyes;
            }
            else if (renderer.CompareTag("Shadow"))
            {
                if (ShadowsEnabled)
                    renderer.material = shadow;
                else
                    renderer.enabled = false;
            }
            else
            {
                if (toonType == ToonType.shadows)
                    renderer.enabled = false;
                else
                    renderer.material = body;
            }

            ToggleRenderer(renderer,
                renderer.enabled &&
                ((renderer is SpriteRenderer sr && sr.sprite != null) ||
                (renderer is SpriteMask sm && sm.sprite != null))
                 );
        }
    }
    bool ShadowsEnabled {
        get => toonType != ToonType.character;
    }
    public void ToggleRenderers()
    {
        foreach (Renderer renderer in renderers)
        {
            if (renderer.CompareTag("Shadow"))
            {
                ToggleRenderer(renderer, renderer.enabled && ShadowsEnabled);
            }
            else
            {
                ToggleRenderer(renderer, renderer.enabled && toonType != ToonType.shadows);
            }
        }
    }
    public void SetSprite(string rendererName, Sprite sprite)
    {
        var renderer = FindSpriteRenderer(rendererName);
        if (renderer == null) return;
        bool enableState = (renderer.CompareTag("Shadow") && ShadowsEnabled) || toonType != ToonType.shadows;

        if (renderer is SpriteRenderer spriteRenderer)
        {
            ToggleRenderer(spriteRenderer, enableState);
            spriteRenderer.sprite = sprite;
        }
        else if (renderer is SpriteMask spriteMask)
        {
            ToggleRenderer(spriteMask, enableState);
            spriteMask.sprite = sprite;
        }
    }
    public Renderer FindSpriteRenderer(string searchName)
    {
        if (SpriteRenderers.ContainsKey(searchName.ToLower()))
        {
            return SpriteRenderers[searchName.ToLower()];
        }
        return null;
    }
    public void ClearSprite(string rendererName)
    {
        foreach (Renderer renderer in renderers)
        {
            if (renderer.name == rendererName)
            {
                if (renderer is SpriteMask sm)
                    sm.sprite = null;
                if (renderer is SpriteRenderer sr)
                    sr.sprite = null;
            }
        }
    }
    public void ClearSprites()
    {
        foreach (Renderer renderer in renderers)
        {
            if (renderer is SpriteMask sm)
                sm.sprite = null;
            if (renderer is SpriteRenderer sr)
                sr.sprite = null;
        }
    }
    public void SetSpriteMultiple(string rendererName, Sprite sprite)
    {
        foreach (Renderer renderer in renderers)
        {
            bool enableState = ((ShadowsEnabled && renderer.CompareTag("Shadow")) || toonType != ToonType.shadows);
            if (renderer.name.Length >= rendererName.Length && renderer.name.Substring(0, rendererName.Length).ToLower() == rendererName.ToLower())
            {
                ToggleRenderer(renderer, enableState);
                if (renderer is SpriteMask sm)
                    sm.sprite = sprite;
                if (renderer is SpriteRenderer sr)
                    sr.sprite = sprite;
            }
        }
    }
    public void ChangeMaskInteraction(SpriteMaskInteraction nInt)
    {
        foreach (Renderer renderer in renderers)
        {
            if (renderer is SpriteRenderer sr)
                sr.maskInteraction = nInt;
        }
    }
    public void ChangeLayer(string layer)
    {
        foreach (var r in renderers)
            r.gameObject.layer = LayerMask.NameToLayer(layer);
    }
    public void ChangeSortingLayer(string layer)
    {
        Debug.Log($"Change {name} layer to {layer}");
        if (SortingLayer.IsValid(SortingLayer.NameToID(layer)) && gameObject.activeInHierarchy)
            foreach (Renderer renderer in renderers)
                renderer.sortingLayerID = SortingLayer.NameToID(layer);
    }
    #region AttachPoints
    public Dictionary<string, Transform> Attachpoints;

    void InitAttachPoints()
    {
        Attachpoints = new Dictionary<string, Transform>();
        var parent = library!= null ? library.transform : transform;
        foreach (HeroAttachPoint atp in parent.GetComponentsInChildren<HeroAttachPoint>())
        {
            if (!Attachpoints.ContainsKey(atp.AttachPointName))
            {
                Attachpoints.Add(atp.AttachPointName, atp.transform);
            }
        }
    }
    public Transform FindAttachPoint(string searchName, string fallback = "origin")
    {
        if (FindAttachPoint(searchName, out Transform atp) || FindAttachPoint(fallback, out atp) || FindAttachPoint("origin", out atp))
            return atp;
        return transform;
    }
    public bool FindAttachPoint(string searchName, out Transform atp)
    {
        atp = null;
        if (Attachpoints.ContainsKey(searchName))
        {
            atp = Attachpoints[searchName];
            return true;
        }
        return false;
    }
    #endregion
    Vector3 headScale = Vector3.one;
    void InitHead()
    {
        if (FindAttachPoint("head") is Transform head)
        {
            headScale = head.localScale;
        }
    }
    public void SetHeadScale(float value)
    {
        if (FindAttachPoint("head",out Transform head))
        {
            head.transform.localScale = headScale * value;
        }
    }
}
