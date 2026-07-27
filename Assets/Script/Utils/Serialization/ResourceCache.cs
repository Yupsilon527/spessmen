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

    public List<DataItemShip> ships = new();
    public List<DataItemPart> parts = new();
    void Precache()
    {
        ships.AddRange(Resources.LoadAll<ItemData>("Scriptables"));
        parts.AddRange(Resources.LoadAll<EquipmentData>("Scriptables"));
        tasks.AddRange(Resources.LoadAll<WorkTaskSO>("Scriptables"));
        recipes.AddRange(Resources.LoadAll<CragtingRecipeSO>("Scriptables"));
    }

    public ScriptableBase LoadAny(string name) { return string.IsNullOrEmpty(name) ? null : ships.FirstOrDefault(items => items.InternalName == name); }
    public ItemData LoadItem(string name) { return string.IsNullOrEmpty(name) ? null : ships.FirstOrDefault(items => items.InternalName == name); }
    public EquipmentData LoadEquipt(string name) { return string.IsNullOrEmpty(name) ? null : parts.FirstOrDefault(items => items.InternalName == name); }
    public WorkTaskSO LoadResource(string name) { return string.IsNullOrEmpty(name) ? null : tasks.FirstOrDefault(items => items.InternalName == name); }
    public CragtingRecipeSO LoadRecipe(string name) { return string.IsNullOrEmpty(name) ? null : recipes.FirstOrDefault(items => items.InternalName == name); }
}
