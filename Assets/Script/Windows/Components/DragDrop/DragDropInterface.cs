using UnityEngine;

public class AbilityDragDropInterface : Initializable
{
    public DataItemBanner unitA, unitB;
    public UnitContainerDescriptipn desc;
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
    public void InitSlots(DataItemBanner army)
    {
        if (UnitSlots == null) return;
        unitA = army;
        unitB = null;

        foreach (var slot in UnitSlots)
        {
            slot.army = army;

            slot.gameObject.SetActive(slot.army != null);

            if (slot.isActiveAndEnabled)
            {
                DataItemUnit unit = slot.position < 0 ? slot.army.formation.transport : slot.army.formation.Formation[slot.position];

                if (unit != null)
                {
                    var token = GenerateToken(unit);
                    token.parent = this;
                    token.AttachToSlot(slot, true);
                }
            }
        }
        desc.Clear();
    }
    public void InitSlots(DataItemBanner a, DataItemBanner b)
    {
        if (UnitSlots == null) return;
        unitA = a;
        unitB = b;

        foreach (var slot in UnitSlots)
        {
            slot.army = slot.left ? a : b;

            slot.gameObject.SetActive(slot.army != null);

            if (slot.isActiveAndEnabled)
            {
                DataItemUnit unit = slot.position < 0 ? slot.army.formation.transport : slot.army.formation.Formation[slot.position];

                if (unit != null)
                {
                    var token = GenerateToken(unit);
                    token.parent = this;
                    token.AttachToSlot(slot, true);
                }
            }
        }
        desc.Clear();
    }
    #region Token Pool
    public GameObject TokenPrefab;
    public ObjectPool TokenPool;

    public DragDropToken GenerateToken(DataItemUnit u)
    {
        GameObject parent = TokenPool.PoolItem(TokenPrefab);
        DragDropToken token = parent.GetComponent<DragDropToken>();
        token.parent = this;
        token.FromUnit(u, true);
        return token;
    }
    #endregion
    #region Hero Init
    public void ApplyChanges()
    {
        foreach (var slot in UnitSlots)
        {
            if (slot.attachedToken != null)
            slot.army?.formation.SetTroopInPosition(slot.position, slot.attachedToken != null ? slot.attachedToken.tokenUnit : null);
        }

        unitA?.Revise();
        unitB?.Revise();
    }
    public void Clear()
    {
        foreach (DragDropSlot token in UnitSlots)
            token.DeleteToken();
    }
    #endregion
}