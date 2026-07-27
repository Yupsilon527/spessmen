using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataItemGrid 
{
    public int grid;
    public int Encode(bool[] grid)
    {
        int result = 0;
        for (int i = 0; i < 10; i++)
        {
            if (grid[i])
                result |= (1 << i);
        }
        return result;
    }
    public static bool[] Decode(int value, int length)
    {
        bool[] grid = new bool[length];
        for (int i = 0; i < length; i++)
        {
            grid[i] = (value & (1 << i)) != 0;
        }
        return grid;
    }
}
