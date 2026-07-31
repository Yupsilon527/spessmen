
public class DataItemPlayer : Initializable
{
    public static DataItemPlayer main;
    public DataItemShip ship;

    public PlayerEconomyController econ;
    public PlayerChaosController score;
    protected override void Initialize()
    {
            main = this;
        econ=GetComponent<PlayerEconomyController>();
        score = GetComponent<PlayerChaosController>();

        econ.Setup();
        score.Setup();
        base.Initialize();
    }
    public void FromData(ShipScriptable s)
    {
        ship = new(s);
    }
    public float GetPropertySpeculative(ModifierDefines.Property property)
    {
        bool multi = ModifierDefines.IsPropertyMultiplicative(property);
        float value = multi ? 1 : 0;

        foreach (var part in ship.parts)
        {
            value *= part.scriptable.GetProperty(property,1);
        }

        return value;
    }
}
