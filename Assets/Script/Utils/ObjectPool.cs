using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public Transform activeObjs;
    public Transform inactiveObjs;

    public virtual GameObject PoolItem(GameObject Prefab)
    {
        foreach (Transform child in inactiveObjs)
        {
            if (child != null && !child.gameObject.activeSelf &&
                Prefab.name.Length <= child.name.Length && Prefab.name == child.name.Substring(0, Prefab.name.Length))
            {
                ActivateObject(child.gameObject);
                return child.gameObject;
            }
        }

        return InitFromPrefab(Prefab);
    }
    protected virtual GameObject InitFromPrefab(GameObject Prefab)
    {
        GameObject nObject = GameObject.Instantiate(Prefab);
        nObject.name = Prefab.name;
        ActivateObject(nObject);
        return nObject;
    }
    public virtual void ActivateObject(GameObject gOb)
    {
        if (gOb == null) return;
        gOb.transform.SetParent(activeObjs);
        gOb.SetActive(true);
    }
    public virtual void DeactivateObject(GameObject gOb, bool changeActive = true)
    {
        if (gOb == null) return;
        if (gOb.transform.parent != inactiveObjs)
        {
            gOb.transform.SetParent(inactiveObjs);
            if (changeActive)
                gOb.SetActive(false);
        }
    }


    public int GetNActiveObjects()
    {
        return activeObjs.childCount;
    }
}
