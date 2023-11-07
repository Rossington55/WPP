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


        public override NightTaskResult NightTask(Message message, List<Player> players)
        {
            NightTaskResult result = new NightTaskResult();
            if (message.subCommand == "None") { return result; }

            //Get the selected player if a potion is used
            Player? selectedPlayer = null;

            selectedPlayer = players.Find(player => player.name == message.data[0]);
            if (selectedPlayer == null) { return result; }

            if(selectedPlayer.role.team == Team.Werewolf)
            {
                selectedPlayer.deathTimer = 0;

                result.data.Add($"{selectedPlayer.name} IS a Werewolf");
                result.data.Add($"They will now die before the sun rises");
            }
            else
            {
                Player me = players.Find(player => player.name == message.player);
                me.deathTimer = 0;

                result.data.Add($"{selectedPlayer.name} is NOT a Werewolf");
                result.data.Add($"Prepare to die for your treason");
            }

            return result;
        }
    }
}
