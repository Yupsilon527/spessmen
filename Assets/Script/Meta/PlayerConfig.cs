
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
            PlayerPrefs.SetInt(saveKey, 1);
        }
        else { 
        PlayerPrefs.SetInt(saveKey, 0);
    }
    }
    public bool HasRun()
    {
        return PlayerPrefs.GetString(saveKey + "-run").Length > 0;
    }
    public void ClearRun()
    {
        PlayerPrefs.SetString(saveKey + "-run", "");
        PlayerPrefs.SetInt(saveKey, 0);
    }

    public bool LoadData()
    {
        Inspect("Loading data from save...");

        Inspect($"Deserialzie {saveKey}");
        if (PlayerPrefs.HasKey(saveKey))
        {
            LoadVars();
            return true;
        }

        return false;
    }
    SerialziedTourney LoadRun()
    {
        string serialized = PlayerPrefs.GetString(saveKey + "-run");
        Inspect("Deserialize data " + serialized);

        SerialziedTourney varData = JsonUtility.FromJson<SerialziedTourney>(serialized);
        if (varData != null)
        {
            return varData;
        }
        return null;
    }
    void LoadVars()
    {

        string serialized = PlayerPrefs.GetString(saveKey + "-vars");
        Inspect("Load run from data " + serialized);

        VariableScopeSerializable varData = JsonUtility.FromJson<VariableScopeSerializable>(serialized);
        if (varData != null)
        {
            varData.Deserialize(globalScope);
        }
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
    public void StartNewGameFromSaveData()
    {
        var run = LoadRun();
        run.player.Deserialize(DataItemPlayer.main);
        run.Deserialize(TourneyController.main);
        ViewManager.Instance.ChangeView(ViewManager.Views.shopView);
    }
}
