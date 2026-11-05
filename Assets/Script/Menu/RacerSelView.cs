

using UnityEngine.UI;

public class RacerSelView : RacerSelComponent
{
    public static RacerSelView main;
    public RacerSelDescription description;
    public CharSelPreview preview;
    public Button proceedButton;
    protected override void Initialize()
    {
        base.Initialize();
        main = this;
    }
    protected override void Start()
    {
        AssignScriptable(PlayerConfig.main.playerCharacter);
    }
    public override void AssignScriptable(ShipScriptable ship)
    {
        base.AssignScriptable(ship);
        preview?.AssignScriptable(ship);
        description?.AssignScriptable(ship);
        if (proceedButton!=null)
        {
            proceedButton.interactable = assigned.IsUnlocked();
        }
    }

    public  void StartTheGame()
    {
        if (assigned.IsUnlocked())
        SceneTransitionManager.main.TransitionGameScene("Race Scene", () => PlayerConfig.main.StartNewGame());
    }
}
