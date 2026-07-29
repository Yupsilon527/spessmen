
public class RacerComponent 
{
    public Racer racer;
    public RacerComponent(Racer racer)
    {
        this.racer = racer;
    }
    public virtual void OnRaceBegin() { }
    public virtual void OnRaceEnd() { }
}