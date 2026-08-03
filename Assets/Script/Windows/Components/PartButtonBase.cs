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
    public int rotation = 0;
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
    public void Rotate(bool clockwise)
    {
        // mPart.Rotate(clockwise);
        Rotate(rotation + (clockwise ? 1 : -1));
        AdjustRotation(rotation);
    }
    public void Rotate(int rot)
    {
        // mPart.Rotate(clockwise);
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
        DataItemPlayer.main.car.TryPlace(mPart, slotCoords.x, slotCoords.y,rotation);
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
