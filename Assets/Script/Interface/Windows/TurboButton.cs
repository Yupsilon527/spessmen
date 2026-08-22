using UnityEngine;

public class TurboButton : Initializable
{
    public float turboSpeed = 2;
    bool isTurbo = false;
    public void ToggleTurbo()
    {
        SetTurbo(!isTurbo);
    }
    public void SetTurbo(bool value)
    {
        if (value == isTurbo) return;
        isTurbo = value;
        Time.timeScale *= value ? turboSpeed : (1/turboSpeed);
#if UNITY_EDITOR
        Time.timeScale *= value ? 10f : .1f;
#endif
    }
    private void OnDisable()
    {
        if (isTurbo)
        {
            SetTurbo(false);
        }
    }
}
