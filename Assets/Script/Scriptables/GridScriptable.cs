using UnityEngine;

public abstract class  GridScriptable : ScriptableBase
{
    public BoolGrid grid ;
    public abstract DataItemGrid Translate();
}
[System.Serializable]
public class BoolGrid
{
    public int width;
    public int height;
    public bool[] cells;

    public bool Get(int x, int y) => cells[y * width + x];
    public void Set(int x, int y, bool value) => cells[y * width + x] = value;

    public bool[] ToOutputGrid()
    {
        bool[] output = new bool[width * height];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                output[x*height + y] = Get(x, y);
        return output;
    }

    public void ValidateAndRecreate()
    {
        int required = width * height;

        if (cells != null && cells.Length == required)
            return; // already valid, nothing to do

        bool[] oldCells = cells;
        int oldWidth = oldCells != null && width > 0 ? (oldCells.Length / Mathf.Max(1, height == 0 ? 1 : height)) : 0;

        cells = new bool[required];

        if (oldCells != null)
        {
            int copyLength = Mathf.Min(oldCells.Length, cells.Length);
            System.Array.Copy(oldCells, cells, copyLength);
        }
    }
}