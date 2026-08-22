using UnityEngine;
using UnityEngine.UI;

public abstract class ButtonBase : Initializable
{
    public Button buttonComponent;
    public virtual void Refresh() { }
    public abstract void OnPressed();
    protected virtual void Reset()
    {
        if (buttonComponent == null)
        {
            FindComponent(ref buttonComponent);
            if (buttonComponent != null)
            {
                buttonComponent.onClick.RemoveListener(OnPressed);
                buttonComponent.onClick.AddListener(OnPressed);
            }
        }
    }
}
