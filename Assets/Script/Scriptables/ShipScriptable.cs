using UnityEngine;

[CreateAssetMenu(fileName = "Ship", menuName = "Data/Ship Data")]
public class ShipScriptable : ModifierScriptable
{
    public Sprite blueprint;
    public PartScriptable[] startingParts;
    protected override void OnValidate()
    {
        base.OnValidate();
        grid.width = ShipDefines.shipSize;
        grid.height = ShipDefines.shipSize;
        grid.ValidateAndRecreate();
    }

}
