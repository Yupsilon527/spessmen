using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "Data/Parts/Buff Data")]
public class ApplyBuffSo : ApplyTagSo
{
    public ModifierDefines.Priority priority;
    public ModifierDefines.Flag flag;
    public ModifierDefines.Behavior behavior = ModifierDefines.Behavior.Unique;

    public ModifierDefines.StateData[] states;
    public ModifierDefines.PropertyData[] properties;
    public override Modifier Translate(Racer t, float m)
    {
        var modifier =  new Modifier(t, 0)
        {
            ModifierName = modifierName,
            states = new(),
            properties = new (),
            priority = priority,
            flag = flag,
            behavior = behavior,
        };
        foreach (var state in states)
        {
            modifier.states.Add(state.State);
        }
        foreach (var prop in properties)
        {
            modifier.properties.Add(prop.Property, prop.value);
        }
        return modifier;
    }
    public override string GetDescription()
    {
        string states = "";

        foreach (var prop in properties)
        {
            if (states.Length > 0)
            {
                states += ", ";
            }
            states += $"{prop.ValueToString(0, true, 0)} {LanguageController.main.Translate("Modifiers", "prop_" + prop.Property.ToString())}";
        }

        if (states.Length > 0 && this.states.Length > 0)
        {
            states += ", ";
        }
        foreach (var state in this.states)
        {
            if (states.Length > 0)
            {
                states += ", ";
            }
            states += LanguageController.main.Translate("Modifiers", "state_" + state.State.ToString());
        }

    string label = LanguageController.main.Translate("Modifiers", "Gain Effect").Replace("%value%", states);
        label = label.Replace("%source%", LanguageController.main.Translate("Abilities", "source_" + effectSource))
               .Replace("%target%", LanguageController.main.Translate("Abilities", "target_" + effectTarget));
        return label;
    }
}
