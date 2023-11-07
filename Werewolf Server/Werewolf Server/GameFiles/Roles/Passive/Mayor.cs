namespace Werewolf_Server.GameFiles.Roles.Villager
{
    public class Mayor : Role
    {

        public Mayor()
        {
            name = "Mayor";
            description = "Your vote counts twice";
            nightDescription = "Time for a calm and 'safe' rest";

            team = Team.Villager;
            hasNightTask = false;
            canMultiClick = false;
        }


        public override NightTaskResult NightTask(Message message, List<Player> players) { return new NightTaskResult(); }
    }
}
