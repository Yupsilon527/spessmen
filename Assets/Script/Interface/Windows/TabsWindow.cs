using System;
using UnityEngine;
using UnityEngine.UI;

public class TabWindow : Window
{
    [Serializable]
    public class WindowTab
    {
        public Button button;
        public GameObject parent;
    }

    public GameObject defaultTab;
    public WindowTab[] tabs;
    protected override void OnOpened()
    {
        base.OnOpened();
        OpenTabGameObject(defaultTab);
    }
    public void OpenTab(int tab)
    {
        OpenTab(tabs[tab]);
    }
    public void OpenTabGameObject(GameObject find)
    {
        foreach (var tab in tabs)
        {
            if (tab.parent == find)
            {
                OpenTab(tab);
            }
        }
    }
    public void OpenTab(WindowTab tab)
    {
        CloseAllTabs();
        if (tab.button != null) tab.button.interactable = false;
        tab.parent.SetActive(true);
    }
    public void CloseTab(WindowTab tab)
    {
        Debug.LogWarning("Closing the tab: " + tab.parent.name);
        if (tab.button != null) tab.button.interactable = true;
        tab.parent.SetActive(false);
    }
    public void CloseAllTabs()
    {
        foreach (WindowTab tab in tabs)
        {
            CloseTab(tab);
        }
    }
}