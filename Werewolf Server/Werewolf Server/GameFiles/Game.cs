
using Werewolf_Server.GameFiles;

namespace Werewolf_Server
{
    public enum State { Lobby, Night, Day }
    public class Game
    {
        public State state;
        public List<Player> _players = new List<Player>();

        private List<Player> AlivePlayers
        {
            get
            {
                return _players.Where(player => player.alive).ToList();
            }
        }

        private List<Player> Werewolves
        {
            get
            {
                return _players.Where(player => player.role.team == Team.Werewolf && player.alive).ToList();
            }
        }


        public Game()
        {
            state = State.Lobby;
        }

        public List<NamedMessage> Start(List<Connection> users)
        {
            List<NamedMessage> namedMessages = new List<NamedMessage>();

            state = State.Day;
            List<Role> roles = GeneratreRoles(users.Count);

            //Add a player with each role
            for (int i = 0; i < roles.Count; i++)
            {
                Player newPlayer = new Player(users[i].connectionName, roles[i]);
                _players.Add(newPlayer);
                users[i].player = newPlayer;

                //Broadcast new role
                namedMessages.Add(new NamedMessage(newPlayer.name, newPlayer.RoleString));
            }

            return namedMessages;
        }

        public List<NamedMessage> Update(string message)
        {
            List<NamedMessage> output = new List<NamedMessage>();
            return output;
        }

        //Returns all important information to the user
        public string GetReminderData(Player player)
        {
            return "";
        }

        public List<Role> GeneratreRoles(int playerCount)
        {
            List<Role> roles = new List<Role>();
            switch (playerCount)
            {
                case 2://2W
                    roles.Add(new Werewolf());
                    roles.Add(new Werewolf());
                    break;
                case 3://2V, 1W
                    roles.Add(new Villager());
                    roles.Add(new Villager());
                    roles.Add(new Werewolf());
                    break;
                default://No werewolves
                    for (int i = 0; i < playerCount; i++)
                    {
                        roles.Add(new Villager());
                    }
                    break;
            }

            //Shuffle list
            Random random = new Random();
            for (int i = roles.Count - 1; i > 1; i--)
            {
                int rnd = random.Next(i + 1);

                var temp = roles[rnd];
                roles[rnd] = roles[i];
                roles[i] = temp;
            }
            return roles;
        }

        public string GetPlayerList(List<Player>? players)
        {
            if (players == null)
            {
                players = _players;
            }

            string output = "";
            foreach (Player player in players)
            {
                //output += player.conn.name;
                output += ",";
            }
            if (output.Contains(","))
            {
                //Remove trailing comma
                output = output.Remove(output.Length - 1);
            }

            return output;
        }

        public void NightInit()
        {
            //Notify of alive players
            foreach (Player player in AlivePlayers)
            {
                //player.conn.Broadcast("Players;" + GetPlayerList(AlivePlayers));

                player.votes = 0;
                //Unready those with tasks
                if (player.role.hasNightTask)
                {
                    player.ready = false;
                }
                else
                {
                    player.ready = true;
                }
            }
        }

        public void NightSubmit(string myName, string message)
        {
            string[] selectedPlayerNames = message.Split(",");
            //Get selectedPlayers by name
            List<Player> selectedPlayers = new List<Player>();
            foreach (string playerName in selectedPlayerNames)
            {
                foreach (Player player in _players)
                {
/*                    if (player.conn.name == playerName)
                    {
                        selectedPlayers.Add(player);
                    }
*/                }

            }
            //Player me = _players.Find(player => player.conn.name == myName);
/*            string info = me.role.NightTask(selectedPlayers);


            //Send back relevant info
            if (info != "")
            {
                me.conn.Broadcast("NightInfo;" + info);
            }
            else
            {
                me.conn.Broadcast("NightInfo;Ready");
            }
            me.ready = true;
*/
            CheckNightFinished();

        }

        //Notify every other werewolf this name is clicked
        public void WerewolfClick(string myName, string clickedName, bool isClicked)
        {
            if (!isClicked)
            {
                clickedName = "!" + clickedName;
            }

            foreach (Player werewolf in Werewolves)
            {
/*                if (werewolf.conn.name != myName)
                {
                    //WerewolfClick;!<name>
                    werewolf.conn.Broadcast($"WerewolfClick;{clickedName};");
                }
*/            }

        }

        //Check everyone is ready
        private void CheckNightFinished()
        {
            int readyCount = 0;
            foreach (Player player in AlivePlayers)
            {
                if (player.ready) { readyCount++; }
            }

            if (readyCount == AlivePlayers.Count)
            {
                //Give at least 2 seconds to the last person getting info
                Thread.Sleep(2000);
                FinishNight();
            }
        }

        private void FinishNight()
        {
/*            _host.Broadcast("EndNight;");
*/
            //Sanity check, find the player most bitten by werewolves
            Player murderedPlayer = null;
            int mostVotes = 0;
            foreach (Player player in AlivePlayers)
            {
                if (mostVotes < player.votes)
                {
                    mostVotes = player.votes;
                    murderedPlayer = player;
                }
            }

            //Send murdered info
/*            if (murderedPlayer != null)
            {
                _host.Broadcast($"Murdered;{murderedPlayer.conn.name}");

                murderedPlayer.alive = false;
                murderedPlayer.conn.Broadcast("Dead");
            }

            _host.Broadcast("State;Day");
            foreach(Player player in _players)
            {
                player.conn.Broadcast("State;Day");
            }
*/        }

    }
}
