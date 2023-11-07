namespace Werewolf_Server.GameFiles.Roles.Werewolf
{
    public class WerewolfRole : Role
    {

        public WerewolfRole()
        {
            name = "Werewolf";
            description = "Kill majority of the town";
            nightDescription = "Choose someone to brutally murder";
            team = Team.Werewolf;
            hasNightTask = true;
            canMultiClick = false;
        }

        public override NightTaskResult NightTask(Message message, List<Player> alivePlayers)
        {

            return new NightTaskResult();
        }
    }
}
