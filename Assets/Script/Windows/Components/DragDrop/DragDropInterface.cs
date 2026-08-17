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
    public PartTooltip tooltip;
    public DragDropSlot buildSlot;
    public DragDropSlot[] shopSlots;
    public DragDropSlot[] stashSlots;

    void FindSlots()
    {
        shopSlots = GetComponentsInChildren<DragDropSlot>().Where(s => s.slot == DragDropSlot.TokenSlot.shop).ToArray();
        stashSlots = GetComponentsInChildren<DragDropSlot>().Where(s => s.slot == DragDropSlot.TokenSlot.stash).ToArray();
    }

    public void InitSlots(DataItemShip s)
    {
        if (s == null) return;
        ship = s;

        foreach (var part in ship.parts)
        {
            if (part.deleted) continue;
            var token = GenerateToken(part);
            token.AttachToSlot(buildSlot, true);
        }
        int i = 0;
        foreach (var part in ship.stash)
        {
            if (part.deleted) continue;
            var token = GenerateToken(part);
            token.AttachToSlot(stashSlots[i], true);
            i++;
        }
    }
    #endregion
    #region Token Pool

    public DragDropToken GenerateToken(PurchaseData p)
    {
        return GenerateToken(new DataItemPart(p.part, p.itemCost));
    }
    public DragDropToken GenerateToken(PartScriptable u)
    {
        return GenerateToken(new DataItemPart(u, u.GetBasePrice()));
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
        ship.stash.RemoveWhere(p => p == null || ship.parts.Contains(p));
        foreach (var slot in stashSlots)
        {
            if (slot.attachedToken != null && slot.attachedToken.mPart != null)
            {
                var part = slot.attachedToken.mPart;
                ship.parts.Remove(part);
                ship.stash.Add(part);
            }
        }
        foreach (var slot in shopSlots)
        {
            if (slot.attachedToken != null)
            {
                var part = slot.attachedToken.mPart;
                ship.stash.Add(part);
            }
        }

    }
    public override void Clear()
    {
        foreach (var token in tokens)
            token.Delete();
        tokens.Clear();
    }
    #endregion
}