namespace Werewolf_Server.GameFiles.Roles.Villager
{
    public class Tanner : Role
    {

        public Tanner()
        {
            name = "Tanner";
            description = "Win by getting voted out";
            nightDescription = "Time for a calm and 'safe' rest";

            team = Team.Tanner;
            hasNightTask = false;
            canMultiClick = false;
        }


        public override NightTaskResult NightTask(Message message, List<Player> players) { return new NightTaskResult(); }
    }
}
