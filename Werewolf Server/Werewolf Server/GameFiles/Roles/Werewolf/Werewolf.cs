namespace Werewolf_Server.GameFiles.Roles.Werewolf
{
    public class Werewolf : Role
    {

        public Werewolf()
        {
            name = "Werewolf";
            description = "Kill majority of the town";
            nightDescription = "Choose someone to brutally murder";
            team = Team.Werewolf;
            hasNightTask = true;
            canMultiClick = false;
        }

        public override List<string> NightTask(Message message, List<Player> alivePlayers)
        {
            return new List<string>();
        }
    }
}
