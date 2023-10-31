
using Werewolf_Server.GameFiles;
using Werewolf_Server.GameFiles.Modes;

namespace Werewolf_Server
{
    public enum State { Lobby, Night, Day }
    public class Game
    {
        public State state;
        private List<Player> _players = new List<Player>();
        private List<Message> _messagesOut;
        private GameModes gameModes;

        private List<Player> AlivePlayers
        {
            get
            {
                return _players.Where(player => player.alive).ToList();
            }
        }

        private List<string> AlivePlayersString
        {
            get
            {
                List<string> alivePlayersString = new List<string>();
                foreach (Player player in AlivePlayers)
                {
                    alivePlayersString.Add(player.name);
                }
                return alivePlayersString;
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
            gameModes = new GameModes();
        }

        public List<Message> Start(List<Connection> users, string gameMode)
        {
            List<Message> namedMessages = new List<Message>();
            gameModes.GetMode(users.Count, gameMode);

            state = State.Day;
            List<Role> roles = GenerateRoles(users.Count);

            //Add a player with each role
            for (int i = 0; i < roles.Count; i++)
            {
                Player newPlayer = new Player(users[i].connectionName, roles[i]);
                _players.Add(newPlayer);
                users[i].player = newPlayer;

                //Broadcast new role
                namedMessages.Add(new Message(
                    newPlayer.name,
                    CommandClient.Role,
                    newPlayer.RoleDetails
                    ));
            }

            return namedMessages;
        }

        public List<Message> Update(Message message)
        {
            _messagesOut = new List<Message>();
            switch (message.commandServer)
            {
                case CommandServer.StartNight:
                    NightInit();
                    break;

                case CommandServer.WerewolfSelectPlayer:
                    WerewolfClick(message);
                    break;

                case CommandServer.NightSubmit:
                    NightSubmit(message);
                    break;
            }
            return _messagesOut;
        }

        //Returns all important information to the user
        public List<Message> GetReminderData(Player player)
        {
            return new List<Message>();
        }

        public List<Role> GenerateRoles(int playerCount)
        {
            List<Role> roles = gameModes.currentMode.roles;
            

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

        public Player GetPlayerByRole(string roleName)
        {
            return _players.Find(player => player.role.name == roleName);
        }
        public void NightInit()
        {
            state = State.Night;
            _messagesOut.Add(new Message(
                "host",
                CommandClient.State,
                state.ToString()
                ));

            //Notify of alive players
            foreach (Player player in AlivePlayers)
            {
                _messagesOut.Add(new Message(
                    player.name,
                    CommandClient.PlayerList,
                    AlivePlayersString
                    ));
                _messagesOut.Add(new Message(
                    player.name,
                    CommandClient.State,
                    state.ToString()
                    ));

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

        public void NightSubmit(Message message)
        {
            Player? player = _players.Find(player => player.name == message.player);
            if (player == null) { return; }

            string result = player.role.NightTask(message, AlivePlayers);

            //Player successfully submitted
            player.ready = true;
            _messagesOut.Add(new Message(player.name, CommandClient.Submitted));
            CheckNightFinished();
        }

        //Notify every other werewolf this name is clicked
        public void WerewolfClick(Message message)
        {
            //Find the selected player
            Player? selectedPlayer = _players.Find(player => player.name == message.data[0]);
            if (selectedPlayer == null) { return; }

            //Select/Deselect
            if (message.subCommand == "select")
            {
                selectedPlayer.werewolvesAttacking++;
            }
            else if (message.subCommand == "deselect")
            {
                if (selectedPlayer.werewolvesAttacking > 0)
                {
                    selectedPlayer.werewolvesAttacking--;
                }
            }

            //Get names of players with how many attacking
            List<string> attackList = new List<string>();
            foreach (Player player in AlivePlayers)
            {
                attackList.Add($"{player.name};{player.werewolvesAttacking}");
            }

            //Notify each werewolf of the new selection
            foreach (Player werewolf in Werewolves)
            {
                _messagesOut.Add(new Message(
                    werewolf.name,
                    CommandClient.WerewolfSelectedPlayerList,
                    attackList
                    ));
            }

        }

        //Check everyone is ready
        private void CheckNightFinished()
        {
            int readyCount = 0;
            foreach (Player player in AlivePlayers)
            {
                //Players without a night task are ready by default
                if (player.ready) { readyCount++; }
            }

            if (readyCount == AlivePlayers.Count)
            {
                FinishNight();
            }
        }

        private void FinishNight()
        {
            state = State.Day;
            _messagesOut.Add(new Message(
                "host",
                CommandClient.State,
                state.ToString()
                ));

            //Sanity check, find the player most bitten by werewolves
            Player murderedPlayer = null;
            int mostVotes = 0;
            foreach (Player player in AlivePlayers)
            {
                if (mostVotes < player.werewolvesAttacking)
                {
                    mostVotes = player.werewolvesAttacking;
                    murderedPlayer = player;
                }

                //Notify all its day
                _messagesOut.Add(new Message(
                    player.name,
                    CommandClient.State,
                    state.ToString()
                    ));
            }

            if (murderedPlayer != null)
            {
                murderedPlayer.alive = false;
                _messagesOut.Add(new Message(murderedPlayer.name, CommandClient.Murdered));
            }
        }

    }
}
