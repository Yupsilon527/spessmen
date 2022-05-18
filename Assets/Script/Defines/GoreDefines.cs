using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GoreDefines
{
    public static GameObject GorePrefab = Resources.Load<GameObject>("Prefabs/Gibs/Chunk");
    public static GameObject BloodPrefab = Resources.Load<GameObject>("Prefabs/Gibs/Blood");
    public static GameObject RagdollPrefab = Resources.Load<GameObject>("Prefabs/Critters/CritterRagdoll");

    public static Color bloodColor = new Color(125, 0, 0);
    public static float bloodStainRange = .05f;
    public static float gorezlevel = -6;
    public static float damage_to_blood = .2f;
    public static float bleed_interval = .2f;
    public static int initial_pool_size = 50;
    public static int limb_chunk_count = 3;
}
