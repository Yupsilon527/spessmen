using UnityEngine;

public class Window : MonoBehaviour
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
                WindowManager.main.CloseAllWindows();
            WindowManager.main.OpenWindow(this);
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
}