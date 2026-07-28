using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataItemGrid 
{
    public int width, height;
    public int value;
    public int Encode(bool[] grid)
    {
        int result = 0;
        for (int i = 0; i < width; i++)
        {
            if (grid[i])
                result |= (1 << i);
        }
        return result;
    }
    public virtual bool[,] Decode()
    {
        return Decode(value, width, height);
    }
    public static bool[,] Decode(int value, int width, int height, int rotation = 0)
    {
        bool[,] grid = new bool[width, height];
        int bit = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                grid[x, y] = (value & (1 << bit)) != 0;
                bit++;
            }
        }

        return Rotate(grid, rotation);
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
