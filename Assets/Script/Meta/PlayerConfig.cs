
using System;
using UnityEngine;
[Serializable]
public class PlayerConfig : MonoBehaviour
{
    public ShipScriptable playerCharacter;
    public static PlayerConfig main;

    private void Awake()
    {
        if (main == null)
        {
            main = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }
    private void Start()
    {
        DataItemPlayer.main.FromData(playerCharacter);
        ViewManager.Instance.OnNewGameBegin();
    }
}
