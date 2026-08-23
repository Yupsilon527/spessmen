
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
    SerialziedTourney ser;

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

        SaveScope();
        SaveRun();
        PlayerPrefs.SetInt(saveKey, 1);

        PlayerPrefs.Save();
    }
    void SaveScope()
    {
        string globalScope = JsonUtility.ToJson(new VariableScopeSerializable(this.globalScope));
        Inspect($"Serialized playerScope {globalScope}");
        Inspect(globalScope);
        PlayerPrefs.SetString(saveKey + "-vars", globalScope);
    }
    void SaveRun()
    {
        if (TourneyController.main != null)
        {  
        ser = new SerialziedTourney(TourneyController.main);
            string tourneyString = JsonUtility.ToJson(ser);

            Inspect($"Serialized run {tourneyString}");
            Inspect(tourneyString);

            PlayerPrefs.SetString(saveKey+"-run", tourneyString);
        }
    }

    public bool LoadData()
    {
        Inspect("Loading data from save...");

        Inspect($"Deserialzie {saveKey}");
        if (PlayerPrefs.HasKey(saveKey))
        {
            string serialized = PlayerPrefs.GetString(saveKey + "-vars");
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
    public void StartNewGame()
    {
        DataItemPlayer.main.FromData(playerCharacter);
        TourneyController.main.FreshStart();
        ViewManager.Instance.OnNewGameBegin();
    }
}
