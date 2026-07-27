using UnityEngine;

public class ContainerMenu : Initializable
{
    [Header("Components")]
    public RectTransform outputTransform;
    public GameObject copyObject;

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
                return child.gameObject;
            }
        }
        GameObject nGO = GameObject.Instantiate(copyObject);
        nGO.name = "Entry " + outputTransform.childCount;
        nGO.transform.SetParent(outputTransform);
        nGO.transform.localScale = Vector3.one;
        return nGO;
    }
    public virtual void ClearList()
    {
        foreach (Transform child in outputTransform)
        {
            child.gameObject.SetActive(false);
        }
    }
    #endregion

}
