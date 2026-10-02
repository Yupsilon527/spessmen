using System;

public abstract class  UnlockScriptable : ScriptableBase
{
    public UnlockCondition condition ;
    public virtual bool IsUnlocked()
    {
        return condition?.IsUnlocked() ?? true;
    }
}

[Serializable]
public class UnlockCondition
{
    public bool lockedByDefault = false;
    public Variables.Condition[] conditions;
    public bool IsUnlocked()
    {
        return !lockedByDefault && PlayerConfig.main.globalScope.AllConditionsMet(conditions);
    }
}