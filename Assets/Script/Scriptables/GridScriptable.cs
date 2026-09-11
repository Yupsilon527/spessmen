using UnityEngine;

public abstract class  GridScriptable : ScriptableBase
{
    public BoolGrid grid ;
}

public class UnlockCondition
{
    public bool lockedByDefault = false;
    public Variables.Condition[] conditions;
    public bool IsUnlocked()
    {
        return !lockedByDefault && PlayerConfig.main.globalScope.AllConditionsMet(conditions);
    }
}