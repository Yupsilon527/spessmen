using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropSlot : EventTrigger
{
    public enum TokenSlot
    {
        build,
        shop,
        discard,
    }
    public TokenSlot slot;
    public bool Locked = false;
    public bool Interactable = true;


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
        if (slot == TokenSlot.build)
            DataItemPlayer.main.ship.ResetOccupancy();
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

    public Vector2Int GetGridPosition(Vector2 screenPos)
    {
        var grid = DataItemPlayer.main.ship;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            recttransform, screenPos, GetCanvasCamera(), out Vector2 localPoint);

        Rect rect = recttransform.rect;

        // localPoint is relative to the pivot; shift so (0,0) is top-left of the rect
        float relativeX = localPoint.x - rect.xMin;
        float relativeY = rect.yMax - localPoint.y;

        int x = Mathf.FloorToInt(relativeX / rect.width * grid.width);
        int y = Mathf.FloorToInt(relativeY / rect.height * grid.height);

        return new Vector2Int(x, y);
    }

    private Camera GetCanvasCamera()
    {
        Canvas canvas = recttransform.GetComponentInParent<Canvas>();
        if (canvas == null) return null;
        return canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
    }
}
