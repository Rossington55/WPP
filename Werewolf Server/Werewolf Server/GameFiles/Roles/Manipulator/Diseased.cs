namespace Werewolf_Server.GameFiles.Roles.Villager
{
    public class Diseased : Role
    {

        public Diseased()
        {
            name = "Diseased";
            description = "When bitten, Werewolves will spend a night recovering";
            nightDescription = "Time for a calm and 'safe' rest";

            team = Team.Villager;
            hasNightTask = false;
            canMultiClick = false;
        }


        public override NightTaskResult NightTask(Message message, List<Player> players) { return new NightTaskResult(); }
    }
}
