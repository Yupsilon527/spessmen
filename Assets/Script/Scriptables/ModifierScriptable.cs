

public class ModifierScriptable : GridScriptable
{
    public ModifierDefines.StateData[] states;
    public ModifierDefines.PropertyData[] properties;
    public ModifierDefines.RelativeStatData[] relative;
    public bool HasModifier()
    {
        return states.Length > 0 || properties.Length > 0 || relative.Length > 0;
    }
    public Modifier GetInnateModifier(Racer racer)
    {
        return new Modifier(racer, this);
    }
}
