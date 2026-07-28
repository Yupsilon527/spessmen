using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropSlot : EventTrigger
{
    public enum TokenSlot
    {
        setup,
        shop,
        discard,
    }
    public DataItemShip ship;
    public TokenSlot slot;
    public int position;
    public bool Locked = false;
    public bool Interactable = true;

    public int slotX => Mathf.FloorToInt(position / ship.width);
    public int slotY => position % ship.width;

    public RectTransform recttransform;
    public DragDropToken attachedToken;
   

    #region Components
    private void Awake()
    {
        if (recttransform == null)
            recttransform = GetComponent<RectTransform>();
    }
    #endregion
    #region Info Overlay
    public virtual string GetTooltipString()
    {
       
        return "";
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        string toolTip = GetTooltipString();
     //   if (toolTip != string.Empty)
     //       InfoOverlayController.main.OpenAtPosition(toolTip, recttransform.position);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        //InfoOverlayController.main.Close();
    }
    #endregion

    #region Attached Token

    public void AttachToken(DragDropToken token, bool gen)
    {
        attachedToken = token;
        token.AttachToSlot(this, gen);
    }
    public void ClearToken()
    {
        attachedToken = null;
    }
    public void DeleteToken()
    {
        if (attachedToken != null)
            attachedToken.Delete();
        ClearToken();
    }
    #endregion
    #region Locked 
    public void SetLocked(bool value)
    {
        Locked = value;
    }
    #endregion
    #region Enabled/Disalbed 
    public void SetEnabled(bool value)
    {
        Interactable = value;
    }
    #endregion
}
