using UnityEngine;

public class ContinueRunButton : ButtonBase
{
    private void Start()
    {
        buttonComponent.interactable = PlayerConfig.main?.HasRun() ?? false;
    }
    public override void OnPressed()
    {
        if (PlayerConfig.main?.HasRun() ?? false)
        SceneTransitionManager.main.TransitionGameScene("Race Scene", () => PlayerConfig.main.StartNewGameFromSaveData());
    }

}
