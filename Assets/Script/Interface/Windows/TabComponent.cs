using UnityEngine;
using UnityEngine.UI;

    [System.Serializable]
    public class Tab
    {
        public Button btn;
        public GameObject gameObject;

        public void Open()
        {
        if (btn!=null)
            btn.interactable = false;
            gameObject.gameObject.SetActive(true);
        }
        public void Close()
    {
        if (btn != null)
            btn.interactable = true;
            gameObject.gameObject.SetActive(false);
        }
    }

public class TabComponent  : MonoBehaviour
{
    public GameObject defaultTab;
    private void Start()
    {
        OpenTab(defaultTab);
    }
    public Tab[] Tabs;
    public void OpenTab(GameObject tab)
    {
        if (tab == null) return;
        for (int i = 0; i < Tabs.Length; i++)
        {
            if (Tabs[i].gameObject == tab)
            {
                OpenTab(i);
                return;
            }
        }
    }
    void OpenTab(int val)
    {
        CloseAllTabs();
        if (val >= 0 && val < Tabs.Length)
        {
            Tabs[val].Open() ;
        }
    }
    public void CloseAllTabs()
    {
        foreach (Tab tab in Tabs)
        {
            tab.Close();
        }
    }
}
