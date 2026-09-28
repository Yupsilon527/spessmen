using UnityEngine;
using UnityEngine.UI;

public class ViewManager : WindowManager
{
    public static ViewManager Instance { get; private set; }
    public TabComponent tabComponent;

    public RaceView race;
    public ShopView shop;
    public GameObject settingsMenu;
    public GameObject confirmExitMenu;
    public enum Views
    {
        shopView = 0,
        raceView = 1,
    }


    protected override void Initialize()
    {
        Instance = this;
        base.Initialize();
        CloseAux();
    }

    public void ChangeView(Views view)
    {
        CloseAux();
        race?.preview?.Clear();
        switch (view)
        {
            case Views.shopView:
                tabComponent.OpenTab(shop.gameObject);
                shop.OnOpened();
                break;
            case Views.raceView:
                tabComponent.OpenTab(race.gameObject);
                race.OnOpened();
                break;
        }
    }

    public void OnNewGameBegin()
    {
        CloseAux();
        if (shop.gameObject.activeSelf)
        {
            shop.OnOpened();
        }
        else
        {
            ChangeView(Views.shopView);
        }
    }
    public void CloseAux()
    {
        CloseSettingsMenu();
        CloseConfirmMenu();
    }
    public void OpenSettingsMenu()
    {
        settingsMenu?.gameObject?.SetActive(true);
    }
    public void CloseSettingsMenu()
    {
        settingsMenu?.gameObject?.SetActive(false);
    }
    public void OpenConfirmMenu()
    {
        confirmExitMenu?.gameObject?.SetActive(true);
    }
    public void CloseConfirmMenu()
    {
        confirmExitMenu?.gameObject?.SetActive(false);
    }
}
