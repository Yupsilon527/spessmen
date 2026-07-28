using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceCache : Initializable
{
    public static ResourceCache main;
    protected override void Initialize()
    {
        if (main == null)
        {
            main = this;
            DontDestroyOnLoad(gameObject);
            Precache();
            base.Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<ShipScriptable> ships = new();
    public List<PartScriptable> parts = new();
    void Precache()
    {
        ships.AddRange(Resources.LoadAll<ShipScriptable>("Scriptables"));
        parts.AddRange(Resources.LoadAll<PartScriptable>("Scriptables"));
    }

    public ScriptableBase LoadAny(string name) { return string.IsNullOrEmpty(name) ? null : ships.FirstOrDefault(items => items.InternalName == name); }
    public ShipScriptable LoadResource(string name) { return string.IsNullOrEmpty(name) ? null : ships.FirstOrDefault(items => items.InternalName == name); }
    public PartScriptable LoadRecipe(string name) { return string.IsNullOrEmpty(name) ? null : parts.FirstOrDefault(items => items.InternalName == name); }
}
