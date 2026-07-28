
public abstract class DataItemGrid 
{
    public int width, height;
    public bool[,] _grid;
    public void Encode(bool[] grid)
    {
        _grid = new bool[width,height];

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                int dot = y * width + x;
                if (dot>=0 && dot < grid.Length)
                {
                    _grid[x,y] = grid[dot];
                }
            }
    }
    public virtual bool[,] RetrieveRotated( int rotation = 0)
    {
        return Rotate(_grid, rotation);
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

}
