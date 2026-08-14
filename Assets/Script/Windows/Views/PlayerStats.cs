using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : Initializable
{
    public Racer player;
    public TextMeshProUGUI  lap, distance, speed, position, fuelpercent, fuelvalue;
    public Image fuelFill;

    public void Update()
    {
        UpdatePlayerPosition();
        UpdatePlayerFuel();
    }
    public void AssignRacer(Racer racer)
    {
        player = racer;
    }
    void UpdatePlayerPosition()
    {
        if (player != null)
        {
            if (lap != null)
                lap.text = player.position.currentLap.ToString();
            if (position != null)
                position.text = (TourneyController.main.ongoingRace.GetPositionForRacer(player) + 1).ToString();
            if (distance != null)
                distance.text = $"{Mathf.RoundToInt(player.position.distanceTraveled * 10) / 10}m";
            if (speed != null)
                speed.text = (Mathf.RoundToInt(player.stats.realSpeed * 10) / 10).ToString()+"km/h";
        }
    }
    void UpdatePlayerFuel()
    {
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
