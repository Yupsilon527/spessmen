

using Variables;

public class DataItemPlayer : Initializable
{
    public static DataItemPlayer main;
    public DataItemShip car;

    public PlayerEconomyController econ;
    public PlayerChaosController score;
    public VariableScope scope;
    protected override void Initialize()
    {
            main = this;
        econ=GetComponent<PlayerEconomyController>();
        score = GetComponent<PlayerChaosController>();
        scope = new VariableScope();

        base.Initialize();
    }
    public void FromData(ShipScriptable s)
    {
        car = new(s);
        econ.Setup();
        score.Setup();

        econ.GiveGold(s.startingGold);
    }
    public float GetPropertySpeculative(ModifierDefines.Property property)
    {
        bool multiplicative = ModifierDefines.IsPropertyMultiplicative(property);
        float value = multiplicative ? 1 : 0;

        if (multiplicative)
            value *= car.scriptable.GetProperty(property, 1);
        else
            value += car.scriptable.GetProperty(property, 1);

        foreach (var part in car.parts)
        {
            if (multiplicative)
                value *= part.scriptable.GetProperty(property, 1);
            else
            value += part.scriptable.GetProperty(property,1);
        }

        return value;
    }
}
