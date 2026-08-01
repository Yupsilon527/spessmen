using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropToken : PartButtonBase
{
    public AbilityDragDropInterface parent;
    bool dragDropMode = false;
    public EventTrigger eventTrigger;

    #region Rotate
    private void Update()
    {
        if (dragDropMode)
        {
            if (Input.mouseScrollDelta.y < 0)
            {
                Rotate(true);
            }
            else if (Input.mouseScrollDelta.y > 0)
            {
                Rotate(false);
            }
        }
    }
    public void Rotate(bool clockwise)
    {
        mPart.Rotate(clockwise);
        AdjustRotation(mPart.rotation);
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
    /* public override void OnPointerEnter(PointerEventData eventData)
     {
         //      RectTransform rtf = GetComponent<RectTransform>();
         //   InfoOverlayController.main.OpenAtPosition(GetTooltipString(), rtf.position);
     }
     public override void OnPointerExit(PointerEventData eventData)
     {
         //     InfoOverlayController.main.Close();
     }*/
    #endregion
    #region Drag Drop
    protected override void Initialize()
    {
        base.Initialize();
        FindComponent(ref eventTrigger);

        EventTrigger.Entry OnBeginDrag = new EventTrigger.Entry();
        OnBeginDrag.eventID = EventTriggerType.BeginDrag;
        OnBeginDrag.callback.AddListener((data) => { this.OnBeginDrag((PointerEventData)data); });
        eventTrigger.triggers.Add(OnBeginDrag);

        EventTrigger.Entry OnDrag = new EventTrigger.Entry();
        OnDrag.eventID = EventTriggerType.Drag;
        OnDrag.callback.AddListener((data) => { this.OnDrag((PointerEventData)data); });
        eventTrigger.triggers.Add(OnDrag);

        EventTrigger.Entry OnEndDrag = new EventTrigger.Entry();
        OnEndDrag.eventID = EventTriggerType.EndDrag;
        OnEndDrag.callback.AddListener((data) => { this.OnEndDrag((PointerEventData)data); });
        eventTrigger.triggers.Add(OnEndDrag);

        EventTrigger.Entry PointerEnter = new EventTrigger.Entry();
        PointerEnter.eventID = EventTriggerType.PointerEnter;
        PointerEnter.callback.AddListener((data) => { this.OnPointerEnter((PointerEventData)data); });
        eventTrigger.triggers.Add(PointerEnter);

        EventTrigger.Entry PointerExit = new EventTrigger.Entry();
        PointerExit.eventID = EventTriggerType.PointerExit;
        PointerExit.callback.AddListener((data) => { this.OnPointerExit((PointerEventData)data); });
        eventTrigger.triggers.Add(PointerExit);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slot.Interactable)
        {
            dragDropMode = true;
            // InfoOverlayController.main.Close();
            if (slot.slot == DragDropSlot.TokenSlot.build)
                DataItemPlayer.main.ship.RemovePart(mPart);

            ViewManager.Instance.shop.playership.UpdateVisual();
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (dragDropMode)
        {
            recttransform.position = Input.mousePosition;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        RaycastSlot();
        dragDropMode = false;
        ViewManager.Instance.shop.playership.UpdateVisual();
    }
    bool RaycastSlot()
    {
        GraphicRaycaster gr = WindowManager.main.GetComponent<GraphicRaycaster>();//TODO define
        PointerEventData ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        ped.radius = recttransform.sizeDelta;
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(ped, results);

        foreach (RaycastResult ob in results)
        {
            if (ob.isValid)
            {
                if (ob.gameObject.TryGetComponent(out DragDropToken ntoken))
                {
                    if (TryMergeAnother(ntoken))
                        return true;
                }
                else if (ob.gameObject.TryGetComponent(out DragDropSlot nslot))
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
        }
        SnapBack();
        return false;
    }
    #endregion
    #region Merging
    public bool CanMergeAnother(DragDropToken token)
    {
        return token.mPart.CanMerge(mPart);
    }
    public bool TryMergeAnother(DragDropToken token)
    {
        if (token!=this && CanMergeAnother(token))
        {
            token.mPart.Transform(token.mPart.GetMergeOutcome(mPart));
            token.mPart.purchaseCost = token.mPart.purchaseCost + token.mPart.purchaseCost;
            token.FromPart(token.mPart, true);
            Delete();
            return true;
        }
        return false; 
    }
    #endregion
    #region Attach
    DragDropSlot slot;
    Vector3 Delta()
    {
        return new Vector3(cellSize * -mPart.width, cellSize * mPart.height) / 2;
    }
    bool CanAttachToSlot(DragDropSlot slot)
    {
        if (slot.slot == DragDropSlot.TokenSlot.shop)
            return false;
        if (slot.slot == DragDropSlot.TokenSlot.discard)
            return mPart.CanBeDiscarded();
        if (slot.slot == DragDropSlot.TokenSlot.stash)
            return true;

        var slotCoords = slot.grid.GetGridPosition(transform.position + Delta());
        return DataItemPlayer.main.ship.CanPlace(mPart, slotCoords.x, slotCoords.y);

    }

    public void AttachToSlot(DragDropSlot slot, bool force)
    {
        if (slot == null)
            return;
        if (force)
        {
            if (this.slot != null && this.slot.attachedToken != null)
                this.slot.ClearToken();
            ChangeSlot(slot, true);
        }
        else if (CanAttachToSlot(slot))
        {
            if (slot.slot == DragDropSlot.TokenSlot.discard)
            {
                DataItemPlayer.main.econ.GiveGold(mPart.purchaseCost * .6f);
                Delete();
            }
            else if (slot.attachedToken == null)
            {
                ChangeSlot(slot);
            }
            else if (this.slot.slot != DragDropSlot.TokenSlot.build)
            {
                SwapSlots(slot.attachedToken);
            }
        }
        SnapBack();
    }
    void ChangeSlot(DragDropSlot target, bool force = false)
    {
        if (slot != null && slot != target && this.slot.slot != DragDropSlot.TokenSlot.build)
            slot.ClearToken();
        slot = target;
        if (target.slot == DragDropSlot.TokenSlot.build)
        {
            if (!force)
            {
                Vector2 realPos = transform.position + Delta();
                Vector2Int slotCoords = target.grid.GetGridPosition(realPos);
                DataItemPlayer.main.ship.TryPlace(mPart, slotCoords.x, slotCoords.y);
            }
        }
        else
        {
            slot.attachedToken = this;
        }
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
        {
            if (slot.slot == DragDropSlot.TokenSlot.build)
            {
                SnapToGrid(slot.recttransform);
            }
            else
            {
                recttransform.position = slot.recttransform.position;
            }
        }
    }
    void DiscardToken()
    {
        foreach (DragDropSlot trashslot in parent.stashSlots)
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
        if (slot != null)
            slot.ClearToken();
        slot = null;
        parent.TokenPool.DeactivateObject(gameObject);
    }
    #endregion
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (mPart != null && parent != null && parent.tooltip != null)
        {
            parent.tooltip.ShowPart(mPart.scriptable);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (parent != null && parent.tooltip != null)
        {
            parent.tooltip.Clear();
        }
    }
}
