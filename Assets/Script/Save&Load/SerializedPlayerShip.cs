using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

[Serializable]
public class SerializedPlayerShip : SerializableData<DataItemShip>
{
    public string internalName;
    public List<SerializedPart> parts, stash;

    public int w, h;
    public BigInteger grid;
    public SerializedPlayerShip(DataItemShip data) : base(data)
    {
        internalName = data.scriptable.InternalName;
        parts = data.parts.Select(p => new SerializedPart(p)).ToList();
        stash = data.stash.Select(p => new SerializedPart(p)).ToList();

        w = data.width; h = data.height;
        grid = EncodeToBigInteger(data.mGrid);
    }
    public override DataItemShip Deserialize()
    {
        throw new NotImplementedException();
    }
    public static BigInteger EncodeToBigInteger(bool[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        BigInteger result = BigInteger.Zero;
        int bitIndex = 0;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (grid[r, c])
                    result |= (BigInteger.One << bitIndex);
                bitIndex++;
            }
        }

        return result;
    }

    public static bool[,] DecodeFromBigInteger(BigInteger value, int rows, int cols)
    {
        bool[,] grid = new bool[rows, cols];
        int bitIndex = 0;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                grid[r, c] = !(value & (BigInteger.One << bitIndex)).IsZero;
                bitIndex++;
            }
        }

        return grid;
    }
}
[Serializable]
public class SerializedPart : SerializableData<DataItemPart>
{
    public string internalName;
    public int x, y, r;
    public float cost;
    public SerializedPart(DataItemPart data) : base(data)
    {
        internalName = data.scriptable.InternalName;
        x = data.originX; y = data.originY; r = data.rotation;
        cost = data.purchaseCost;
    }
}
