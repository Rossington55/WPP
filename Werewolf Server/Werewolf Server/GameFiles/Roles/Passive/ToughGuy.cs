namespace Werewolf_Server.GameFiles.Roles.Passive
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


        public override List<string> NightTask(Message message, List<Player> players) { return new List<string>(); }
    }
}
