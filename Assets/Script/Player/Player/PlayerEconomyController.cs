using UnityEngine;

public class PlayerEconomyController : PlayerComponent
{
    public Resource gold;
    public Resource bank;
    public override void Setup()
    {
        base.Setup();
        gold = new ResourceInt(0, "gold", false, false);
        bank = new ResourceInt(0, "bank", false, false);
    }

    public void GiveGold(float amount)
    {
       // if (playerBank.GetValue() > 0)
       //     player.TextEffect("X2", Color.green);
        amount += bank.SubstractedValue(amount);
        gold.GiveValue(amount);
    }

}
