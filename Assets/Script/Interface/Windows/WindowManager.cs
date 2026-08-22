using System.Collections.Generic;
using UnityEngine;

public class WindowManager : Initializable
{
    public static WindowManager main;
    public List<Window> openWindows;
    protected override void Initialize()
    {
        base.Initialize();
        main = this;
        openWindows = new List<Window>();
        foreach (var Window in GetComponentsInChildren<Window>())
        {
            Window.Close();
        }
    }
    public bool IsMouseOverWindows()
    {
        foreach (Window openWindow in openWindows)
        {
            if (openWindow.isActiveAndEnabled && RectTransformUtility.RectangleContainsScreenPoint(openWindow.GetComponent<RectTransform>(), Input.mousePosition))
            {
                return true;
            }
        }
        return false;
    }
    public bool AreThereOpenWindows()
    {
        foreach (Window openWindow in openWindows)
        {
            if (openWindow.isActiveAndEnabled)
            {
                return true;
            }
        }
        return false;
    }
    public Window FindWindow(string ID, bool open)
    {
        foreach (Window openWindow in openWindows)
        {
            if (openWindow.name == ID && (open || !openWindow.isActiveAndEnabled))
                return openWindow;
        }
        return null;
    }
    public void OpenWindow(Window win)
    {
        if (win == null)
            return;

        win.gameObject.SetActive(true);
        InstallWindow(win.gameObject);
    }
    public void InstallWindow(GameObject gob)
    {
        openWindows.Add(gob.GetComponent<Window>());
    }
    public void InstallWindow(Window window)
    {
        openWindows.Add(window);
    }
    public void CloseAllWindows()
    {
        foreach (Window openWindow in openWindows.ToArray())
        {
            openWindow.Close();
        }
        openWindows.RemoveAll((Window match) => { return !match.IsOpen(); });
    }
    public void CloseWindow(string ID)
    {
        Window openWindow = FindWindow(ID, true);
        if (openWindow != null)
        {
            openWindow.Close();
        }
    }
    public Window MainOpenWindow()
    {
        if (openWindows.Count > 0)
            return openWindows[0];
        return null;
    }
}