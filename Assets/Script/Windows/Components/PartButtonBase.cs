using UnityEngine;
using UnityEngine.UI;

public class PartButtonBase : Initializable
{
    public float cellSize = 20;

    public DataItemPart mPart;
    protected RectTransform recttransform;
    protected   GridPreview gridPreview;
    public Image sprite;
    public Outline outline;
    public void FromPart(DataItemPart part, bool draw)
    {
        ClearToken(false);
        mPart = part;
        if (draw)
            Redraw();
    }
    public void ClearToken(bool draw)
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
        sprite.sprite = mPart.scriptable.icon;
        gridPreview.Draw(mPart.mGrid, mPart.scriptable.grid.width, mPart.scriptable.grid.height);
  if (outline!=null)      outline.effectColor = ItemDefines.GetColorForRarity(mPart.scriptable.boonRarity);
    }
    public virtual void AdjustRotation(int rotation)
    {
        transform.rotation = Quaternion.Euler(0, 0, -90 * rotation);
    }
    public void SnapToGrid(RectTransform targetRect)
    {
        Vector2Int slotCoords = new Vector2Int(mPart.originX, mPart.originY);
        DataItemPlayer.main.ship.TryPlace(mPart, slotCoords.x, slotCoords.y);
        Rect rect = targetRect.rect;

        Vector2 localPoint = new Vector2(
            rect.xMin + (slotCoords.x + mPart.width / 2f) * cellSize * 2,
            rect.yMax - (slotCoords.y + mPart.height / 2f) * cellSize * 2
        );

        Vector3 worldPoint = targetRect.TransformPoint(localPoint);
        recttransform.position = worldPoint;
    }
}
