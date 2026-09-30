using UnityEngine;

public class Logo : MonoBehaviour
{
   static bool skippable = false;
   public void OnAnimationEnd()
    {
        gameObject.SetActive(false);
        skippable = true;
    }
    public void TrySkip()
    {
        if (skippable)
            gameObject.SetActive(false);

    }
}
