using System.Collections.Generic;
using System;
using UnityEngine;

public class ContainerMenu : Initializable
{
    [Header("Components")]
    public RectTransform outputTransform;
    public GameObject copyObject;
    List<GameObject> buttonList = new();
    public Action<List<GameObject>> sortAction = (list) => { };

    protected override void Initialize()
    {
        base.Initialize();
        if (outputTransform == null)
            outputTransform = GetComponent<RectTransform>();
        if (copyObject == null)
            copyObject = outputTransform.GetChild(0).gameObject;
    }
    #region List Contents
    public void Refresh()
    {
        ClearList();
        PopulateList();
    }
    protected virtual bool PopulateList()
    {
        int totalEntries = 6;
        for (int i = 0; i < totalEntries; i++)
        {
            GameObject btn = PoolEmptyContainer();
            btn.SetActive(true);
        }
        return true;
    }
    public virtual GameObject PoolEmptyContainer()
    {
        foreach (Transform child in outputTransform)
        {
            if (!child.gameObject.activeSelf)
            {
                buttonList.Add(child.gameObject);
                return child.gameObject;
            }
        }
        GameObject nGO = GameObject.Instantiate(copyObject);
        nGO.name = "Entry " + outputTransform.childCount;
        nGO.transform.SetParent(outputTransform);
        nGO.transform.localScale = Vector3.one;
        buttonList.Add(nGO);
        return nGO;
    }
    public virtual void ClearList()
    {
        foreach (Transform child in outputTransform)
        {
            child.gameObject.SetActive(false);
        }
        buttonList.Clear();
    }
    #endregion

    public void Sort(Action<List<GameObject>> action)
    {
        sortAction = action;
        Sort();
    }
    public void Sort()
    {
        sortAction.Invoke(buttonList);
        for (int i = 0; i < buttonList.Count; i++)
        {
            buttonList[i].transform.SetSiblingIndex(i);
        }
    }
}
