
public static class RaceDefines
{
    public static int SeasonRaces = 3;
    public static int TournamentSeasons = 3;

    public static float raceLength = 20;
    public static float raceLengthLong = 30;

    public enum RacePhase
    {
        RaceBegin = 0,
        RaceSetup = 1,
        RaceTick = 2,
        RaceEnd = 3,

    }
    public enum AbilityTarget
    {
        Self,
        FirstRacer,
        FrontRacer,
        BackRacer,
        RivalRacer,
    }
    public enum RaceModifiers
    {
        Nothing = 0,
        FasterRival=1,
        FuelCosnumption=2,
        ActiveCooldown=3,
        EngineCooldown=4,
        LapsLonger=5,
        LongerRace=6,

        //FEA PlayerStunned,
        //FEA RivalImmune,

        Elite = 7,

        AllCooldownsOff=7,
        RandomEngine=8,
        RandomGadget=9,
        Total=10,
    }

    public static Racer GetRacerRelative(Racer original, AbilityTarget target)
    {
        switch (target)
        {
            case AbilityTarget.FirstRacer:
                return TourneyController.main.ongoingRace?.racers[0] ?? null;
            case AbilityTarget.RivalRacer:
                return TourneyController.main.GetPlayerRival();
            case AbilityTarget.FrontRacer:
            case AbilityTarget.BackRacer:
                if (TourneyController.main?.ongoingRace == null) return null;
                int position = TourneyController.main.ongoingRace.GetPositionForRacer(original) + (target == AbilityTarget.FrontRacer ? -1 : 1) + TourneyController.main.ongoingRace.racers.Count;
                return TourneyController.main.ongoingRace?.racers[position % TourneyController.main.ongoingRace.racers.Count] ?? null;
            default:
                return original;
        }
    }
}
