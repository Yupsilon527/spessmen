
public class PlayerRacer : Racer
{
    public PlayerRacer( DataItemShip playerShip) :base(0)
    {
        foreach (var part in playerShip.parts)
        {
            modifiers.Add(new ModifierData(this,part.scriptable));
        }
    }
    public override string ToString()
    {
        return "Player Racer";
    }
}
