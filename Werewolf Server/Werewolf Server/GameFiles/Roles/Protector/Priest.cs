namespace Werewolf_Server.GameFiles.Roles.Protector
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


        public override NightTaskResult NightTask(Message message, List<Player> players)
        {
            NightTaskResult result = new NightTaskResult();
            if (message.subCommand == "None") { return result; }

            //Get the selected player if the prayer is used
            Player? selectedPlayer = null;
            selectedPlayer = players.Find(player => player.name == message.data[0]);
            if (selectedPlayer == null) { return result; }

            selectedPlayer.invincible = true;

            result.data.Add("Your prayers have been answered");
            result.data.Add($"{selectedPlayer.name} will be protected for tonight");

            hasNightTask = false;

            return result;
        }
    }
}
