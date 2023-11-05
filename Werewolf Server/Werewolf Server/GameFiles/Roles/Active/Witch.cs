﻿namespace Werewolf_Server.GameFiles.Roles.Active
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


        public override List<string> NightTask(Message message, List<Player> players)
        {

            //Get the selected player if a potion is used
            Player? selectedPlayer = null;
            if (message.subCommand == "None") { return new List<string>(); }
            selectedPlayer = players.Find(player => player.name == message.data[0]);
            if (selectedPlayer == null) { return new List<string>(); }

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
            if(!healthPotionUsed && !poisonPotionUsed) { hasNightTask = false; }

            return new List<string>();
        }
    }
}
