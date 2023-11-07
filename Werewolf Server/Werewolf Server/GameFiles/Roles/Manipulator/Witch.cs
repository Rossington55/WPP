namespace Werewolf_Server.GameFiles.Roles.Manipulator
{
    public class Witch : Role
    {
        private bool healthPotionUsed;
        private bool poisonPotionUsed;

        public Witch()
        {
            name = "Witch";
            description = "You have one health and one poison potion to use at night";
            nightDescription = "Choose a player to use a potion on. (One time use each)";
            team = Team.Villager;
            hasNightTask = true;
            canMultiClick = false;

            healthPotionUsed = false;
            poisonPotionUsed = false;

        }


        public override NightTaskResult NightTask(Message message, List<Player> players)
        {
            NightTaskResult result = new NightTaskResult();
            //Get the selected player if a potion is used
            Player? selectedPlayer = null;
            if (message.subCommand == "None") { return result; }
            selectedPlayer = players.Find(player => player.name == message.data[0]);
            if (selectedPlayer == null) { return result; }

            switch (message.subCommand)
            {
                case "Health":
                    if (!healthPotionUsed)
                    {
                        selectedPlayer.invincible = true;
                        healthPotionUsed = true;
                    }
                    break;
                case "Poison":
                    if (!poisonPotionUsed)
                    {
                        selectedPlayer.deathTimer = 0;
                        poisonPotionUsed = true;
                    }
                    break;
            }

            //Remove night task if both potions are used
            if (healthPotionUsed && poisonPotionUsed)
            {
                hasNightTask = false;
                Player? me = players.Find(player => player.name == message.player);
                if (me != null)
                {
                    result.secondaryMessage = new Message(
                        me.name,
                        CommandClient.Role,
                        me.RoleDetails
                        );
                }
            }


            return result;
        }
    }
}
