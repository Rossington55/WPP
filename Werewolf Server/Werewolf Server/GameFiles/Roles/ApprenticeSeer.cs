namespace Werewolf_Server
{
    public class ApprenticeSeer : Role
    {
        
        public ApprenticeSeer()
        {
            name = "Apprentice Seer";
            description = "Become the Seer if they are eliminated";
            nightDescription = "Find if someone is a werewolf";
            team = Team.Villager ;
            hasNightTask = false;//Changes to true when seer dies
            canMultiClick = false;
        }

        //Confirm if selected player is a werewolf.
        public override string NightTask(Message message, List<Player> alivePlayers) {

            Player? selectedPlayer = alivePlayers.Find(player => player.name == message.data[0]);
            if (selectedPlayer == null) { return ""; }

            bool isWerewolf = selectedPlayer.role.team == Team.Werewolf;
            if (isWerewolf)
            {
                return "This player IS a Werewolf";
            }
            else
            {
                return "This player is NOT a Werewolf";
            }
        }
    }
}
