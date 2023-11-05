﻿namespace Werewolf_Server.GameFiles.Roles.Active
{
    public class Seer : Role
    {

        public Seer()
        {
            name = "Seer";
            description = "Search for the Werewolves at night";
            nightDescription = "Choose a player to learn if they're a Werewolf";
            team = Team.Villager;
            hasNightTask = true;
            canMultiClick = false;
        }

        //Confirm if selected player is a werewolf
        public override List<string> NightTask(Message message, List<Player> alivePlayers)
        {
            List<string> result = new List<string>();
            Player? selectedPlayer = alivePlayers.Find(player => player.name == message.data[0]);
            if (selectedPlayer == null) { return result; }

            bool isWerewolf = selectedPlayer.role.team == Team.Werewolf;//True if werewolf
            if(selectedPlayer.role.name == "Lycan") {isWerewolf =true;}//Or if Lycan

            if (isWerewolf)
            {
                result.Add("This player IS a Werewolf");
            }
            else
            {
                result.Add("This player is NOT a Werewolf");
            }

            return result;
        }
    }
}