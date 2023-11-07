namespace Werewolf_Server.GameFiles.Roles.Seer
{
    public class AuraSeer : Role
    {

        public AuraSeer()
        {
            name = "Aura Seer";
            description = "Learn if a player has special abilities";
            nightDescription = "Choose a player to inspect their Aura";
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

            bool hasSpecial = selectedPlayer.role.hasNightTask;

            //Werewolves technically arent special
            if(selectedPlayer.role.name == "Werewolf")
            {
                hasSpecial = false;
            }

            if (hasSpecial)
            {
                result.data.Add($"{selectedPlayer.name} HAS a special ability");
            }
            else
            {
                result.data.Add($"{selectedPlayer.name} has NO special abilities");
            }

            return result;
        }
    }
}
