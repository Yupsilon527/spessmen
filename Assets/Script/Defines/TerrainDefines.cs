using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainDefines
{
    public static GameObject TerrainPrefab = Resources.Load<GameObject>("Prefabs/Terrain/Land Prefab");

    public static float[] CameraBounds = new float[] { 5, 5, 3, 1 };
    public enum Element
    {
        nothing = 0,
        dirt = 1,
        rock = 2,
        gold = 3,
        fertilizer = 4,
        core = 5,
        total = 6,
    }
    public static Color[] ElementColors = new Color[]
    {
        Color.clear,
        new Color(.6f,.45f,.33f),
        new Color(.5f,.5f,.5f),
        new Color(1f,.85f,0),
        new Color(.8f,.8f,.8f),
        new Color(1f,0,0),
    };
    public enum Behavior
    {
        Empty = 0,
        Foreground = 1,
        Background = 2,
        Frozen = 4,
        Indestructable = 8,
    }
    public static int MatterInChunk = 100;
    public static int MatterLossChance = 33;

    public static int terrain_ram_size = 1000;
    public static int terrain_chunk_size = 30;

    public static float terrain_dirty_ratio = .5f;
    public static float terrain_PPU = 24;
    public static float terrain_mass_multiplier = .10f;
    public static float terrain_zlayer = -3;

}
