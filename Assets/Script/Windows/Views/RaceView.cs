
public class RaceView : ViewBase
{
    public PlayerShipGrid playership;
    public PlayerAbilityPreview preview;
    public Leaderboard leaderboard;

    public override void OnOpened()
    {
        base.OnOpened();
        if (DataItemPlayer.main == null) return;
        playership.AssignShip(DataItemPlayer.main.ship);
        preview.LoadPlayerShip(DataItemPlayer.main.ship);
    }
}
