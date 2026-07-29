using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : Initializable
{
    public TextMeshProUGUI time, lap, distance, speed, position, fuelpercent, fuelvalue;
    public Image fuelFill;

    public void Update()
    {

        UpdateTime();
        UpdatePlayerPosition();
        UpdatePlayerFuel();
    }
    void UpdateTime()
    {
        if (time != null)
        {
            if (TourneyController.main.currentRace!= null && TourneyController.main.currentRace.IsRunning())
            {

                time.text = (Mathf.Ceil(TourneyController.main.currentRace.GetTimeRemaining() * 10) / 10).ToString();
            }
            else
            {
                time.text = "";
            }
        }
    }
    void UpdatePlayerPosition()
    {
        var player = TourneyController.main.GetPlayerRacer();
        if(player!=null)
        {
            if (lap!=null)
                lap.text = player.position.currentLap.ToString() ;
            if (position != null)
                position.text = (TourneyController.main.currentRace.GetPositionForRacer(player)+1).ToString();
            if (distance != null)
                distance.text = (Mathf.RoundToInt(player.position.distanceTraveled * 10) / 10).ToString();
            if (speed != null)
                speed.text = (Mathf.RoundToInt(player.stats.realSpeed * 10) / 10).ToString();
        }
    }
    void UpdatePlayerFuel()
    {
        var player = TourneyController.main.GetPlayerRacer();
        if(player!=null)
        {
            if (fuelvalue != null)
                fuelvalue.text =$"{ player.abilities.fuel.GetValue()}/{player.abilities.fuel.GetLimit()}";
            if (fuelpercent != null)
                fuelpercent.text = (Mathf.RoundToInt(player.abilities.fuel.GetPercentage()*100f) ).ToString();
                if (fuelFill != null)
                fuelFill.fillAmount = player.abilities.fuel.GetPercentage();
        }
    }
}
