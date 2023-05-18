using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InterfaceController : MonoBehaviour
{
    public static InterfaceController main;

    private void Awake()
    {
        main = this;
        errorTooltip.color = Color.clear;
    }

    public TextMeshProUGUI healthGauge;
    public TextMeshProUGUI oxygenGauge;
    public TextMeshProUGUI goldGauge;
    public TextMeshProUGUI woodGauge;

    public TextMeshProUGUI handTooltip;
    public TextMeshProUGUI itemTooltip;

    public TextMeshProUGUI errorTooltip;
    public GameObject helpTooltip;
    Color errorColor = Color.clear;

    Player myPlayer;
    public void TieToPlayer(Player nPlayer)
    {
        myPlayer = nPlayer;
    }
    private void Update()
    {
        if (myPlayer == null)
            return;

        healthGauge.text = "Health: " + Mathf.Ceil(myPlayer.health.Health.GetPercentage() * 100) + "%";
        oxygenGauge.text = "Oxygen: " + Mathf.Ceil(AtmosphereController.oxygen.GetPercentage() * 100) + "%";

        goldGauge.text = "Gold: " + myPlayer.resources.GetResource(ResourceController.Resources.gold);
        woodGauge.text = "Stone: " + myPlayer.resources.GetResource(ResourceController.Resources.wood);

        if (myPlayer.backpack.GetActiveItem() != null)
            handTooltip.text = myPlayer.backpack.GetActiveItem().GetMobName();
        else
            handTooltip.text = "";

        if (myPlayer.hauler.TouchedItem != null)
            itemTooltip.text = myPlayer.hauler.TouchedItem.GetMobName();
        else
            itemTooltip.text = "";
        if (helpTooltip != null)
            helpTooltip.gameObject.SetActive(Input.GetKey(KeyCode.H));
    }
    public void ShowWarning(string ErrorText, float duration)
    {
        ShowError(ErrorText, Color.white, duration);
    }
    public void ShowError(string ErrorText, float duration)
    {
        ShowError(ErrorText, Color.red, duration);
    }
    public void ShowError(string ErrorText, Color textColor, float duration)
    {
        if (ErrorCoroutine!=null)
        {
            StopCoroutine(ErrorCoroutine);
        }
        ErrorCoroutine = StartCoroutine(DisplayError(ErrorText, textColor, duration));
    }
    Coroutine ErrorCoroutine;
    IEnumerator DisplayError(string ErrorText, Color textColor, float duration)
    {
        errorTooltip.text = ErrorText;
        float endAlpha = textColor.a;
        errorColor = textColor;
        
        while (errorColor.a!=endAlpha)
        {
            errorColor.a += Time.deltaTime * 4;
            errorTooltip.color = errorColor;
            yield return new WaitForEndOfFrame();
        }
        errorColor.a = endAlpha;
        errorTooltip.color = errorColor;
        yield return new WaitForSeconds(duration);

        while (errorColor.a > 0)
        {
            errorColor.a -= Time.deltaTime * 4;
            errorTooltip.color = errorColor;
            yield return new WaitForEndOfFrame();
        }
        errorTooltip.color = Color.clear;
    }
}
