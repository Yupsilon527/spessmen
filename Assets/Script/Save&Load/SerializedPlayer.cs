using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Variables;

[Serializable]
public class SerializedPlayer : SerializableData<DataItemPlayer>
{
    public float gold, bank, chaos;
    public SerializedPlayerShip car;
    public VariableScopeSerializable scope;

    public int numRerolls;
    public List<SerializedPurchaseData> shop;
    public SerializedPlayer(DataItemPlayer data) : base(data)
    {
        gold = data.econ.gold.GetValue();
        bank = data.econ.bank.GetValue();
        chaos = data.score.playerChaos ;
        scope = new (data.scope);
        car = new (data.car);

        numRerolls = data.shop.numRerolls;
        shop= data.shop.itemActions.Select(p =>  new SerializedPurchaseData(p)).ToList();
    }
}

[Serializable]
public class SerializedPurchaseData : SerializableData<PurchaseData>
{
    public string part;
    public float purchaseCost, discount;
    public bool purchased;

    public SerializedPurchaseData(PurchaseData data) : base(data)
    {
        part = data.part.InternalName;
        purchaseCost = data.purchaseCost;
        discount = data.discount;
        purchased = data.wasPurchased;
}
}