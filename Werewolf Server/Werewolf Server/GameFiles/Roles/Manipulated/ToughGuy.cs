namespace Werewolf_Server.GameFiles.Roles.Manipulated
{
    public class ToughGuy : Role
    {

        public ToughGuy()
        {
            name = "Tough Guy";
            description = "You take two nights to die by Werewolf";
            nightDescription = "Time for a calm and 'safe' rest";

            team = Team.Villager;
            hasNightTask = false;
            canMultiClick = false;
        }


        public override NightTaskResult NightTask(Message message, List<Player> players) { return new NightTaskResult(); }
    }
}
