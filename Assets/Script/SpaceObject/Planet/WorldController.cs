using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class WorldController : Initializable
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

    public List<PlanetoidController> planets = new List<PlanetoidController>();
    public List<PlixelMapMob> terrainmobs = new List<PlixelMapMob>();

    public ObjectPool MobPool;
    public ObjectPool EffectPool;

    protected override void Initialize()
    {
        base.Initialize();
        active = this;
    }
    private void Start() {
        StartCoroutine(PrepareWorld());
    }

    public IEnumerator PrepareWorld()
    {
        Debug.Log("[entityWorld] Draw the world.");
        ChangePhase(GamePhase.Loading);//prepating landscape

        yield return new WaitForSecondsRealtime(1);
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
            Time.timeScale = 0f;

        bool keepwait = true;
        while (keepwait)
        {
            keepwait = false;
            foreach (PlixelMapMob chunk in GetActiveTerrainMobs())
            {
                if (!chunk.isComplete())
                {
                    yield return new WaitForEndOfFrame();
                    keepwait = true;
                }
            }
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

    public PlanetoidController GetClosestPlanetToPoint(Vector3 center)
    {
        if (planets.Count == 0) return null;
        float dist = Mathf.Infinity;
        PlanetoidController closest = planets[0];
        foreach (var p in planets)
        {
            if ((p.transform.position - center).sqrMagnitude < dist)
            {
                closest = p;
                dist = (p.transform.position - center).sqrMagnitude ;
            }
        }
        return closest;
    }
}