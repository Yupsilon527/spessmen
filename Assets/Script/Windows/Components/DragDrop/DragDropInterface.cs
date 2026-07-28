using UnityEngine;

public class AbilityDragDropInterface : Initializable
{
    public DataItemShip ship;
   // public UnitContainerDescriptipn desc;
    #region DD Slots
    public DragDropSlot[] UnitSlots;
    public DragDropSlot[] DiscardSlots;

    void FindSlots()
    {
        UnitSlots = GetComponentsInChildren<DragDropSlot>();
    }
    #endregion

    protected override void Initialize()
    {
        base.Initialize();
        if (TokenPool == null)
            TokenPool = GetComponent<ObjectPool>();
        FindSlots();
    }
    public void InitSlots(DataItemShip s)
    {
        if (UnitSlots == null) return;
        ship = s;

        foreach (var slot in ship.parts)
        {
         
                    var token = GenerateToken(slot);
                    token.parent = this;
                   // token.AttachToSlot(slot, true);
        }
     //   desc.Clear();
    }
    #region Token Pool
    public GameObject TokenPrefab;
    public ObjectPool TokenPool;

    public DragDropToken GenerateToken(PartScriptable u)
    {
        return GenerateToken(u.Translate() as DataItemPart);
    }
    public DragDropToken GenerateToken(DataItemPart u)
    {
        GameObject parent = TokenPool.PoolItem(TokenPrefab);
        DragDropToken token = parent.GetComponent<DragDropToken>();
        token.parent = this;
        token.transform.localScale = Vector3.one;
        token.FromPart(u, true);
        return token;
    }
    #endregion
    #region Hero Init
    public void ApplyChanges()
    {
        foreach (var slot in UnitSlots)
        {
        //    if (slot.attachedToken != null)
           // slot.ship?.formation.SetTroopInPosition(slot.position, slot.attachedToken != null ? slot.attachedToken.tokenUnit : null);
        }

    }
    public void Clear()
    {
        foreach (DragDropSlot token in UnitSlots)
            token.DeleteToken();
    }
    #endregion
}