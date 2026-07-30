
public class DataItemPart : DataItemGrid
{
    public PartScriptable scriptable;
    public int rotation = 0;
    public int originX;
    public int originY;
    public Ability correspondingAbility;

    public DataItemPart(PartScriptable so)
    {
        scriptable = so;
        width = so.grid.width;
        height = so.grid.height;
        Encode(so.grid.ToOutputGrid());
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
    public bool HasModifier()
    {
        return scriptable.states.Length > 0 ||scriptable.properties.Length > 0 ||scriptable.relative.Length>0;
    }
    public Modifier GetInnateModifier(Racer racer)
    {
        return new Modifier(racer, scriptable);
    }
}
