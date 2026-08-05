
using System;
using UnityEngine;
using Variables;

[Serializable]
public class PlayerConfig : Initializable
{
    public string saveKey = "variables";
    public ShipScriptable playerCharacter;
    public VariableScope globalScope;
    public static PlayerConfig main;

    protected override void Initialize()
    {
        if (main == null)
        {
            main = this;
            globalScope = new();
            LoadData();

            base.Initialize();
            GameObject.DontDestroyOnLoad(gameObject);
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }
    #region Save
    private void OnApplicationQuit()
    {
        SaveData();
    }
    public void SaveData()
    {
        if (!initialized) return;
        Inspect("Saving " + name);

        string heroSerialized = JsonUtility.ToJson(new VariableScopeSerializable(globalScope));
        Inspect($"Serialized heroes {heroSerialized}");
        Inspect(heroSerialized);
        PlayerPrefs.SetString(saveKey, heroSerialized);
        PlayerPrefs.Save();
    }

    public bool LoadData()
    {
        Inspect("Loading data from save...");

        Inspect($"Deserialzie {saveKey}");
        if (PlayerPrefs.HasKey(saveKey))
        {
            string serialized = PlayerPrefs.GetString(saveKey);
            Inspect("Deserialize data " + serialized);

            VariableScopeSerializable heroData = JsonUtility.FromJson<VariableScopeSerializable>(serialized);
            if (heroData != null)
            {
                heroData.Deserialize(globalScope);
                return true;
            }
        }

        return false;
    }
    public void EraseData()
    {
        PlayerPrefs.DeleteAll();
    }
    #endregion
    private void Start()
    {
        StartNewGame();
    }
    public void StartNewGame()
    {
        DataItemPlayer.main.FromData(playerCharacter);
        TourneyController.main.FreshStart();
        ViewManager.Instance.OnNewGameBegin();
    }
}
