namespace Werewolf_Server.GameFiles.Roles.Active
{
    public class Revealer : Role
    {
        public Revealer()
        {
            name = "Revealer";
            description = "Put your life on the line to find and kill a Werewolf";
            nightDescription = "You may reveal a player. If they're a Werewolf they die. Otherwise you die";
            team = Team.Villager;
            hasNightTask = true;
            canMultiClick = false;
        }


        public override List<string> NightTask(Message message, List<Player> players)
        {
            List<string> output = new List<string>();
            if (message.subCommand == "None") { return output; }

            //Get the selected player if a potion is used
            Player? selectedPlayer = null;

            selectedPlayer = players.Find(player => player.name == message.data[0]);
            if (selectedPlayer == null) { return output; }

            if(selectedPlayer.role.team == Team.Werewolf)
            {
                selectedPlayer.deathTimer = 0;

                output.Add($"{selectedPlayer.name} IS a Werewolf");
                output.Add($"They will now die before the sun rises");
            }
            else
            {
                Player me = players.Find(player => player.name == message.player);
                me.deathTimer = 0;

                output.Add($"{selectedPlayer.name} is NOT a Werewolf");
                output.Add($"Prepare to die for your treason");
            }

            return output;
        }
    }
}
