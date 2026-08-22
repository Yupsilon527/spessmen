public class ChangeSceneButton : ButtonBase
    {
        public string SceneToChange;
    public override void OnPressed()
    {

            if (SceneTransitionManager.main != null)
            {
                SceneTransitionManager.main.TransitionScene(SceneToChange);
            }
        }
    }

