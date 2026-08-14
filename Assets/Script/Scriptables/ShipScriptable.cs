using UnityEngine;

[CreateAssetMenu(fileName = "Racer", menuName = "Data/Racer Data")]
public class ShipScriptable : ModifierScriptable
{
    public Color baseColor;
    public Sprite portrait, blueprint;
    public GameObject prefab;
    public float startingGold = 30;
    public PartScriptable[] startingParts;
    protected override void OnValidate()
    {
        base.OnValidate();
        grid.width = ShipDefines.shipSize;
        grid.height = ShipDefines.shipSize;
        grid.ValidateAndRecreate();
    }

}
