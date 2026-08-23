using UnityEngine;

public class ViewManager : WindowManager
{
    public static ViewManager Instance { get; private set; }
    public TabComponent tabComponent;

    public RaceView race;
    public ShopView shop;
    public GameObject settingsMenu;

    public enum Views
    {
        shopView = 0,
        raceView = 1,
    }


    protected override void Initialize()
    {
        Instance = this;
        base.Initialize();
        CloseSettingsMenu();
    }

    public void ChangeView(Views view)
    {
        CloseSettingsMenu();
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
        if (shop.gameObject.activeSelf)
        {
            shop.OnOpened();
        }
        else
        {
            ChangeView(Views.shopView);
        }
    }
    public void OpenSettingsMenu()
    {
        settingsMenu?.gameObject?.SetActive(true);
    }
    public void CloseSettingsMenu()
    {
        settingsMenu?.gameObject?.SetActive(false);
    }
}
