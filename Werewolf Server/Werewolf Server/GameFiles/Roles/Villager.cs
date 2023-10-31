namespace Werewolf_Server
{
    public class Villager : Role
    {

        public Villager()
        {
            name = "Villager";
            description = "Find the werewolves";
            nightDescription = "Time for a calm and 'safe' rest";

            team = Team.Villager;
            hasNightTask = false;
            canMultiClick = false;
        }


        public override string NightTask(Message message, List<Player> players) { return ""; }
    }
}
