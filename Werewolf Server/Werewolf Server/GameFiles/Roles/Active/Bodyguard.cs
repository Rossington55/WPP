namespace Werewolf_Server.GameFiles.Roles.Active
{
    public class Bodyguard : Role
    {
        private Player lastProtected;

        public Bodyguard()
        {
            name = "Bodyguard";
            description = "Can protect a player every night";
            nightDescription = "You may protect someone. (You may do so once per night)";
            team = Team.Villager;
            hasNightTask = true;
            canMultiClick = false;
            canSelectLast = false;
        }


        public override List<string> NightTask(Message message, List<Player> players)
        {
            List<string> output = new List<string>();
            if (message.subCommand == "None") { return output; }

            //Get the selected player if the prayer is used
            Player? selectedPlayer = null;
            selectedPlayer = players.Find(player => player.name == message.data[0]);
            if (selectedPlayer == null || selectedPlayer == lastProtected) { return output; }

            selectedPlayer.invincible = true;
            lastProtected = selectedPlayer;

            output.Add("That's some solid protection!");
            output.Add($"{selectedPlayer.name} will be protected for tonight.");

            return output;
        }
    }
}
