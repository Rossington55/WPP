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
        public override List<string> NightTask(Message message, List<Player> alivePlayers)
        {
            List<string> result = new List<string>();
            Player? selectedPlayer = alivePlayers.Find(player => player.name == message.data[0]);
            if (selectedPlayer == null) { return result; }

            result.Add($"This player a {selectedPlayer.role.name}");
            result.Add($"Role Description: {selectedPlayer.role.description}");

            return result;
        }
    }
}
