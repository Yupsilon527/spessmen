using UnityEngine;
[CreateAssetMenu(fileName = "Component", menuName = "Data/Component Data")]
public class PartScriptable : GridScriptable
{
    public Sprite icon;
    public ItemDefines.BoonRarity boonRarity;
    public float boonValue = -1;
    public bool unique = false;
    public bool lockedForSomeReason = false;
    protected override void OnValidate()
    {
        base.OnValidate();
        grid.ValidateAndRecreate();
        if (boonValue < 0)
        {
            boonValue = 20 * Mathf.Pow(2, (int)boonRarity);
        }
    }
    public override DataItemGrid Translate()
    {
        DataItemPart outputPart = new DataItemPart(this);
        return outputPart;
    }
    public virtual bool IsUnlocked()
    {
        return !lockedForSomeReason;
    }
}
