namespace Werewolf_Server.GameFiles.Roles.Villager
{
    public class CultLeader : Role
    {
        public CultLeader()
        {
            name = "Cult Leader";
            description = "Win when everyone still alive is in your cult";
            nightDescription = "Select someone to join your cult";

            team = Team.Cult;
            hasNightTask = true;
            canMultiClick = false;
        }


        public override NightTaskResult NightTask(Message message, List<Player> players)
        {
            NightTaskResult result = new NightTaskResult();
            Player? selectedPlayer = players.Find(player => player.name == message.data[0]);
            if (selectedPlayer == null) { return result; }

            selectedPlayer.inCult = true;
            result.data.Add($"{selectedPlayer.name} is now apart of your Cult");

            //
            int cultCount = 0;
            foreach (Player player in players)
            {
                if (player.inCult) { cultCount++; }
            }

            result.data.Add($"{cultCount} out of {players.Count} are in your cult");

            return result;
        }
    }
}
