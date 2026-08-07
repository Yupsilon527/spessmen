using UnityEngine;

[CreateAssetMenu(fileName = "Component", menuName = "Data/Enviroment Data")]
public class EnvironmentScriptable : ScriptableBase
{
    public Sprite icon;
    public Sprite[] weatherIcons;

    protected override void OnValidate()
    {
        base.OnValidate();
        if (weatherIcons.Length == 0)
        {
            weatherIcons = new Sprite[(int)RaceDefines.RaceModifiers.Total];
        }
    }
}
