using System.Collections.Generic;

public class ModifierData : Modifier
{
    public ModifierScriptable data;
    public ModifierData(Racer racer, ModifierScriptable data, int stacks = 1):base(racer, racer, stacks)
    {
        this.data = data;
        ModifierName = data.InternalName;
        SetStackCount(stacks);
    }
    public override void UpdateFromLevel()
    {
        properties.Clear();

        HashSet<ModifierDefines.Property> props = new();
        foreach (ModifierDefines.PropertyData prop in data.properties)
        {
            props.Add(prop.Property);
        }
        foreach (ModifierDefines.RelativeStatData prop in data.relative)
        {
            props.Add(prop.Property);
        }

        foreach (var prop in props)
        {
            if (ModifierDefines.IsPropertyMultiplicative(prop))
                SetProperty(prop, GetProperty(prop) * data.GetProperty(prop, stacks - 1) * data.GetPropertyForRacer(prop, owner) - 1);
            else
                SetProperty(prop, GetProperty(prop) + data.GetProperty(prop, stacks - 1) + data.GetPropertyForRacer(prop, owner));
        }

        if (properties.Count > 0)
            owner.modifiers.RefreshModifier(this);
    }
}