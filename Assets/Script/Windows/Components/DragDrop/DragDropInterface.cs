using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityDragDropInterface : ShipPreview
{
    public HashSet<DragDropToken> tokens = new();
    // public UnitContainerDescriptipn desc;
    protected override void Initialize()
    {
        base.Initialize();
        FindSlots();
    }
    #region DD Slots
    public DragDropSlot buildSlot;
    public DragDropSlot[] DiscardSlots;

    void FindSlots()
    {
        DiscardSlots = GetComponentsInChildren<DragDropSlot>().Where(s => s.slot == DragDropSlot.TokenSlot.discard).ToArray() ;
    }

    public void InitSlots(DataItemShip s)
    {
        ship = s;

        foreach (var slot in ship.parts)
        {
         
                    var token = GenerateToken(slot);
                    token.AttachToSlot(buildSlot, true);
        }
    }
    #endregion
    #region Token Pool

    public DragDropToken GenerateToken(PartScriptable u)
    {
        return GenerateToken(new DataItemPart(u));
    }
    public DragDropToken GenerateToken(DataItemPart u)
    {
        GameObject parent = TokenPool.PoolItem(TokenPrefab);
        DragDropToken token = parent.GetComponent<DragDropToken>();
        token.parent = this;
        token.transform.localScale = Vector3.one;
        token.FromPart(u, true);
        tokens.Add(token);
        return token;
    }
    #endregion
    #region Hero Init
    public void ApplyChanges()
    {
        foreach (var slot in DiscardSlots)  //REFUND 
        {
        //    if (slot.attachedToken != null)
           // slot.ship?.formation.SetTroopInPosition(slot.position, slot.attachedToken != null ? slot.attachedToken.tokenUnit : null);
        }

    }
    public override void Clear()
    {
        foreach (var token in tokens)
            token.Delete();
    }
    #endregion
}