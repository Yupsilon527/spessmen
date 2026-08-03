

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
        bool multiplicative = ModifierDefines.IsPropertyMultiplicative(property);
        float value = multiplicative ? 1 : 0;

        if (multiplicative)
            value *= ship.scriptable.GetProperty(property, 1);
        else
            value += ship.scriptable.GetProperty(property, 1);

        foreach (var part in ship.parts)
        {
            if (multiplicative)
                value *= part.scriptable.GetProperty(property, 1);
            else
            value += part.scriptable.GetProperty(property,1);
        }

        return value;
    }
}
