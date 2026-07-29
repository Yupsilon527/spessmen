
using System.Linq;

public class ModifierScriptable : GridScriptable
{
    public ModifierDefines.StateData[] states;
    public ModifierDefines.PropertyData[] properties;
    public ModifierDefines.RelativeStatData[] relative;
    public bool GetState(ModifierDefines.State state)
    {
        return states.Any(s => sx.State == state);
    }
}
