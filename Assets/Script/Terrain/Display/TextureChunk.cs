using System;
using UnityEngine;

public class TextureChunk
{
    public PlixelDisplayManager pdm;
    public int x, y, size;
    public bool bg;
    [NonSerialized] public Plixel[] plixels;
    [NonSerialized] public Color32[] pixels;
    bool dirty = false;

    public TextureChunk(PlixelDisplayManager p, int x, int y, int size, Plixel[] z)
    {
        pdm = p;
        this.x = x;
        this.y = y;
        this.size = size;

        plixels = z;
        pixels = new Color32[size * size];

        Revise();
    }
    public void Revise()
    {
        for (int i = 0; i < plixels.Length; i++)
        {
            pixels[i] = plixels[i].getColor(bg);
        }
        ApplyToTexture();
    }
    void ApplyToTexture()
    {
        //  pdm.SetPixels(x * size, y * size, Mathf.Min((x +1)* size, pdm.outputTexture.width) - x*size, Mathf.Min((y + 1) * size, pdm.outputTexture.height) - y * size, pixels);
        //    pdm.SetPixels(x * size, y * size, size, size, pixels);
    }
}
