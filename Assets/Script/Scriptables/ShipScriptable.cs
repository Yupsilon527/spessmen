using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ship", menuName = "Data/Ship Data")]
public class ShipScriptable : GridScriptable
{
    protected override void OnValidate()
    {
        base.OnValidate();
        grid.width = ShipDefines.shipSize;
        grid.height = ShipDefines.shipSize;
        grid.ValidateAndRecreate();
    }

    public override DataItemGrid Translate()
    {
        DataItemShip output = new();

        output.Encode(grid.ToOutputGrid());
        return output;
    }
}
