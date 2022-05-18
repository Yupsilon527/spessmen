using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class WorldController : MonoBehaviour
{
    public enum GamePhase
    {
        Loading = 0,
        GameRunning = 1,
        GamePaused = 2,
    }
    public GamePhase upcommingPhase = GamePhase.Loading;
    public float phaseTime = 0f;
    public GamePhase currentPhase = GamePhase.Loading;

    public static WorldController active;
    public int nEntitites = 0;
    public Vector2 worldSize;
    public float waterLevel;
    public float bottomLevel;
    public bool[] boundlock = new bool [3];

    public List<Mob> MobsInMotion = new List<Mob>();
    public List<PlixelMapMob> terrainmobs = new List<PlixelMapMob>();

    Texture2D mapTexture = null;
    Texture2D mskTexture = null;

    public string MapName = "test_mask";
    public class GameData
    {
        public string mapName = "test-med";
    }

    private void Start() {
        active = this;

        mapTexture = GameResource.active.LoadTexture(GameDirectory.MapFolder, MapName);
        mskTexture = GameResource.active.LoadTexture(GameDirectory.MapFolder, MapName + "_mask");
        StartCoroutine(PrepareWorld());
    }

    public List<Vector2> DefaultStartLocations(List<Vector2> startLocs, Vector2 worldSize, int desired_locations)
    {
        startLocs.Sort((Vector2 vA, Vector2 vB) =>
        {
            return vA.x.CompareTo(vB.x);
        });

        float averageSqrDistance = 0;
        for (int iVector = 1; iVector < startLocs.Count; iVector++)
        {
            averageSqrDistance += ((startLocs[iVector] - startLocs[iVector - 1]).sqrMagnitude);
        }
        averageSqrDistance /= startLocs.Count * desired_locations;

        List<Vector2> validStartLocs = new List<Vector2>();
        validStartLocs.Add(GetPointClosest(startLocs, Vector2.zero));

        for (int I = 1; I< desired_locations; I++)
        {
            validStartLocs.Add(GetNextClosest(startLocs, validStartLocs, averageSqrDistance));
        }

        return validStartLocs;
    }

    public List<Vector2> ForceDefaultStartLocations(int nPlayers, Vector2 worldSize)
    {
        Vector2 PlayerSize = MobDefines.defaultPlayerSize/2f ;

        Rect[] PlayerRects = new Rect[]
        {
            new Rect(
                PlayerSize.x,
                PlayerSize.y,
                worldSize.x/4f-PlayerSize.x,
                worldSize.y/4f-PlayerSize.y),
            new Rect(
                worldSize.x*3/4f-PlayerSize.x,
                PlayerSize.y,
                worldSize.x/4f-PlayerSize.x,
                worldSize.y/4f-PlayerSize.y),
            new Rect(
                PlayerSize.x,
                worldSize.y*3/4f,
                worldSize.x/4f-PlayerSize.x,
                worldSize.y/4f-PlayerSize.y-1),
            new Rect(
                worldSize.x*3/4f,
                worldSize.y*3/4f,
                worldSize.x/4f-PlayerSize.x,
                worldSize.y/4f-PlayerSize.y-1)
        };

        List<Vector2> validStartLocs = new List<Vector2>();

        foreach (Rect rectbound in PlayerRects)
        {
            validStartLocs.Add(
                (new Vector2(
                    Random.Range(rectbound.xMin, rectbound.xMax) ,
                    -Random.Range(rectbound.yMin, rectbound.yMax) 

                ) + new Vector2(-worldSize.x / 2 , worldSize.y / 2 ))
                );
        }
        while (validStartLocs.Count > nPlayers)
        { validStartLocs.RemoveAt(Random.Range(0, validStartLocs.Count - 1)); }

        return validStartLocs;
    }

    public Vector2 GetPointClosest(List<Vector2> startLocs, Vector2 targetpoint)
    {
        Vector2 closest = targetpoint;

        foreach (Vector2 loc in startLocs)
        {
            if (closest == targetpoint || (loc - targetpoint).sqrMagnitude < (closest - targetpoint).sqrMagnitude)
            {
                closest = loc;
            }
        }
        return closest;
    }

    public Vector2 GetNextClosest(List<Vector2> startLocs, List<Vector2> checkpoints,float averagedistancesqrd)
    {
        float mindistance = Mathf.Infinity ;
        Vector2 closest = startLocs[0];

        foreach (Vector2 loc in startLocs)
        {
            if (checkpoints.Contains(loc))
            {
                continue;
            }
            float locdistance = Mathf.Infinity;
            foreach (Vector2 check in checkpoints)
            {
                float dist = (loc - check).sqrMagnitude;
                if (dist < averagedistancesqrd)
                {
                    locdistance = Mathf.Infinity;
                    break; 
                }
                    if(dist < locdistance)
                {
                    locdistance = dist;
                }
            }
            if (locdistance<mindistance)
            {
                closest = loc;
                mindistance = locdistance;
            }
        }
        return closest;
    }

    public IEnumerator PrepareWorld()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("[entityWorld] Draw the world.");
        PlixelMapMob tileset;
        if (mskTexture == null) { 
            tileset = PlixelMapMob.LoadFromTexture(mapTexture);
        }
        else
        {
            tileset = PlixelMapMob.LoadFromTexture(mskTexture);
        }

        boundlock[0] = tileset.GetEBounds();
        boundlock[1] = tileset.GetWBounds();
        boundlock[2] = tileset.GetNBounds();
        /*bool hasWater = false;
       
            if (tileset.GetNBounds() || tileset.GetEBounds() || tileset.GetWBounds())
            {
                lockedBounds = true;
                SetBounds(mapTexture.width, mapTexture.height);
            }
            else if (tileset.GetSBounds())
            {
                hasWater = true;
            }*/

        //draw water

        /*if (hasWater)
        {
            //TBD
        }*/

        ChangePhase(GamePhase.Loading);//prepating landscape
        yield return new WaitUntil(() => { return tileset.isComplete(); });

        //gamePhase = -2;//prepating startlocs
        List<Vector2> startLocs = tileset.GetValidSpawnLocations(MobDefines.defaultPlayerSize);
        yield return new WaitForEndOfFrame();

        if (startLocs.Count == 0)
        {
            Debug.LogError("[entityWorld] Failed startpoints generation!");
        }
        Debug.Log("[entityWorld] " + startLocs.Count + " startpoints generated!");

        int nPlayers = 12;
        if (startLocs.Count < nPlayers)//do we have spawnpoints for each player?
        {
            Debug.LogWarning("[entityWorld] Start Locations generated BY FORCE!");
            startLocs = ForceDefaultStartLocations(nPlayers,tileset.GetWorldSize()/TerrainDefines.terrain_PPU);
            float entityRadius = MobDefines.defaultPlayerSize.magnitude / 2f;
            foreach (Vector2 point in startLocs)
            {
                foreach (PlixelMapMob Zim in GetActiveTerrainMobs())
                {
                    print("Resolve " + Zim.name);
                    Zim.HandleExplosion(point + Vector2.up * MobDefines.defaultPlayerSize.y / 2f, 0, 0, entityRadius, 999, entityRadius,1);
                }
            }
        }
        else
        {
            Debug.Log("[entityWorld] Generate Start Locations!");
            startLocs = DefaultStartLocations(startLocs, tileset.GetWorldSize() / TerrainDefines.terrain_PPU, nPlayers);
        }
        yield return new WaitForEndOfFrame();
        while (!tileset.isComplete()) { yield return new WaitForEndOfFrame(); }
        yield return new WaitForEndOfFrame();
        SetBounds(mapTexture.width, mapTexture.height);
        //complete
        Debug.Log("[entityWorld] Drawing complete!");
        ChangePhase(GamePhase.GameRunning);
    }
    public void ChangePhase(GamePhase phase,float time)
    {
        if (phaseTime == 0)
        {
            Debug.Log("[entityWorld] Upcomming phase " + phase + " in " + time + " seconds.");
            upcommingPhase = phase;
            phaseTime = Time.time + time;
        }
    }
    public void ChangePhase(GamePhase phase)
    {
        Debug.Log("[entityWorld] Change game phase to "+phase);
        phaseTime = 0;
        switch (phase)
        {
            case GamePhase.Loading:
                StartCoroutine(PauseForTerrainToLoad());
                break;
            case GamePhase.GameRunning:
                Time.timeScale = 1;
                break;
            case GamePhase.GamePaused:
                Time.timeScale = 0;
                break;
        }
        currentPhase = phase;
    }
    public void HandlePhase()
    {
        if (phaseTime>0 && phaseTime<Time.time)
        {
            ChangePhase(upcommingPhase);
        }
        Debug.Log("[entityWorld] Handle phase "+ currentPhase);
        switch (currentPhase)
        {
            case GamePhase.Loading:
            case GamePhase.GamePaused:
            case GamePhase.GameRunning:
                break;
        }
    }

    public IEnumerator MakePhysicsExplosion(ExplosionData data)
    {
        yield return MakePhysicsExplosion(data.center, data.inner_radius, data.inner_damage, data.middle_radius, data.middle_damage, data.outer_radius, data.outer_damage, data.shockwave_radius, data.knockback_force);
    }
        public IEnumerator MakePhysicsExplosion(Vector2 center, float inner_radius, int inner_damage, float middle_radius, int middle_damage, float outer_radius, int outer_damage, float shockwave_radius, float knockback_force)
    {
        Debug.Log("[entityWorld] Start Explosion at " + center + " of size " + inner_radius +":"+ middle_radius + ":"+ outer_radius);
        int explosion_radius = Mathf.CeilToInt(Mathf.Max(inner_radius, middle_radius) * TerrainDefines.terrain_PPU);

        if (inner_radius == 0 && outer_radius == 0) { yield break; }

        terrainmobs.RemoveAll((PlixelMapMob chunk) => { return chunk == null; });
        List<PlixelMapMob> current_chunks = new List<PlixelMapMob>();
        current_chunks.AddRange(terrainmobs);

        foreach (PlixelMapMob Zim in current_chunks)
        {
            print("Resolve " + Zim.name);
            Zim.HandleExplosion(center, inner_radius, inner_damage, middle_radius, middle_damage, outer_radius, outer_damage);
        }
        Debug.Log("[entityWorld] Explosion handled the chunks");
        yield return new WaitForEndOfFrame();

        PlixelMapMob[] terrchunks = GetActiveTerrainMobs();
        bool waiting = true;
        while (waiting)
        {
            waiting = false;
            foreach (PlixelMapMob Zim in terrchunks)
            {
                if (!Zim.isComplete())
                {
                    Debug.Log("Waiting for "+ Zim.gameObject.name + " to complete coroutines");
                    yield return new WaitForEndOfFrame();
                    waiting = true;
                    break;
                }
            }

        }

        if (knockback_force > 0)
        {
            Debug.Log("[entityWorld] Apply " + knockback_force + " force in a " + shockwave_radius + " aoe");
            foreach (PlixelMapMob Zim in terrchunks)
            {
                if (Zim != null)
                {
                    Zim.HandleShockwave(center, inner_radius,shockwave_radius, knockback_force);
                }
            }
        }

        Debug.Log("[entityWorld] Explosion complete!");
    }

    float lastclicktime = 1f;
    public void Update()
    {
       
        HandlePhase();
}

    public bool isGamePaused()
    {
        return false;
    }

    public void SetBounds(float W, float H)
    {
        worldSize.x = W / TerrainDefines.terrain_PPU * .5f;
        worldSize.y = H / TerrainDefines.terrain_PPU * .5f;
        GetComponent<BoxCollider2D>().size = worldSize;

        Vector2 mins = worldSize + new Vector2(boundlock[0] ? TerrainDefines.CameraBounds[0] : 0, boundlock[2] ? TerrainDefines.CameraBounds[2] : 0);
        Vector2 maxs = worldSize + new Vector2(boundlock[1] ? TerrainDefines.CameraBounds[1] : 0, TerrainDefines.CameraBounds[3]);

        SidewaysCamera.active.UpdateCameraBounds(-mins, maxs);

        waterLevel = worldSize.y;
        bottomLevel = Mathf.Max(SidewaysCamera.active.GetComponent<Camera>().orthographicSize, maxs.y);

        Debug.Log("[entityWorld] Bottomlevel set to "+ bottomLevel);
    }

    public IEnumerator PauseForTerrainToLoad()
    {
        Time.timeScale = 0f;

        bool keepwait = true;
        while (keepwait)
        {
            keepwait = false;
            foreach (PlixelMapMob chunk in GetActiveTerrainMobs())
            {
                if (!chunk.isComplete())
                {
                    keepwait = true;
                    break;
                }
            }
            yield return new WaitForEndOfFrame();
        }

        Time.timeScale = 1f;
    }

    public PlixelMapMob[] GetActiveTerrainMobs()
    {
        return terrainmobs.FindAll((PlixelMapMob mob) =>
        {
            return mob.gameObject.activeInHierarchy;
        }).ToArray();
    }
}