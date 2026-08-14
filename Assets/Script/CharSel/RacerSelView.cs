
using System.Collections;
using UnityEngine.SceneManagement;

public class RacerSelView : RacerSelComponent
{
    public static RacerSelView main;
    public RacerSelDescription description;
    public CharSelPreview preview;

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
        preview.AssignScriptable(ship);
        description.AssignScriptable(ship);
    }

    public  void StartTheGame()
    {
        PlayerConfig.main.StartCoroutine(NewGameCoroutine());
    }
    IEnumerator NewGameCoroutine()
    {
        yield return SceneManager.LoadSceneAsync("Race Scene", LoadSceneMode.Single);
        yield return null;
        PlayerConfig.main.StartNewGame();
    }
}
