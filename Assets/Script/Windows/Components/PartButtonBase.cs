using UnityEngine;
using UnityEngine.UI;

public class PartButtonBase : Initializable
{
    public float cellSize = 20;

    public DataItemPart _part;
    protected RectTransform recttransform;
    protected   GridPreview gridPreview;
    public Image sprite;
    public void FromPart(DataItemPart part, bool draw)
    {
        ClearToken(false);
        _part = part;
        if (draw)
            Redraw();
    }
    public void ClearToken(bool draw)
    {
        _part = null;
        if (draw)
            Redraw();
    }

    protected override void Initialize()
    {
        base.Initialize();
        FindComponent(ref recttransform);
        FindComponent(ref gridPreview);

    }
    protected void Redraw()
    {
        sprite.sprite = _part.scriptable.icon;
        gridPreview.Draw(_part._grid, _part.scriptable.grid.width, _part.scriptable.grid.height);
    }
    public void AdjustRotation(int rotation)
    {
        transform.rotation = Quaternion.Euler(0, 0, -90 * rotation);
    }
    public void SnapToGrid(RectTransform targetRect)
    {
        Vector2Int slotCoords = new Vector2Int(_part.originX, _part.originY);
        DataItemPlayer.main.ship.TryPlace(_part, slotCoords.x, slotCoords.y);
        Rect rect = targetRect.rect;

        Vector2 localPoint = new Vector2(
            rect.xMin + (slotCoords.x + _part.width / 2f) * cellSize * 2,
            rect.yMax - (slotCoords.y + _part.height / 2f) * cellSize * 2
        );

        Vector3 worldPoint = targetRect.TransformPoint(localPoint);
        recttransform.position = worldPoint;
    }
}
