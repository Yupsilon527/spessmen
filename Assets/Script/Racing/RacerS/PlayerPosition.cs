
using UnityEngine;

public class RacerPosition : RacerComponent
{
    int racerPosition = 0;
    bool aheadOfRival = false;
    public float distanceTraveled = 0;
    public int currentLap => Mathf.FloorToInt(distanceTraveled/ (TourneyController.main?.ongoingRace.lapDistance ?? 9999));
    public RacerPosition(Racer racer) : base(racer)
    {
    }
    public override void HandleRacePhase(RaceDefines.RacePhase phase)
    {
        base.HandleRacePhase(phase);
        switch (phase)
        {
            case RaceDefines.RacePhase.RaceBegin:
                distanceTraveled = 0;
                racerPosition = TourneyController.main.ongoingRace.racers.Count;
                aheadOfRival = false;
                break;
            case RaceDefines.RacePhase.RaceTick:
                float dt =  (racer.GetState(ModifierDefines.State.Stunned)) ? 0 : Time.fixedDeltaTime;
                int lastLap = currentLap;
                bool raceStart = distanceTraveled == 0;
                distanceTraveled += racer.stats.realSpeed * dt;
                if (raceStart) return;

                int position = TourneyController.main.ongoingRace.GetPositionForRacer(racer);
                if (position > racerPosition)
                    racer.abilities.ListenToEvent(ShipDefines.PartEvent.OnOtherOvertaken);
                racerPosition = position;

                bool overtakenRival = racer.GetRival() == null ? false : racerPosition > racer.GetRival().position.racerPosition;
                if (aheadOfRival != overtakenRival)
                {
                    if (overtakenRival)
                        racer.abilities.ListenToEvent(ShipDefines.PartEvent.OnRivalOvertaken);
                    aheadOfRival = overtakenRival;
                }

                if (currentLap > lastLap)
                    racer.abilities.ListenToEvent(ShipDefines.PartEvent.OnLapCompleted);
                break;
        }
    }
}
