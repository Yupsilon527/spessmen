using UnityEditor;
using UnityEngine;

public class ScriptableBase : ScriptableObject
{
    public string InternalName = "MISSING";
    public virtual void Rename()
    {
        InternalName = name;
    }
    protected virtual void OnValidate()
    {
        if (name != InternalName)
        {
            Rename();
        }
    }
}
