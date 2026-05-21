using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class WorldController : MonoBehaviour
{
    public enum GamePhase
    {
        Loading = 0,
        GameRunning = 1,
        GamePaused = 2,
    }
    public GamePhase upcommingPhase = GamePhase.Loading;
    public GamePhase currentPhase = GamePhase.Loading;
    float phaseTime;

    public static WorldController active;
    public Vector2 worldSize;

    public List<Mob> MobsInMotion = new List<Mob>();
    public List<PlixelMapMob> terrainmobs = new List<PlixelMapMob>();

    public Texture2D mapTexture;
    public SpriteRenderer renderComp;

    public ObjectPool MobPool;
    public ObjectPool EffectPool;

    private void Start() {
        active = this;
        StartCoroutine(PrepareWorld());
    }
    private void OnValidate()
    {
        if (renderComp != null)
            renderComp.sprite = Sprite.Create(mapTexture, new Rect(0, 0, mapTexture.width, mapTexture.height), Vector2.one / 2, TerrainDefines.terrain_PPU);
    }

    public IEnumerator PrepareWorld()
    {
        if (renderComp != null)
            renderComp.enabled = false;
            yield return new WaitForEndOfFrame();
        Debug.Log("[entityWorld] Draw the world.");
        PlixelMapMob tileset;
        ChangePhase(GamePhase.Loading);//prepating landscape
        tileset = PlixelMapMob.LoadFromTexture(mapTexture);

        yield return new WaitUntil(() => { return tileset.isComplete(); });
        yield return PauseForTerrainToLoad();
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
            case GamePhase.GamePaused:
                Time.timeScale = 0;
                break;
            case GamePhase.GameRunning:
                Time.timeScale = 1;
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
                    Zim.HandleShockwave(center, inner_radius,shockwave_radius, knockback_force, 0);
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

    public IEnumerator PauseForTerrainToLoad()
    {
        if (currentPhase != GamePhase.Loading)
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

        if (currentPhase != GamePhase.Loading)
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