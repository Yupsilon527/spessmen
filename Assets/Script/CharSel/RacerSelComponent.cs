using UnityEngine;
using UnityEngine.UI;

public class RacerSelComponent : Initializable
{
    public ShipScriptable assigned;
    public Graphic[] colorGraphics;
    protected virtual void Start()
    {
        if (assigned != null )
        {
            AssignScriptable(assigned);
        }
    }
    public virtual void AssignScriptable(ShipScriptable ship)
    {
        assigned = ship;
        foreach (var g in colorGraphics)
        {
            g.color = ship.baseColor;
        }
    }
}