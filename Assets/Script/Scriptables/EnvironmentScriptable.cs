using UnityEngine;

[CreateAssetMenu(fileName = "Component", menuName = "Data/Enviroment Data")]
public class EnvironmentScriptable : ScriptableBase
{
    public Sprite icon, background;
    public Sprite[] weatherIcons;
    public WeightSprite[] groundSprites, frontSprites, backSprites, farSprites;
    public Color[] bgColors;

    protected override void OnValidate()
    {
        base.OnValidate();
        if (weatherIcons.Length == 0)
        {
            weatherIcons = new Sprite[(int)RaceDefines.RaceModifiers.Total];
        }
    }
}

[System.Serializable]
public class WeightSprite : WeightList.WeightItem<Sprite>
{
    public WeightSprite(Sprite value, float weight) : base(value, weight)
    {
        this.value = value;
    }
}
