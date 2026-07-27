using UnityEngine;

public class ViewManager : WindowManager
{
    public static ViewManager Instance { get; private set; }
    public TabComponent tabComponent;

    public BuildView build;
    public ShopView shop;

    public enum Views
    {
        homeView = 0,
        mapView = 0,
        combatView = 1,
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
            case Views.mapView:
                break;
            case Views.combatView:
                break;
        }
    }

    public void OnNewGameBegin()
    {
        ChangeView(Views.mapView);
    }
    public void OpenBuildingButton()
    {

    }
}
