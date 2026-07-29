using System.Collections.Generic;

public class TourneyController : Initializable
{
    public static TourneyController main;
   HashSet<Racer> racers = new();
    protected override void Initialize()
    {
        main = this;
        base.Initialize();
    }
    public void InitRacers(int opponents = 5)
    {
        racers.Add(new PlayerRacer(DataItemPlayer.main.ship));
        for (int i  = 0; i < opponents; i++)
        {
            racers.Add(new AiRacer(i+1));
        }
    }
}
