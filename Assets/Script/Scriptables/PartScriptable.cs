using UnityEngine;
[CreateAssetMenu(fileName = "Component", menuName = "Data/Component Data")]
public class PartScriptable : ModifierScriptable
{
    public PartAbility ability;
    public Sprite icon;
    public ItemDefines.BoonRarity boonRarity;
    public ItemDefines.PartType partType;
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
        ability.InternalName = name;
        ability.classification = partType;
    }
    public virtual bool IsUnlocked()
    {
        return !lockedForSomeReason;
    }

}
