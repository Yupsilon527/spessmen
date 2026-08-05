using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class RacerToon
{
    public Racer racer;
    public Toon toon;

    public void PlayAnimation(string animName, int priority = 0, float fadeTime = .1f, float delay = 0, bool forced = true)
    {
        toon.PlayAnimation(animName, priority, fadeTime, delay, forced);
    }
}


public class ArenaController : MonoBehaviour
{
    [Header("Components")]
    public Camera camera;
    public SpecialEffectPool epool;
    public GameObject racerPrefab, opponentPrefab;
    public HashSet<RacerToon> racers=new();
    [Header("Toon Distance Delta")]
    public float zDelta = 5;
    public float distanceDelta = 100;
    public float distanceFarAway = 300;
    public float distanceFarAwayDelta = 30;

    public static ArenaController main;
    private void Awake()
    {
        main = this;
        if (camera == null)
            camera = GetComponentInChildren<Camera>();
        if (epool == null)
            epool = GetComponentInChildren<SpecialEffectPool>();
    }
    private void Update()
    {
        UpdateRacerPositions();
    }
    public void UpdateRacerPositions()
    {
        var playerRacer = GetPlayerRacer();
        foreach (var racer in racers)
        {
            float relativePosition = 1-racer.racer.position.distanceTraveled / Mathf.Min(1,playerRacer.racer.position.distanceTraveled);
            if (relativePosition == 0) continue;
            racer.toon.transform.position = Vector3.right * ((Mathf.Min(Mathf.Abs(relativePosition), distanceFarAway) / distanceDelta + Mathf.Max(Mathf.Abs(relativePosition) - distanceFarAway, 0) / distanceFarAwayDelta) * Mathf.Sign(relativePosition) + racer.racer.id * zDelta);
                }
    }
    public void LoadRacers(Racer[] racers)
    {
        foreach (var racer in racers)
            LoadFighterPrefab(racer, racer.id == 0 ? racerPrefab : opponentPrefab);
    }
    public void Clear()
    {
        foreach (var racer in racers)
        {
            epool.DeactivateObject(racer.toon.gameObject);
        }
        racers.Clear();
        foreach (var ef in GetComponentsInChildren<SpecialEffectController>())
            ef.Stop();
    }

    public RacerToon LoadFighterPrefab(Racer racer, GameObject prefab)
    {
        RacerToon fighter = new RacerToon()
        {
            racer = racer,
        };
        if (PoolToonForPlayer(prefab, transform, 0) is Toon toon)
        {
            toon.character.toonType = CharacterResolver.ToonType.complete;
            toon.overlay.AssignRacer(racer);
            fighter.toon = toon;
        }
        racers.Add(fighter);
        return fighter;
    }
    public Toon PoolToonForPlayer(GameObject prefab, Transform parent, int childIndex = 0)
    {
        var gob = epool.PoolItem(prefab);
        if (gob != null)
        {
            var tempParent = parent.GetChild(childIndex);

            tempParent.transform.localPosition = Vector3.zero;
            tempParent.transform.localScale = Vector3.one;

            gob.transform.SetParent(tempParent.transform);
            gob.transform.localPosition = Vector3.zero;
            gob.transform.localScale = Vector3.one;
            gob.transform.localRotation = Quaternion.identity;

            var toon = gob.GetComponent<Toon>();

           // toon.character.ChangeLayer("Combatant");
          //  toon.character.ChangeSortingLayer("Players");
            toon.character.ChangeMaskInteraction(SpriteMaskInteraction.None);

            return toon;
        }
        return null;
    }
    public RacerToon GetPlayerRacer()
    {
        return GetFighterByOwner(TourneyController.main.GetPlayerRacer());
    }
    public RacerToon GetFighterByOwner(Racer racer)
    {
        return racers.FirstOrDefault(t => t.racer == racer);
    }
    public void ResetAnimations()
    {
        foreach (var toon in racers)
        {
            toon.toon.ResetAnimation();
        }
    }
}
