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
        x = data.originX; y = data.originY; r = data.rotation;
        cost = data.purchaseCost;
    }

    public override DataItemPart Deserialize()
    {
        var zxc = ResourceCache.main.LoadComponent(internalName);
        if (zxc != null)
        {
            var output = new DataItemPart(zxc, cost);
            output.originX = x; output.originY = y;
            output.rotation = r;
            return output;
        }
        throw new Exception($"No part with name {internalName} has been found!");
    }
}
