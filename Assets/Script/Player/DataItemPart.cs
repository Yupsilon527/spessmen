
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
        scriptable = so;
        width = so.grid.width;
        height = so.grid.height;
        Encode(so.grid.ToOutputGrid());
        this.purchaseCost = purchaseCost;
    }
    public PartAbility GetAbility()
    {
        return scriptable.ability;
    }
    public void Rotate(bool clockwise)
    {
        rotation = (rotation + (clockwise ? 1 : -1)) % 4;
    }
    public bool CanBeDiscarded()
    {
        return true;
    }
}
