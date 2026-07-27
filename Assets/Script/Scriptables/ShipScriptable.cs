using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ship", menuName = "Data/Ship Data")]
public class ShipScriptable : GridScriptable
{
    private void OnValidate()
    {
        grid.width = 10;
        grid.height = 10;
        grid.ValidateAndRecreate();
    }

    public bool[,] GetOutputGrid() => grid.ToOutputGrid();

    public override DataItemGrid Translate()
    {
        throw new System.NotImplementedException();
    }
}
}
