namespace Werewolf_Server.GameFiles.Roles.Villager
{
    public class Spellcaster : Role
    {
        private Player lastSelected;
        public Spellcaster()
        {
            name = "Spellcaster";
            description = "Silence someone every day";
            nightDescription = "Select someone to cast a spell of muteness";

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

            result.data.Add($"{selectedPlayer.name} will be silenced");
            result.secondaryMessage = new Message(
                selectedPlayer.name,
                CommandClient.Alert,
                new List<string>() { "Silenced by the Spellcaster","You must stay completely silent for the next day"}
                );

            lastSelected = selectedPlayer;
            return result;
        
        }
    }
}
