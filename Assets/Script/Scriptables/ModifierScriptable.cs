

using System.Linq;

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
        return new ModifierData(racer, this);
    }
    public float GetProperty(ModifierDefines.Property property, int level)
    {
        bool multiplicative = ModifierDefines.IsPropertyMultiplicative(property);
        float total = multiplicative ? 1 : 0;
        foreach (var prop in properties.Where(p => p.Property == property))
        {
            if (multiplicative)
                total *= 1+prop.value + prop.IncreasePerLevel * level;
            else
                total += prop.value + prop.IncreasePerLevel * level;
        }
        return total;
    }
    public float GetPropertyForRacer(ModifierDefines.Property property, Racer racer)
    {
        bool multiplicative = ModifierDefines.IsPropertyMultiplicative(property);
        float total = multiplicative ? 1 : 0;
        foreach (var prop in relative.Where(p => p.Property == property))
        {
            if (multiplicative)
                total *= 1+prop.GetValueForRacer(racer);
            else
                total += prop.GetValueForRacer(racer);
        }
        return total;
    }
    public virtual string GetEffectDescription()
    {
        string output = "";

        foreach (var prop in properties)
        {
            if (output.Length > 0)
            {
                output += "<br>";
            }
            output += $"{LanguageController.main.Translate("Modifiers", "prop_"+prop.Property.ToString())} {prop.ValueToString(0,true,0)}";
        }

        if (output.Length > 0 && relative.Length > 0)
        {
            output += "<br><br>";
        }
        foreach (var rel in relative)
        {
            if (output.Length > 0)
            {
                output += "<br>";
            }
            output += rel.ValueToString() ;
        }

        if (output.Length > 0 && states.Length > 0)
        {
            output += "<br><br>";
        }
        foreach (var state in states)
        {
            if (output.Length > 0)
            {
                output += "<br>";
            }
            output += LanguageController.main.Translate("Modifiers", "state_" + state.State.ToString()) ;
        }

        return output;
    }
}
