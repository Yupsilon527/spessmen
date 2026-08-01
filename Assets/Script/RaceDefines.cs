
public static class RaceDefines 
{
    public enum RacePhase
    {
        RaceBegin = 0,
        RaceTick = 1,
        RaceEnd = 2,

    }
    public enum AbilityTarget
    {
        Self,
        FirstRacer,
        FrontRacer,
        BackRacer,
    }

    public static Racer GetRacerRelative (Racer original, AbilityTarget target)
    {
        switch (target)
        {
            case AbilityTarget.FirstRacer:
                return TourneyController.main.ongoingRace?.racers[0] ?? null;
                case AbilityTarget.FrontRacer:
            case AbilityTarget.BackRacer:
                if (TourneyController.main?.ongoingRace == null) return null;
                int position = TourneyController.main.ongoingRace.GetPositionForRacer(original)+ (target == AbilityTarget.FrontRacer ? 1 : -1) % TourneyController.main.ongoingRace.racers.Count;
                return TourneyController.main.ongoingRace?.racers[position] ?? null;
            default:
                return original;
        }
    }
}
