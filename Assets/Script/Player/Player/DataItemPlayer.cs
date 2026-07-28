using UnityEngine;

public class DataItemPlayer : MonoBehaviour
{
    public static DataItemPlayer main;
    public ShipScriptable ship;

    public PlayerEconomyController econ;
    public PlayerChaosController score;
    private void Awake()
    {
            main = this;
        econ=GetComponent<PlayerEconomyController>();
        score = GetComponent<PlayerChaosController>();

        econ.Setup();
        score.Setup();
    }
    public void FromData(ShipScriptable s)
    {
        ship = s;
    }
}
