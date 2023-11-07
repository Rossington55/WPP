namespace Werewolf_Server.GameFiles.Roles.Villager
{
    public class Mason : Role
    {

        public Mason()
        {
            name = "Mason";
            description = "You know who the other Masons are";
            nightDescription = "Remind yourself who the other masons are";

            team = Team.Villager;
            hasNightTask = true;
            canMultiClick = false;
            noNightSelection = true;
        }


        public override NightTaskResult NightTask(Message message, List<Player> players)
        {
            NightTaskResult result = new NightTaskResult();
            //Get the other masons (not me)
            List<Player> masons = players.FindAll(player => player.role.name == "Mason" && player.name != message.player);

            result.data.Add("The other Masons are:");
            if (masons.Count == 0)
            {
                result.data[0] = "There are no other Masons";
            }

            foreach (Player mason in masons)
            {
                result.data.Add(mason.name);
            }

            return result;
        }
    }
}
