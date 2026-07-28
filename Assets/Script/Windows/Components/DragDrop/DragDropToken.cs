using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropToken : EventTrigger
{
    public float cellSize = 20;
    public DataItemPart tokenPart;
    public AbilityDragDropInterface parent;
    GridPreview gridPreview;
    RectTransform recttransform;

    public Image sprite;
    public void ClearToken(bool draw)
    {
        tokenPart = null;
        HasMoved = false;
        if (draw)
            Redraw();
    }
    #region Draw
    private void Awake()
    {
        if (recttransform == null)
            recttransform = GetComponent<RectTransform>();
        if (gridPreview == null)
            gridPreview = GetComponentInChildren<GridPreview>();
    }
    bool dragDropMode = false;
    void Redraw()
    {
        sprite.sprite = tokenPart.scriptable.icon;
        gridPreview.Draw(tokenPart.value, tokenPart.scriptable.grid.width, tokenPart.scriptable.grid.height);
    }
    #endregion
    #region Rotate
    public void Rotate(bool clockwise)
    {
        tokenPart.Rotate(clockwise);
        AdjustRotation();
    }
    void AdjustRotation()
    {
        transform.rotation = Quaternion.Euler(0, 0, 90 * tokenPart.rotation);
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
        // parent?.desc.ForUnit(tokenUnit);
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if (slot.Interactable)
        {
            dragDropMode = true;
            // InfoOverlayController.main.Close();
            if (slot.slot == DragDropSlot.TokenSlot.build)
                DataItemPlayer.main.ship.RemovePart(tokenPart);
        }
    }
    public override void OnDrag(PointerEventData eventData)
    {
        if (dragDropMode)
        {
            recttransform.position = Input.mousePosition;
            base.OnDrag(eventData);
            base.OnDrag(eventData);
        }
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        RaycastSlot();
        dragDropMode = false;
        base.OnEndDrag(eventData);
        SnapBack();
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

    public void FromPart(DataItemPart part, bool draw)
    {
        ClearToken(false);
        tokenPart = part;
        if (draw)
            Redraw();
    }

    #region Attach
    DragDropSlot slot;
    bool CanAttachToSlot(DragDropSlot slot)
    {
        if (slot.slot == DragDropSlot.TokenSlot.shop)
            return false;
        if (slot.slot == DragDropSlot.TokenSlot.discard)
            return tokenPart.CanBeDiscarded();

        var slotCoords = slot.GetGridPosition(transform.position);
        return DataItemPlayer.main.ship.CanPlace(tokenPart, slotCoords.x, slotCoords.y);

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
        SnapBack();
    }
    void ChangeSlot(DragDropSlot slot)
    {
        if (this.slot != null && this.slot != slot)
            this.slot.ClearToken();
        this.slot = slot;
        if (slot.slot == DragDropSlot.TokenSlot.build)
        {
            var slotCoords = slot.GetGridPosition(transform.position );
            DataItemPlayer.main.ship.TryPlace(tokenPart, slotCoords.x, slotCoords.y);
        }
        else { 
        this.slot.attachedToken = this;
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
            if (slot.slot == DragDropSlot.TokenSlot.build)
            {
                Vector2Int slotCoords = new Vector2Int(tokenPart.originX, tokenPart.originY);
                DataItemPlayer.main.ship.TryPlace(tokenPart, slotCoords.x, slotCoords.y);
                Rect rect = slot.recttransform.rect;

                Vector2 localPoint = new Vector2(
                    rect.xMin + (slotCoords.x+1 )* cellSize / recttransform.lossyScale.x,
                    rect.yMax - (slotCoords.y+1) * cellSize / recttransform.lossyScale.x
                );

              //  if (tokenPart.width % 2 == 0) localPoint.x += cellSize / 2f;
             //   if (tokenPart.height % 2 == 0) localPoint.y -= cellSize / 2f;

                Vector3 worldPoint = slot.recttransform.TransformPoint(localPoint );
                recttransform.position = worldPoint;
            }
            else
            {
                recttransform.position = slot.recttransform.position;
            }
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
