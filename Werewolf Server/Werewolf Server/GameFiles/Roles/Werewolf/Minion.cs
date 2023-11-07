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
            noNightSelection = true;
        }


        public override NightTaskResult NightTask(Message message, List<Player> players)
        {
            NightTaskResult result = new NightTaskResult();
            //Get the other masons (not me)
            List<Player> werewolves = players.FindAll(player => player.role.team == Team.Werewolf && player.name != message.player);

            result.data.Add("The other Werewolves are:");
            if(werewolves.Count == 0 ) {
                result.data[0] = "There are no other Werewolves";
            }

            foreach (Player werewolf in werewolves)
            {
                result.data.Add(werewolf.name);
            }

            return result;
        }
    }
}
