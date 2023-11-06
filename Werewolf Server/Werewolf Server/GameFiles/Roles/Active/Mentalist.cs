namespace Werewolf_Server.GameFiles.Roles.Active
{
    public class Mentalist : Role
    {

        public Mentalist()
        {
            name = "Mentalist";
            description = "Know who's on the same team";
            nightDescription = "Select to players to see of they're on the same team";
            team = Team.Villager;
            hasNightTask = true;
            canMultiClick = false;
        }

        public override List<string> NightTask(Message message, List<Player> alivePlayers)
        {
            List<string> result = new List<string>();
            Player? selectedPlayer1 = alivePlayers.Find(player => player.name == message.data[0]);
            Player? selectedPlayer2 = alivePlayers.Find(player => player.name == message.data[1]);
            if (selectedPlayer1 == null) { return result; }
            if (selectedPlayer2 == null) { return result; }

            bool sameTeam = selectedPlayer1.role.team == selectedPlayer2.role.team;
            //Lycan check
            if (sameTeam)
            {
                //Already on same team so must both be villagers
                if (selectedPlayer1.role.name == "Lycan" || selectedPlayer2.role.name == "Lycan")
                {
                    sameTeam = false;
                }
            }

            //Check if both players are the same team
            if (sameTeam)
            {
                result.Add("The players ARE on the same team");
            }
            else
            {
                result.Add("The players are NOT on the same team");
            }

            return result;
        }
    }
}
