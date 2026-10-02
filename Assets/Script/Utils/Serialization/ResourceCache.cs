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
    public List<EnvironmentScriptable> environments = new();
    public List<TrackedUnlockable> unlockables = new();
    void Precache()
    {
        ships.AddRange(Resources.LoadAll<ShipScriptable>("Scriptables"));
        parts.AddRange(Resources.LoadAll<PartScriptable>("Scriptables"));
        environments.AddRange(Resources.LoadAll<EnvironmentScriptable>("Scriptables"));

    }

    private void Start()
    {
        foreach (var ship in ships.Where(s => !s.IsUnlocked()))
        {
            unlockables.Add(new TrackedUnlockable(ship));
        }
        foreach (var part in parts.Where(s => !s.IsUnlocked()))
        {
            unlockables.Add(new TrackedUnlockable(part));
        }
    }

    public ScriptableBase LoadAny(string name) { return string.IsNullOrEmpty(name) ? null : ships.FirstOrDefault(items => items.InternalName == name); }
    public ShipScriptable LoadShip(string name) { return string.IsNullOrEmpty(name) ? null : ships.FirstOrDefault(items => items.InternalName == name); }
    public PartScriptable LoadComponent(string name) { return string.IsNullOrEmpty(name) ? null : parts.FirstOrDefault(items => items.InternalName == name); }
    public EnvironmentScriptable LoadEnvironment(string name) { return string.IsNullOrEmpty(name) ? null : environments.FirstOrDefault(items => items.InternalName == name); }
}

public class TrackedUnlockable
{
    public UnlockScriptable unlockable;
    public bool isUnlocked;

    public TrackedUnlockable(UnlockScriptable unlockable)
    {
        this.unlockable = unlockable;
        RecheckUnlocked();
    }
    public void RecheckUnlocked()
    {
        this.isUnlocked = unlockable.IsUnlocked();
    }
}