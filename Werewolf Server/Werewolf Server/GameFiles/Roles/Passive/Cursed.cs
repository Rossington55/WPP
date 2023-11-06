namespace Werewolf_Server.GameFiles.Roles.Passive
{
    public class Cursed : Role
    {

        public Cursed()
        {
            name = "Cursed";
            description = "Turns into a Werewolf when bitten by the Werewolves";
            nightDescription = "Time for a calm and 'safe' rest";

            team = Team.Villager;
            hasNightTask = false;
            canMultiClick = false;
        }


        public override List<string> NightTask(Message message, List<Player> players) { return new List<string>(); }
    }
}
