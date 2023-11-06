namespace Werewolf_Server.GameFiles.Roles.Werewolf
{
    public class Minion : Role
    {

        public Minion()
        {
            name = "Minion";
            description = "Works for the Werewolves without them knowing";
            nightDescription = "Remind yourself who the other Werewolves are";

            team = Team.Werewolf;
            hasNightTask = true;
            canMultiClick = false;
        }


        public override List<string> NightTask(Message message, List<Player> players)
        {
            //Get the other masons (not me)
            List<Player> werewolves = players.FindAll(player => player.role.team == Team.Werewolf && player.name != message.player);

            List<string> output = new List<string>() { "The other Werewolves are:" };
            if(werewolves.Count == 0 ) {
                output[0] = "There are no other Werewolves";
            }

            foreach (Player werewolf in werewolves)
            {
                output.Add(werewolf.name);
            }

            return output;
        }
    }
}
