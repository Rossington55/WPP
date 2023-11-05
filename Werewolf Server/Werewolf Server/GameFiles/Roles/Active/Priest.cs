namespace Werewolf_Server.GameFiles.Roles.Active
{
    public class Priest : Role
    {
        public Priest()
        {
            name = "Priest";
            description = "May protect someone once";
            nightDescription = "You may protect someone. (One time use)";
            team = Team.Villager;
            hasNightTask = true;
            canMultiClick = false;
        }


        public override List<string> NightTask(Message message, List<Player> players)
        {
            List<string> output = new List<string>();
            if (message.subCommand == "None") { return output; }

            //Get the selected player if the prayer is used
            Player? selectedPlayer = null;
            selectedPlayer = players.Find(player => player.name == message.data[0]);
            if (selectedPlayer == null) { return output; }

            selectedPlayer.invincible = true;

            output.Add("Your prayers have been answered");
            output.Add($"{selectedPlayer.name} will be protected for tonight");

            hasNightTask = false;

            return output;
        }
    }
}
