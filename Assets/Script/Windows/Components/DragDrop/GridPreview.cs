using UnityEngine;
using UnityEngine.UI;

public class GridPreview : MonoBehaviour
{
    public GridLayoutGroup gridLayout;
    public Image[] tile;
    public void Draw(bool[,] shape, int width, int height)
    {
        Clear();
        gridLayout.constraintCount = width;

        if (tile.Length < width * height)
        {
            Debug.LogWarning($"GridPreview: tile array length ({tile.Length}) does not match shape size ({width * height}).");
            return;
        }

        int index = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                tile[index].gameObject.SetActive(true);
                tile[index].enabled = shape[x ,y];
                index++;
            }
        }
    }

    public void Clear()
    {
        foreach (var t in tile)
        {
            t.gameObject.SetActive(false);
        }
    }
    public Vector2Int GetGridPosition(Vector2 screenPos)
    {
        RectTransform recttransform = GetComponent<RectTransform>();
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
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return null;
        return canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
    }
}
