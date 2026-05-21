using System;
using System.Collections;
using UnityEngine;

public class PlixelDisplayManager : PlixelManager
{
    public bool IsBackground = false;
    int width, height;
    public SpriteRenderer spriteRenderer;
    [NonSerialized] public Color32[] pixels, edited;
    public Texture2D outputTexture;

    public static bool displayDebug = true;
    public override void OnCreated()
    {
        Debug.Log("[entityTerrain] Initialize texture of " + name);
        InitTexture();
        pixels = new Color32[width * height];
        base.OnCreated();
    }
    private void LateUpdate()
    {
        Modify();
    }
    void InitTexture()
    {
        width = parent.GetWidth();
        height = parent.GetHeight();
        if (width > 0 && height > 0)
        {
            outputTexture = new Texture2D(width, height);
            spriteRenderer.sprite = Sprite.Create(outputTexture, new Rect(0, 0, outputTexture.width, outputTexture.height), Vector2.one / 2, TerrainDefines.terrain_PPU);
            pixels = outputTexture.GetPixels32();
        }
    }
    public void SetPixel(int x, int y, Color32 color)
    {
        int p = y * width + x;
        if (p >= 0 && p < pixels.Length)
            pixels[p] = color;
    }
    protected override IEnumerator ModifyCoroutine()
    {
        if (displayDebug)
            Debug.Log($"[entityTerrain] {name} enqueue {workRect.size} tiles to redraw..");

        for (var y = workRect.yMin; y < workRect.yMax; y++)
        {
            var o = y * width;

            for (var x = workRect.xMin; x < workRect.xMax; x++)
            {
                if (o + x < pixels.Length)
                {
                    pixels[o + x] = parent.GetTileAt(x, y)?.getColor(IsBackground) ?? Color.clear;
                    if (Step()) yield return null;
                }
            }
        }
        var i = 0;
        int total = workRect.width * workRect.height;
        if (total > 0)
        {
            edited = new Color32[total];

            for (var y = workRect.yMin; y < workRect.yMax; y++)
            {
                var o = y * width;

                for (var x = workRect.xMin; x < workRect.xMax; x++)
                {
                    edited[i++] = pixels[o + x];
                }
            }
            yield return null;
            outputTexture.SetPixels32(workRect.xMin, workRect.yMin, workRect.width, workRect.height, edited);
            outputTexture.Apply(false, false);
        }
        EndWork();
    }
}
