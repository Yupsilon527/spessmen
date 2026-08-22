using UnityEngine;
using UnityEngine.UI;

public class PartButtonBase : Initializable
{
    public float cellSize = 20;

    public DataItemPart mPart;
    protected RectTransform recttransform;
    protected GridPreview gridPreview;
    public Image sprite, outlineMask;
    public Image outline;
    public int rotation = 0;
    public virtual void FromPart(DataItemPart part, bool draw)
    {
        Clear(false);
        mPart = part;
        if (draw)
            Redraw();
    }
    public virtual void Clear(bool draw)
    {
        mPart = null;
        if (draw)
            Redraw();
    }

    protected override void Initialize()
    {
        base.Initialize();
        FindComponent(ref recttransform);
        FindComponent(ref gridPreview);

    }
    protected virtual void Redraw()
    {
        DrawScriptable(mPart.scriptable);
        gridPreview?.Draw(mPart);
    }
    public virtual void DrawScriptable(PartScriptable part)
    {
        if (sprite != null) sprite.sprite = part.icon;
        if (outlineMask != null) outlineMask.sprite = part.icon;
        if (outline != null) outline.color = ItemDefines.GetColorForRarity(part.boonRarity); 
    }
    public void Rotate(bool clockwise)
    {
        Rotate(rotation + (clockwise ? 1 : -1));
    }
    public void Rotate(int rot)
    {
        rotation = rot % 4;
        AdjustRotation(rotation);
    }
    public virtual void AdjustRotation(int rotation)
    {
        transform.rotation = Quaternion.Euler(0, 0, -90 * rotation);
    }
    public void SnapToGrid(RectTransform targetRect)
    {
        Vector2Int slotCoords = new Vector2Int(mPart.originX, mPart.originY);
        DataItemPlayer.main.car.TryPlace(mPart, slotCoords.x, slotCoords.y, rotation);
        Rect rect = targetRect.rect;

        int width = mPart.rotation % 2 == 0 ? mPart.width : mPart.height;
        int height = mPart.rotation % 2 == 0 ? mPart.height : mPart.width;
        Vector2 localPoint = new Vector2(
            rect.xMin + (slotCoords.x + width / 2f) * cellSize * 2,
            rect.yMax - (slotCoords.y + height / 2f) * cellSize * 2
        );

        Vector3 worldPoint = targetRect.TransformPoint(localPoint);
        recttransform.position = worldPoint;
    }
}
