using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompartimentComponent : MonoBehaviour
{
    public List<PlayerMob> Inhabitants;
    public int InhabitantLimit = 1;
    public Mob Owner;
    public DoorComponent entryDoor;
    private void Awake()
    {
        Owner = GetComponent<Mob>();
        Inhabitants = new List<PlayerMob>();
    }

    public bool CanLoadMob()
    {
        return Inhabitants.Count < InhabitantLimit;
    }
    public void LoadMob(PlayerMob mob)
    {
        Inhabitants.Add(mob);
        mob.indoor = this;
        mob.OnEnterBuilding();
        if (entryDoor != null)
            entryDoor.OnUsed();
    }
    public void UnloadMob(PlayerMob mob)
    {
        if (entryDoor!=null)
            UnloadMobAtPosition(mob, entryDoor.transform.position);
        else
        UnloadMobAtPosition(mob, transform.position);
    }
    public void UnloadMobAtPosition(PlayerMob mob, Vector2 position)
    {
        mob.transform.position = position;
        Inhabitants.Remove(mob);
        mob.indoor = null;
        mob.OnExitBuilding();
        if (entryDoor != null)
            entryDoor.OnUsed();
    }
}
