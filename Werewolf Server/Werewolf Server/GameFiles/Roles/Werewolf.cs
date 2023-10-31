namespace Werewolf_Server
{
    public class Werewolf : Role
    {
        
        public Werewolf()
        {
            name = "Werewolf";
            description = "Kill majority of the villagers";
            nightDescription = "Choose someone to brutally murder";
            team = Team.Werewolf;
            hasNightTask = true;
            canMultiClick = false;
        }

        public override string NightTask(Message message, List<Player> alivePlayers) {
            return "";
        }
    }
}
