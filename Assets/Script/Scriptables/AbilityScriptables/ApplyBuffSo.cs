using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "Data/Parts/Buff Data")]
public class ApplyBuffSo : ApplyTagSo
{
    public ModifierDefines.Priority priority;
    public ModifierDefines.Flag flag;
    public ModifierDefines.Behavior behavior = ModifierDefines.Behavior.Unique;

    public ModifierDefines.StateData[] states;
    public ModifierDefines.PropertyData[] properties;
    public override Modifier Translate(Racer t)
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
}
