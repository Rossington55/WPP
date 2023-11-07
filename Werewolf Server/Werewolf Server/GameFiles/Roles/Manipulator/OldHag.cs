namespace Werewolf_Server.GameFiles.Roles.Villager
{
    public class OldHag : Role
    {
        private Player lastSelected;
        public OldHag()
        {
            name = "Old Hag";
            description = "Send someone out of the room every day";
            nightDescription = "Select someone to leave town for the day";

            team = Team.Villager;
            hasNightTask = true;
            canMultiClick = false;
            canSelectLast = false;
        }


        public override NightTaskResult NightTask(Message message, List<Player> players) {
            NightTaskResult result = new NightTaskResult();
            Player? selectedPlayer = players.Find(player => player.name == message.data[0]);
            if(selectedPlayer == null) { return result; }

            //Cant select last
            if (selectedPlayer == null || selectedPlayer == lastSelected) { return result; }

            selectedPlayer.canVote = false;

            result.data.Add($"{selectedPlayer.name} will be sent out of the room");
            result.secondaryMessage = new Message(
                selectedPlayer.name,
                CommandClient.Alert,
                new List<string>() { "Shunned by the Old Hag","You must leave the room for the next day"}
                );

            lastSelected = selectedPlayer;
            return result;
        
        }
    }
}
