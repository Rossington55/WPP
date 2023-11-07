namespace Werewolf_Server.GameFiles.Roles.Active
{
    public class Huntress : Role
    {

        public Huntress()
        {
            name = "Huntress";
            description = "Can kill a player every night";
            nightDescription = "Choose someone to kill";
            team = Team.Villager;
            hasNightTask = true;
            canMultiClick = false;
            canSelectLast = false;
        }


        public override NightTaskResult NightTask(Message message, List<Player> players)
        {
            NightTaskResult result = new NightTaskResult();
            if (message.subCommand == "None") { return result; }

            //Get the selected player if the prayer is used
            Player? selectedPlayer = null;
            selectedPlayer = players.Find(player => player.name == message.data[0]);
            if (selectedPlayer == null ) { return result; }

            selectedPlayer.deathTimer = 0;

            result.data.Add($"You cast your bow at {selectedPlayer.name}");

            return result;
        }
    }
}
