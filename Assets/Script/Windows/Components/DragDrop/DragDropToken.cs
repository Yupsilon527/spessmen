using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropToken : EventTrigger
{
    bool playerOwned;
    public DataItemUnit tokenUnit;
    public UnitContainer drawUnit;
    public AbilityDragDropInterface parent;
    RectTransform recttransform;
    public void ClearToken(bool draw)
    {
        tokenUnit = null;
        HasMoved = false;
        if (draw)
            Redraw();
    }
    #region Draw
    private void Awake()
    {
        if (recttransform == null)
            recttransform = GetComponent<RectTransform>();
    }
    bool dragDropMode = false;
    private void Update()
    {
        if (!dragDropMode)
            SnapBack();
    }
    void Redraw()
    {
        drawUnit.ForUnit(tokenUnit);
    }
    #endregion
    #region Info Overlay
    public virtual string GetTooltipString()
    {
      /*  switch (behavior)
        {
            case TokenBehavior.Ability:
                if (attachedAbility != null)
                {
                    return attachedAbility.original.GetTooltip(true);
                }
                break;
            case TokenBehavior.Effect:
                if (attachedEffect)
                {
                    string desc = attachedEffect.name;//TODO getdesc
                    if (slot != null && slot.GetMod() != null)
                        desc = slot.GetMod().modCondition + "\n" + desc;//TODO get modData desc
                    return desc;
                }
                break;
            case TokenBehavior.Event:
                if (overrideEvent != AbilityDefines.Event.Nothing)
                {
                    return overrideEvent.ToString();
                }
                break;
        }*/
        return "MISSINGNUM";
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
  //      RectTransform rtf = GetComponent<RectTransform>();
     //   InfoOverlayController.main.OpenAtPosition(GetTooltipString(), rtf.position);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
   //     InfoOverlayController.main.Close();
    }
    #endregion
    #region Drag Drop
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        parent?.desc.ForUnit(tokenUnit);
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if (slot.Interactable && playerOwned)
        {
            dragDropMode = true;
           // InfoOverlayController.main.Close();
        }
    }
    public override void OnDrag(PointerEventData eventData)
    {
        if (dragDropMode) { 
        recttransform.position = Input.mousePosition;
        base.OnDrag(eventData);
        base.OnDrag(eventData);
    }
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (playerOwned)
        {
            RaycastSlot();
        }
        dragDropMode = false;
        base.OnEndDrag(eventData);
    }
    bool RaycastSlot()
    {
        GraphicRaycaster gr = InterfaceManager.main.GetComponent<GraphicRaycaster>();//TODO define
        PointerEventData ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        ped.radius = recttransform.sizeDelta;
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(ped, results);

        foreach (RaycastResult ob in results)
        {
            if (ob.isValid && ob.gameObject.TryGetComponent(out DragDropSlot nslot))
            {
                if (!nslot.Interactable)
                    continue;
                if (CanAttachToSlot(nslot))
                {
                    AttachToSlot(nslot, false);
                    return true;
                }
            }
        }

        return false;
    }
    #endregion

    public void FromUnit(DataItemUnit unit, bool draw)
    {
        ClearToken(false);
        tokenUnit = unit;
        playerOwned = unit.GetAlignment(GameManager.main.playerManager.currentPlayer) ==  PlayerDefines.Alignment.playerowned;
        if (draw)
            Redraw();
    }

    #region Attach
    DragDropSlot slot;
    bool CanAttachToSlot(DragDropSlot slot)
    {
        if (tokenUnit.isTransport())
            return slot.position < 0;

        return (slot.army.formation.CanIAccept(tokenUnit));
    }
    public void AttachToSlot(DragDropSlot slot, bool force)
    {
        if (slot == null)
            return;
        if (force)
        {
            if (this.slot != null && this.slot.attachedToken != null)
                this.slot.ClearToken();
            ChangeSlot(slot);
        }
        else if (CanAttachToSlot(slot))
        {
          if (slot.attachedToken == null)
            {
                ChangeSlot(slot);
            }
            else
            {
                SwapSlots(slot.attachedToken);
            }
        }
    }
    void ChangeSlot(DragDropSlot slot)
    {
        if (this.slot != null && this.slot != slot)
            this.slot.ClearToken();
        this.slot = slot;
        this.slot.attachedToken = this;
    }
    void SwapSlots(DragDropToken other)
    {
        var slotA = slot;
        var slotB = other.slot;

        if (slotA == slotB)
        { return; }

        ChangeSlot(slotB);

        if (slotA.Locked)
        {
            other.DiscardToken();
        }
        else
        {
            other.ChangeSlot(slotA);
        }
    }
    void SnapBack()
    {
        if (slot != null)
            recttransform.position = slot.recttransform.position;
    }
    void DiscardToken()
    {
        foreach (DragDropSlot trashslot in parent.DiscardSlots)
        {
            if (trashslot.attachedToken == null)
            {
                ChangeSlot(trashslot);
                break;
            }
        }
    }
    public void Delete()
    {
        ClearToken(false);
        slot = null;
        parent.TokenPool.DeactivateObject(gameObject);
    }
    #endregion
    #region Has Moved
    bool moved = false;

    public bool HasMoved { get => moved; set => moved = value; }
    #endregion
}
