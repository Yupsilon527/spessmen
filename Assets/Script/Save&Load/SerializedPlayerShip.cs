using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SerializedPlayerShip : SerializableData<DataItemShip>
{
    public string internalName;
    public List<SerializedPart> parts ,stash;
    public SerializedPlayerShip(DataItemShip data) : base(data)
    {
        internalName = data.scriptable.InternalName;
        parts = data.parts.Select(p => new SerializedPart(p)).ToList();
        stash = data.stash.Select(p => new SerializedPart(p)).ToList();
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
