public abstract class DataItemGrid
{
    public int width, height;
    public bool[,] mGrid;
    public void Encode(bool[] grid)
    {
        mGrid = Translate(grid, width, height);
    }
    public static bool[,] Translate(BoolGrid grid)
    {
        return Translate(grid.ToOutputGrid(), grid.width, grid.height);
    }
    public static bool[,] Translate(bool[] grid, int width, int height)
    {
        bool[,] output = new bool[width, height];

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                int dot = y * width + x;
                if (dot >= 0 && dot < grid.Length)
                {
                    output[x, y] = grid[dot];
                }
            }
        return output;
    }

    public bool IsInsideBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }
    public void SetValue(int x, int y, bool value)
    {
        if (IsInsideBounds(x, y))
            mGrid[x, y] = value;
    }
    public virtual bool[,] RetrieveRotated(int rotation = 0)
    {
        return Rotate(mGrid, rotation);
    }
    public static bool[,] Rotate(bool[,] grid, int rotation)
    {
        rotation = ((rotation % 4) + 4) % 4;
        bool[,] result = grid;

        for (int r = 0; r < rotation; r++)
            result = RotateOnce(result);

        return result;
    }

    private static bool[,] RotateOnce(bool[,] source)
    {
        int width = source.GetLength(0);
        int height = source.GetLength(1);
        bool[,] rotated = new bool[height, width];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                rotated[height - 1 - y, x] = source[x, y];

        return rotated;
    }
    public int CountTilesTotal()
    {
        int total = 0;
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                if (!mGrid[x, y])
                    total++;
            }
        return total;
    }
    public bool ColumnUsed(int x)
    {
        if (!IsInsideBounds(x, 0)) return true;
        for (int checkY = 0; checkY < height; checkY++)
        {
            if (mGrid[x, checkY]) return false;
        }
        return true;
    }
    public bool RowUsed(int y)
    {
        if (!IsInsideBounds(0, y)) return true;
        for (int checkX = 0 + 1; checkX < width; checkX++)
        {
            if (mGrid[checkX, y]) return false;
        }
        return true;
    }
    public bool IsTopmost(int y)
    {
        return RowUsed(y - 1);
    }

    public bool IsBottommost(int y)
    {
        return RowUsed(y + 1);
    }

    public bool IsLeftmost(int x)
    {
        return ColumnUsed(x - 1);
    }

    public bool IsRightmost(int x)
    {
        return ColumnUsed(x + 1);
    }

}
