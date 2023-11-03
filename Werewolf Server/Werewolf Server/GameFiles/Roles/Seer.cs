namespace Werewolf_Server
{
    public class Seer : Role
    {
        
        public Seer()
        {
            name = "Seer";
            description = "Search for the werewolves";
            nightDescription = "Find if someone is a werewolf";
            team = Team.Villager ;
            hasNightTask = true;
            canMultiClick = false;
        }

        //Confirm if selected player is a werewolf
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
