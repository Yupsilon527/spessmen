

using System.Collections;
using System.Collections.Generic;
using Variables;

public class DataItemPlayer : Initializable
{
    public static DataItemPlayer main;
    public DataItemShip car;

    public PlayerEconomyController econ;
    public PlayerShopController shop;
    public PlayerChaosController score;
    public VariableScope scope;
    protected override void Initialize()
    {
            main = this;
        econ=GetComponent<PlayerEconomyController>();
        score = GetComponent<PlayerChaosController>();
        shop = GetComponent<PlayerShopController>();
        scope = new VariableScope();

        base.Initialize();
    }
    public void FromData(ShipScriptable s)
    {
        car = new(s,true);
        econ.Setup();
        score.Setup();
        shop.Setup();

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
    public void TriggerEventOutOfRace(ShipDefines.PartEvent evt, bool includeStash)
    {
        var racer = TourneyController.main.GetPlayerRacer();
        OutOfRaceEvent(evt, racer, car.parts);
        if (includeStash)
        {
            OutOfRaceEvent(evt, racer, car.stash );
        }
    }
    public void OutOfRaceEvent(ShipDefines.PartEvent evt, Racer racer,  DataItemPart part)
    {
            foreach (var a in part.scriptable.abilities)
            {
                if (a.function != evt) continue;
                foreach (var e in a.actions)
                {
                    e.AffectOnRacer(racer, racer, null, 1, true);
                }
            }
        }
    void OutOfRaceEvent(ShipDefines.PartEvent evt, Racer racer, IEnumerable< DataItemPart> parts)
    {
        foreach (var part in parts)
        {
            OutOfRaceEvent(evt, racer, part);
        }
    }
}
