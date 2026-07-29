using UnityEngine;

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
}
