using UnityEngine;

public class ViewManager : WindowManager
{
    public static ViewManager Instance { get; private set; }
    public TabComponent tabComponent;

    public RaceView build;
    public ShopView shop;

    public enum Views
    {
        shopView = 0,
        raceView = 1,
    }


    protected override void Initialize()
    {
        Instance = this;
        base.Initialize();
    }

    public void ChangeView(Views view)
    {
        switch (view)
        {
            case Views.shopView:
                tabComponent.OpenTab(shop.gameObject);
                break;
            case Views.raceView:
                tabComponent.OpenTab(build.gameObject);
                break;
        }
    }

    public void OnNewGameBegin()
    {
        ChangeView(Views.shopView);
    }
}
