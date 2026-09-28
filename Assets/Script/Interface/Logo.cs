using UnityEngine;

public class Logo : MonoBehaviour
{
   public void OnAnimationEnd()
    {
        gameObject.SetActive(false);
    }
}
