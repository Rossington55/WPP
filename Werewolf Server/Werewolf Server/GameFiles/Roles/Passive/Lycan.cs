namespace Werewolf_Server.GameFiles.Roles.Passive
{
    public class Lycan : Role
    {

        public Lycan()
        {
            name = "Lycan";
            description = "A Villager with Wolf blood. Will be seen as a Werwolf although you aren't one.";
            nightDescription = "Time for a calm and 'safe' rest";

            team = Team.Villager;
            hasNightTask = false;
            canMultiClick = false;
        }


        public override List<string> NightTask(Message message, List<Player> players) { return new List<string>(); }
    }
}
