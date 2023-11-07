namespace Werewolf_Server.GameFiles.Roles.Villager
{
    public class Doppelganger : Role
    {
        public Doppelganger()
        {
            name = "Doppelganger";
            description = "Become someone elses role when they die";
            nightDescription = "Select someone to copy their role";

            team = Team.Villager;
            hasNightTask = true;
            canMultiClick = false;
        }


        public override NightTaskResult NightTask(Message message, List<Player> players)
        {
            NightTaskResult result = new NightTaskResult();
            Player? selectedPlayer = players.Find(player => player.name == message.data[0]);
            if (selectedPlayer == null) { return result; }

            selectedPlayer.selectedByDoppelganger = true;

            result.data.Add($"When {selectedPlayer.name} dies, you will take on their role");
                
            return result;
        }
    }
}
