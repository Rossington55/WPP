namespace Werewolf_Server.GameFiles.Roles.Active
{
    public class MysticSeer : Role
    {

        public MysticSeer()
        {
            name = "Mystic Seer";
            description = "Learn players exact roles";
            nightDescription = "Choose a player to learn their role";
            team = Team.Villager;
            hasNightTask = true;
            canMultiClick = false;
        }

        //Confirm if selected player is a werewolf
        public override NightTaskResult NightTask(Message message, List<Player> alivePlayers)
        {
            NightTaskResult result = new NightTaskResult();
            Player? selectedPlayer = alivePlayers.Find(player => player.name == message.data[0]);
            if (selectedPlayer == null) { return result; }

            result.data.Add($"This player a {selectedPlayer.role.name}");
            result.data.Add($"Role Description: {selectedPlayer.role.description}");

            return result;
        }
    }
}
