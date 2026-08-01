
using System.Linq;

public class DataItemPart : DataItemGrid
{
    public PartScriptable scriptable;
    public int rotation = 0;
    public int originX;
    public int originY;
    public Ability correspondingAbility;
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
        rotation = (rotation + (clockwise ? 1 : -1)) % 4;
    }
    public bool CanBeDiscarded()
    {
        return true;
    }
    public bool CanMerge(DataItemPart other)
    {
        return other.scriptable.combos.Any ( combo => combo.other == other.scriptable ) || scriptable.combos.Any(combo => combo.other == scriptable);
    }
    public PartScriptable GetMergeOutcome(DataItemPart other)
    {
        var found = other.scriptable.combos.FirstOrDefault(c => c.other == scriptable)
             ?? scriptable.combos.FirstOrDefault(c => c.other == other.scriptable);

        return found != null ? found.result : scriptable;
    }
}
