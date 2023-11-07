using Werewolf_Server.GameFiles.Roles.Villager;
using Werewolf_Server.GameFiles.Roles.Werewolf;

namespace Werewolf_Server.GameFiles.Roles.Manipulated
{
    public class Drunk : Role
    {
        private int nightCount = 0;
        public Drunk()
        {
            name = "Drunk";
            description = "You cant remember your role until the 3rd night";
            nightDescription = "Attempt to Sober up";

            team = Team.Villager;
            hasNightTask = true;
            canMultiClick = false;
            noNightSelection = true;
        }


        public override NightTaskResult NightTask(Message message, List<Player> players)
        {
            NightTaskResult result = new NightTaskResult();
            Player me = players.Find(player => player.name == message.player);
            nightCount++;

            if (nightCount == 3)
            {
                Role newRole;
                Random rand = new Random();
                if (rand.Next(0, 1) == 1)//1 = werewolf, 0 = villager
                {
                    newRole = new WerewolfRole();
                }
                else
                {
                    newRole = new VillagerRole();
                }

                me.role = newRole;
                result.data.Add("You Sober Up");
                result.data.Add($"You are now a {newRole.name}");

                result.secondaryMessage = new Message(
                    me.name,
                    CommandClient.Role,
                    me.RoleDetails
                    );
            }
            else
            {
                result.data.Add("Not sober yet. But keep trying");
                result.data.Add("I believe in you");
            }

            return result;
        }
    }
}
