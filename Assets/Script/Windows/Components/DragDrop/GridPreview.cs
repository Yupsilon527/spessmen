using UnityEngine;
using UnityEngine.UI;

public class GridPreview : MonoBehaviour
{
    public GridLayoutGroup gridLayout;
    public Image[] tile;
    public void Draw(bool[] shape, int width, int height)
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
                tile[index].enabled = shape[y * width + x];
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
}
