using UnityEngine;

public class ViewBase : Initializable
{
    protected override void OnEnable()
    {
        if (initialized) OnOpened();
        base.OnEnable();
    }
    public virtual void OnOpened()
    {

    }
}
