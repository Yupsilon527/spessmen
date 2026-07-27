using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowManager : Initializable
{
    public List<Window> openWindows;
    public GameObject clearButton;
    protected override void Initialize()
    {
        base.Initialize();
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
        Canvas.ForceUpdateCanvases();
        InstallWindow(gob.GetComponent<Window>());
    }
    public void InstallWindow(Window window)
    {
        openWindows.Add(window);
        LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        clearButton?.SetActive(openWindows.Count > 0);
    }
    public void CloseAllWindows()
    {
        foreach (Window openWindow in openWindows.ToArray())
        {
            openWindow.Close();
        }
        openWindows.RemoveAll((Window match) => { return !match.IsOpen(); });
        clearButton?.SetActive(openWindows.Count > 0);
    }
    public void CloseWindow(string ID)
    {
        Window openWindow = FindWindow(ID, true);
        if (openWindow != null)
        {
            openWindow.Close();
        }
        clearButton?.SetActive(openWindows.Count > 0);
    }
    public Window MainOpenWindow()
    {
        if (openWindows.Count > 0)
            return openWindows[0];
        return null;
    }
}
