using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class RacerToon
{
    public Racer racer;
    public Toon toon;
    public GameObject character;
    public float displaySpeed;
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
    public ParallaxController parallax;
    public GameObject racerPrefab, opponentPrefab;
    public HashSet<RacerToon> racers = new();
    [Header("Toon Distance Delta")]
    public float distanceFarAway = 300;
    public float distanceDelta = 100;
    public float distanceFarAwayDelta = 30;

    public static ArenaController main;
    private void Awake()
    {
        main = this;
        if (camera == null)
            camera = GetComponentInChildren<Camera>();
        if (epool == null)
            epool = GetComponentInChildren<SpecialEffectPool>();
        if (parallax == null)
            parallax = GetComponentInChildren<ParallaxController>();
    }
    private void OnEnable()
    {
        foreach (var racer in racers)
        {
            racer.displaySpeed = racer.racer.stats.realSpeed;
            racer.toon.ResetAlertTimes();
            racer.toon.animator.SetBool("Driving", false);
        }
    }
    private void Update()
    {
        if (TourneyController.main.currentPhase > TourneyController.TourneyPhase.beforeRace) { 
        UpdateRacerPositions();
        HandleFloatingText();
    }
    }
    public void OnNewRaceBegin()
    {
        ResetAnimations();
        UpdateRacerPositions();
        HandleFloatingText();
    }
    public void UpdateRacerPositions()
    {
        var playerRacer = GetPlayerRacer();
        float posDelta =  TourneyController.main?.ongoingRace?.lapDistance ?? 200;
        foreach (var racer in racers)
        {
            float relativePosition = Mathf.Min(racer.racer.position.distanceTraveled - playerRacer.racer.position.distanceTraveled);
            if (relativePosition == 0) continue;

            float d = 100;
            float m = 15;

            if (racer.racer.position.distanceTraveled < d)
            {
                float t = Mathf.Clamp01(racer.racer.position.distanceTraveled / d);
                float nearStartMultiplier = Mathf.Max(m, 1f / (racer.racer.position.distanceTraveled * m + 0.0001f)); // +epsilon avoids div-by-zero at 0
                float multiplier = Mathf.SmoothStep(nearStartMultiplier, 1f, t);

                relativePosition *= multiplier;
            }
            racer.toon.transform.position = Vector3.right * ((Mathf.Min(Mathf.Abs(1 +relativePosition), posDelta) / distanceDelta + Mathf.Max(Mathf.Abs(relativePosition) - posDelta, 0) / distanceFarAwayDelta) * Mathf.Sign(relativePosition));

        }

        float worldScroll = Mathf.Min(playerRacer.racer.position.distanceTraveled * 10, playerRacer.racer.position.distanceTraveled + 600);
        parallax?.SetWorldDelta(worldScroll);
    }
    void HandleFloatingText()
    {
        foreach (var racer in racers)
        {
            if (!racer.toon.nextAlertTime.IsRunning() && racer.racer.stats.realSpeed != racer.displaySpeed)
            {
                float delta = racer.racer.stats.realSpeed - racer.displaySpeed;
                racer.toon.Alert(delta.ToString("F1"), delta < 0 ? Color.red : Color.white, "center");
                racer.displaySpeed = racer.racer.stats.realSpeed;
            }
            racer.toon.animator.SetBool("Driving", TourneyController.main.currentPhase >= TourneyController.TourneyPhase.racing &&  racer.displaySpeed > 0);
            racer.toon.animator.SetBool("Boosting", TourneyController.main.currentPhase >= TourneyController.TourneyPhase.racing &&  racer.displaySpeed > ShipDefines.soundBarrierSpeed);
        }
    }
    public void LoadRacers(Racer[] racers)
    {
        foreach (var racer in racers)
            LoadFighterPrefab(racer, racer.id == 0 ? racerPrefab : opponentPrefab, racer.playerShip);
    }
    public void Clear()
    {
        foreach (var racer in racers)
        {
            epool.DeactivateObject(racer.character);
            epool.DeactivateObject(racer.toon.gameObject);
        }
        racers.Clear();
        foreach (var ef in GetComponentsInChildren<SpecialEffectController>())
            ef.Stop();
    }

    public RacerToon LoadFighterPrefab(Racer racer, GameObject prefab, ShipScriptable character)
    {
        RacerToon gobject = new RacerToon()
        {
            racer = racer,
        };
        if (PoolToonForPlayer(prefab, transform) is Toon toon)
        {
            toon.character.toonType = CharacterResolver.ToonType.complete;
            toon.overlay.AssignRacer(racer);

            gobject.toon = toon;

            if (PoolPrefabForPlayer(character.prefab, toon.character.FindAttachPoint("origin")) is GameObject g)
            {
                toon.animator = g.GetComponentInChildren<Animator>();
                toon.character.Init();
                gobject.character = g;
            }
            toon.character.ChangeSortingOrder(-racer.id);
        }
        racers.Add(gobject);
        return gobject;
    }
    public Toon PoolToonForPlayer(GameObject prefab, Transform parent)
    {
        if (PoolPrefabForPlayer(prefab, parent) is GameObject gob)
        {
            var toon = gob.GetComponent<Toon>();

            // toon.character.ChangeLayer("Combatant");
            //  toon.character.ChangeSortingLayer("Players");
            toon.character.ChangeMaskInteraction(SpriteMaskInteraction.None);

            return toon;
        }
        return null;
    }
    public GameObject PoolPrefabForPlayer(GameObject prefab, Transform parent)
    {
        var gob = epool.PoolItem(prefab);
        if (gob != null)
        {

            gob.transform.SetParent(parent.transform);
            gob.transform.localPosition = Vector3.zero;
            gob.transform.localScale = prefab.transform.localScale;
            gob.transform.localRotation = Quaternion.identity;

            return gob;
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
