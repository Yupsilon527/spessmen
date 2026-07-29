
public class RacerComponent 
{
    public Racer racer;
    public RacerComponent(Racer racer)
    {
        this.racer = racer;
    }
    public virtual void HandleRacePhase(RaceDefines.RacePhase phase) { }
}