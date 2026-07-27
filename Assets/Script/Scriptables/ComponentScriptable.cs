using UnityEngine;
[CreateAssetMenu(fileName = "Component", menuName = "Data/Component Data")]
public class ComponentScriptable : GridScriptable
{
    public int size = 3;
    protected override void OnValidate()
    {
        base.OnValidate();
        grid.width = size;
        grid.height = size;
        grid.ValidateAndRecreate();
    }
    public override DataItemGrid Translate()
    {
        throw new System.NotImplementedException();
    }
}
