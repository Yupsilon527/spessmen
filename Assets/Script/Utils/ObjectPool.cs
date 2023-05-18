using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public Transform activeObjs;
    public Transform inactiveObjs;

        public GameObject PoolItem(GameObject Prefab)
    {
        foreach (Transform child in inactiveObjs)
        {
            if (!child.gameObject.activeSelf && child.name == Prefab.name)
            {
                ActivateObject(child.gameObject);
                return child.gameObject;
            }
        }

        return InitFromPrefab(Prefab);
    }
    GameObject InitFromPrefab(GameObject Prefab)
    {
        GameObject nEnemy = GameObject.Instantiate(Prefab);
        nEnemy.name = Prefab.name;
        if (nEnemy.TryGetComponent(out PoolObject mobData))
        {
            mobData.activeObjectPool = this;
        }
        ActivateObject(nEnemy);
        return nEnemy;
    }
    public void ActivateObject(GameObject gOb)
    {
        gOb.transform.SetParent(activeObjs);
        gOb.SetActive(true);
    }
    public void DeactivateObject(GameObject gOb)
    {
        gOb.transform.SetParent(inactiveObjs);
            gOb.SetActive(false);
        
    }


    public int GetNActiveObjects()
    {
        return activeObjs.childCount;
    }
}
