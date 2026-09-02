using System;
using UnityEngine;
using static ShipDefines;

[Serializable]
public class RacerStatsTable : RacerComponent
{
    public float realSpeed;
    public float gasTotal;
    public float baseSpeed;
    public float boosterSpeed;
    public float alteredSpeed;
    bool requiresUpdate = false;
    bool brokenSoundBarrier = false;

    public RacerStatsTable(Racer racer) : base(racer)
    {
    }
    public override void HandleRacePhase(RaceDefines.RacePhase phase)
    {
        base.HandleRacePhase(phase);
        switch (phase)
        {
            case RaceDefines.RacePhase.RaceBegin:
                realSpeed = 0;
                baseSpeed = 0;
                boosterSpeed = 0;
                alteredSpeed = 0;
                gasTotal = gasBase;
                brokenSoundBarrier = false;
                break;
            case RaceDefines.RacePhase.RaceTick:
                if (requiresUpdate) UpdateRealSpeed();
                break;
        }
    }
    public void SetDirty()
    {
        requiresUpdate = true;
    }
    public void UpdateRealSpeed()
    {
        float bSpeed = baseSpeed;
        bSpeed += racer.GetPropertyAdditive(ModifierDefines.Property.base_speed);
        bSpeed *= racer.GetPropertyMultiplicative(ModifierDefines.Property.base_speed_percent);
        bSpeed += racer.GetPropertyAdditive(ModifierDefines.Property.bonus_speed) * racer.GetPropertyMultiplicative(ModifierDefines.Property.total_speed_percent);

        float tSpeed = boosterSpeed;
        tSpeed += racer.GetPropertyAdditive(ModifierDefines.Property.boost_speed_bonus);
        tSpeed *= racer.GetPropertyMultiplicative(ModifierDefines.Property.boost_speed_percent);

        realSpeed = bSpeed + tSpeed;

        var playerRacer = TourneyController.main.GetPlayerRacer();
        if (playerRacer!=racer)
        {
            realSpeed *= playerRacer.GetPropertyMultiplicative(ModifierDefines.Property.opponent_speed);
            if (playerRacer.GetRival()==racer)
            {
                realSpeed *= playerRacer.GetPropertyMultiplicative(ModifierDefines.Property.rival_speed);
            }
        }
        realSpeed += alteredSpeed;
        realSpeed = Mathf.Clamp(realSpeed, 0, RaceDefines.maxSpeed);

        if (!brokenSoundBarrier && realSpeed > soundBarrierSpeed)
        {
            racer.abilities.ListenToEvent(PartEvent.OnSoundBarrierBroken);
            brokenSoundBarrier = true;
        }


        requiresUpdate = false;
    }
    public void UpdateGasTotal()
    {
        gasTotal = gasBase + racer.GetPropertyAdditive(ModifierDefines.Property.tank_capacity);
    }

    public RacerStatsTable Clone()
    {
        return MemberwiseClone() as RacerStatsTable;
    }
}