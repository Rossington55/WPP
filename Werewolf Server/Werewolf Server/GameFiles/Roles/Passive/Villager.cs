namespace Werewolf_Server.GameFiles.Roles.Villager
{
    public class VillagerRole : Role
    {

        public VillagerRole()
        {
            name = "Villager";
            description = "Find the werewolves and eliminate them";
            nightDescription = "Time for a calm and 'safe' rest";

            team = Team.Villager;
            hasNightTask = false;
            canMultiClick = false;
        }


        public override NightTaskResult NightTask(Message message, List<Player> players) { return new NightTaskResult(); }
    }
}
