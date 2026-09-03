using System;

[Serializable]
public class SerializedPart : SerializableData<DataItemPart>
{
    public string internalName;
    public int x, y, r;
    public float cost;
    public SerializedPart(DataItemPart data) : base(data)
    {
        internalName = data.scriptable.InternalName;
        x = data.originX; y = data.originY; r = data.CanBeRotated() ?  data.rotation : 0;
        cost = data.purchaseCost;
    }

    public override DataItemPart Deserialize()
    {
        var part = ResourceCache.main.LoadComponent(internalName);
        if (part != null)
        {
            var output = new DataItemPart(part, cost);
            output.originX = x; output.originY = y;
            output.rotation = output.CanBeRotated() ? r : 0;
            return output;
        }
        throw new Exception($"No part with name {internalName} has been found!");
    }
}
