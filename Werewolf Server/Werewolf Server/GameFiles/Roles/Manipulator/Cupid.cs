namespace Werewolf_Server.GameFiles.Roles.Villager
{
    public class Cupid : Role
    {
        public Cupid()
        {
            name = "Cupid";
            description = "Link two players to die together";
            nightDescription = "Link two players to die together";

            team = Team.Villager;
            hasNightTask = true;
            canMultiClick = true;
        }


        public override NightTaskResult NightTask(Message message, List<Player> players) {
            NightTaskResult result = new NightTaskResult();
            Player? selectedPlayer1 = players.Find(player => player.name == message.data[0]);
            Player? selectedPlayer2 = players.Find(player => player.name == message.data[1]);
            if(selectedPlayer1 == null || selectedPlayer2 == null) { return result; }


            selectedPlayer1.linkedPlayer = selectedPlayer2;
            selectedPlayer2.linkedPlayer = selectedPlayer1;

            result.data.Add($"{selectedPlayer1.name} and {selectedPlayer2.name} are now linked");

            this.hasNightTask = false;
            return result;
        
        }
    }
}
