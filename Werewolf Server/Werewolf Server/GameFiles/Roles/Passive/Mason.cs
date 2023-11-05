namespace Werewolf_Server.GameFiles.Roles.Passive
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
        }


        public override List<string> NightTask(Message message, List<Player> players)
        {
            //Get the other masons (not me)
            List<Player> masons = players.FindAll(player => player.role.name == "Mason" && player.name != message.player);

            List<string> output = new List<string>() { "The other Masons are:" };
            if(masons.Count == 0 ) {
                output[0] = "There are no other Masons";
            }

            foreach (Player mason in masons)
            {
                output.Add(mason.name);
            }

            return output;
        }
    }
}
