using UnityEngine;

[CreateAssetMenu(fileName = "Component", menuName = "Data/Enviroment Data")]
public class EnvironmentScriptable : ScriptableBase
{
    public Sprite icon, background;
    public Sprite[] weatherIcons, groundSprites, frontSprites, backSprites, farSprites;

    protected override void OnValidate()
    {
        base.OnValidate();
        if (weatherIcons.Length == 0)
        {
            weatherIcons = new Sprite[(int)RaceDefines.RaceModifiers.Total];
        }
    }
}
