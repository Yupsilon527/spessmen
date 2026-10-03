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
        chaos = data.score.playerChaos;
        car = new (data.car);

        numRerolls = data.shop.numRerolls;
        shop= data.shop.itemActions.Select(p =>  new SerializedPurchaseData(p)).ToList();

        string a = "item_purchased_";
        string b = "item_seen_";
        foreach (var kvp in data.scope.scope)
        {
            if ((kvp.Key.Length > a.Length && kvp.Key.Substring(0, a.Length) == a)|| (kvp.Key.Length > b.Length && kvp.Key.Substring(0, b.Length) == b))
            {
                PlayerConfig.main.globalScope.GetVariable(kvp.Key).Change(Change.Case.add, kvp.Value.GetFloatValue());
                kvp.Value.SetFloatValue(0);
            }
        }

        scope = new(data.scope);
    }
    public override void Deserialize(DataItemPlayer output)
    {
        output.econ.Setup();
        output.econ.gold.SetValue(gold);
        output.econ.bank.SetValue(bank);
        output.score.playerChaos = chaos;
        scope.Deserialize(output.scope);

        output.car = car.Deserialize();

        output.shop.numRerolls = numRerolls;
        output.shop.itemActions = shop.Select(p => p.Deserialize()).ToList();
    }
}

[Serializable]
public class SerializedPurchaseData : SerializableData<PurchaseData>
{
    public string part;
    public float  discount;
    public bool purchased;
    public bool locked;

    public SerializedPurchaseData(PurchaseData data) : base(data)
    {
        part = data.scriptable.InternalName;
        discount = data.discount;
        purchased = data.wasPurchased;
       locked = data.playerLocked;
    }
    public override PurchaseData Deserialize()
    {
        var scr = ResourceCache.main.LoadComponent(part);
        PurchaseData output = new PurchaseData(scr, discount);
        output.wasPurchased = purchased;
        output.playerLocked = locked;
        return output;
    }
}