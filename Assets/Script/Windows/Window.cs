using UnityEngine;
using UnityEngine.UI;

public class Window : Initializable
{
    public bool unique = true;
    public bool IsOpen()
    {
        return gameObject.activeSelf;
    }
    public void Open()
    {
        if (!IsOpen())
        {
            if (unique)
                InterfaceManager.main.CloseAllWindows();
            InterfaceManager.main.OpenWindow(this);
            OnOpened();
        }
    }
    public void Close()
    {
        if (IsOpen())
        {
            gameObject.SetActive(false);
            OnClosed();
        }
    }
    protected virtual void OnOpened()
    {

    }
    protected virtual void OnClosed()
    {

    }
    protected virtual void LayoutRefresh()
    {
        LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        Canvas.ForceUpdateCanvases();
    }
}