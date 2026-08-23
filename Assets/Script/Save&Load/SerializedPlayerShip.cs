using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

[Serializable]
public class SerializedPlayerShip : SerializableData<DataItemShip>
{
    public string internalName;
    public List<SerializedPart> parts, stash;
    public BoolGrid grid;
    public SerializedPlayerShip(DataItemShip data) : base(data)
    {
        internalName = data.scriptable.InternalName;
        parts = data.parts.Select(p => new SerializedPart(p)).ToList();
        stash = data.stash.Select(p => new SerializedPart(p)).ToList();

        grid = new BoolGrid();
        grid.Encode(data.mGrid);
    }
    public override DataItemShip Deserialize()
    {
        var orig = ResourceCache.main.LoadShip(internalName);
        if (orig != null)
        {
            var output = new DataItemShip(orig, false);
            output.mGrid = DataItemGrid.Translate(grid) ;

            foreach (var p in parts)
            {
                output.parts.Add(p.Deserialize());
            }
            foreach (var p in stash)
            {
                output.stash.Add(p.Deserialize());
            }
            return output;
        }
        throw new Exception($"No racer with name {internalName} has been found!");
    }
}
