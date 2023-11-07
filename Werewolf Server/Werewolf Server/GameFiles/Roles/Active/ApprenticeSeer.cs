namespace Werewolf_Server.GameFiles.Roles.Active
{
    public class ApprenticeSeer : Role
    {

        public ApprenticeSeer()
        {
            name = "Apprentice Seer";
            description = "Become the Seer if they are eliminated";
            nightDescription = "Choose a player to learn if they're a Werewolf";
            team = Team.Villager;
            hasNightTask = false;//Changes to true when seer dies
            canMultiClick = false;
        }

        //Confirm if selected player is a werewolf.
        public override NightTaskResult NightTask(Message message, List<Player> alivePlayers)
        {
            NightTaskResult result = new NightTaskResult();
            Player? selectedPlayer = alivePlayers.Find(player => player.name == message.data[0]);
            if (selectedPlayer == null) { return result; }

            bool isWerewolf = selectedPlayer.role.team == Team.Werewolf;
            if (isWerewolf)
            {
                result.data.Add("This player IS a Werewolf");
            }
            else
            {
                result.data.Add("This player is NOT a Werewolf");
            }

            return result;
        }
    }
}
