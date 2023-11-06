namespace Werewolf_Server.GameFiles.Roles.Werewolf
{
    public class Sorceress : Role
    {

        public Sorceress()
        {
            name = "Sorceress";
            description = "Help the Werewolves by finding the Seer";
            nightDescription = "Choose a player to learn if they're a Seer";
            team = Team.Werewolf;
            hasNightTask = true;
            canMultiClick = false;
        }

        //Confirm if selected player is a werewolf
        public override List<string> NightTask(Message message, List<Player> alivePlayers)
        {
            List<string> result = new List<string>();
            Player? selectedPlayer = alivePlayers.Find(player => player.name == message.data[0]);
            if (selectedPlayer == null) { return result; }

            bool isSeer = selectedPlayer.role.name == "Seer";//True if werewolf

            //Apprentice Seer check
            if (!isSeer)
            {
                isSeer = selectedPlayer.role.name == "Apprentice Seer" && selectedPlayer.role.hasNightTask;
            }

            if (isSeer)
            {
                result.Add("This player IS a Seer");
            }
            else
            {
                result.Add("This player is NOT a Seer");
            }

            return result;
        }
    }
}
