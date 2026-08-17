
using System.Linq;

public class DataItemPart : DataItemGrid
{
    public bool deleted = false;
    public PartScriptable scriptable;
    public int rotation = 0;
    public int originX;
    public int originY;
    public float purchaseCost = 0;
    public DataItemPart(PartScriptable so, float purchaseCost)
    {
        Transform(so);
        this.purchaseCost = purchaseCost;
    }
    public void Transform(PartScriptable so)
    {
        scriptable = so;
        width = so.grid.width;
        height = so.grid.height;
        Encode(so.grid.ToOutputGrid());
    }
    public void Rotate(bool clockwise)
    {
        Rotate(rotation + (clockwise ? 1 : -1));
    }
    public void Rotate(int rot)
    {
        rotation = rot % 4;
    }
    public bool CanBeDiscarded()
    {
        return true;
    }
    public bool CanMerge(DataItemPart other)
    {
        return other.scriptable.combos.Any ( combo => combo.other == scriptable ) || scriptable.combos.Any(combo => combo.other == other.scriptable);
    }
    public PartScriptable GetMergeOutcome(DataItemPart other)
    {
        var found = other.scriptable.combos.FirstOrDefault(c => c.other == scriptable)
             ?? scriptable.combos.FirstOrDefault(c => c.other == other.scriptable);

        return found != null ? found.result : scriptable;
    }
    public override string ToString()
    {
        return scriptable.InternalName + "Data" ;
    }
}
