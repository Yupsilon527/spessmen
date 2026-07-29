
public class Racer 
{
    public int id = 0;
   public float distanceRaced;

    public RacerComponent[] components;
   public  RacerStatsTable stats;
    public RacerPosition position ;
    public RacerAbilities abilities ;
    public RacerModifiers modifiers ;
    public Racer(int rId)
    {
        id = rId;
    }

    public Racer GetRival()
    {
        if (id == 0)
            return TourneyController.main.GetPlayerRival();
        else
            return TourneyController.main.GetPlayerRacer();
    }
    public virtual void HandleRacePhase(RaceDefines.RacePhase phase)
    {
        foreach (var component in components)
        {
            component.HandleRacePhase(phase);
        }
    }
    #region States and Properties
    public virtual bool GetState(ModifierDefines.State State)
    {
        return (modifiers != null && modifiers.GetState(State));
    }
    public virtual float GetPropertyAdditive(ModifierDefines.Property Property)
    {
        float value = 0;
        if (modifiers != null)
            value += modifiers.GetPropertyAdditive(Property);
        return value;
    }
    public virtual float GetPropertyMultiplicative(ModifierDefines.Property Property)
    {
        float value = 1;
        if (modifiers != null)
            value *= modifiers.GetPropertyMultiplicative(Property);
        return value;
    }
    #endregion
}
