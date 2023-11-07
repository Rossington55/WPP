
using Werewolf_Server.GameFiles;
using Werewolf_Server.GameFiles.Modes;
using Werewolf_Server.GameFiles.Roles.Werewolf;

namespace Werewolf_Server
{
    public enum State { Lobby, Night, Day }
    public class Game
    {
        public State state;
        private List<Player> _players = new List<Player>();
        private List<Message> _messagesOut;
        private GameModes gameModes;

        public List<Player> AlivePlayers
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
            _messagesOut = new List<Message>();
            gameModes.GetMode(users.Count, gameMode);

            List<Role> roles = GenerateRoles(users.Count);

            //Add a player with each role
            for (int i = 0; i < roles.Count; i++)
            {
                Player newPlayer = new Player(users[i].connectionName, roles[i]);
                _players.Add(newPlayer);
                users[i].player = newPlayer;

                //Broadcast new role
                _messagesOut.Add(new Message(
                    newPlayer.name,
                    CommandClient.Role,
                    newPlayer.RoleDetails
                    ));
            }

            ChangeState(State.Day);
            return _messagesOut;
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

                case CommandServer.SelectVote:
                    SelectVote(message);
                    break;

                case CommandServer.SubmitVote:
                    SubmitVote(message);
                    break;
            }
            return _messagesOut;
        }

        //Returns all important information to the user
        public List<Message> GetReminderData(Player player)
        {
            List<Message> messages = new List<Message>();

            //Active Player List
            messages.Add(new Message("", CommandClient.PlayerList, AlivePlayersString));

            //If dead, only tell they are dead
            if (!player.alive)
            {
                messages.Add(new Message("", CommandClient.Murdered));
                return messages;
            }

            //State
            Message stateMessage = new Message("", CommandClient.State, "");
            stateMessage.subCommand = state.ToString();
            messages.Add(stateMessage);

            //Role
            messages.Add(new Message("", CommandClient.Role, player.RoleDetails));


            //Remind of vote
            if (state == State.Day)
            {
                Message fauxVoteMessage = new Message("", CommandServer.SelectVote, player.name);
                List<string> voteList = SelectVote(fauxVoteMessage);

                //Find any players ive already selected
                Player? alreadySelectedPlayer = _players.Find(curPlayer => curPlayer.votedBy.Contains(player.name));
                string alreadySelectedName = "";
                if (alreadySelectedPlayer != null)
                {
                    alreadySelectedName = alreadySelectedPlayer.name;
                }

                Message voteListMessage = new Message("", CommandClient.SelectedPlayerList, voteList);
                voteListMessage.subCommand = alreadySelectedName;
                messages.Add(voteListMessage);
            }

            //Remind if submitted
            if (player.ready)
            {
                messages.Add(new Message("", CommandClient.Submitted, ""));
            }

            return messages;
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
            ChangeState(State.Night);

            //Ready up all players
            foreach (Player player in AlivePlayers)
            {

                player.votes = 0;
                player.votedBy = new List<string>();
                //Unready those with tasks
                if (player.role.hasNightTask)
                {
                    player.ready = false;
                }
                else
                {
                    player.ready = true;
                }

                //Mark down each death timer
                if (player.deathTimer > 0)
                {
                    player.deathTimer--;
                }
            }
        }

        public void NightSubmit(Message message)
        {
            Player? player = _players.Find(player => player.name == message.player);
            if (player == null) { return; }
            if (!player.role.hasNightTask) { return; }

            NightTaskResult result = player.role.NightTask(message, AlivePlayers);

            //Secondary message (e.g. Drunk, Spellcaster, Old Hag)
            if(result.secondaryMessage  != null)
            {
                _messagesOut.Add(result.secondaryMessage);
            }

            //Player successfully submitted
            player.ready = true;
            _messagesOut.Add(new Message(player.name, CommandClient.Submitted, result.data));
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
                if (werewolf.role.name == "Sorceress" || werewolf.role.name == "Minion") { continue; }

                _messagesOut.Add(new Message(
                    werewolf.name,
                    CommandClient.SelectedPlayerList,
                    attackList
                    ));
            }

        }

        //Change the phase and notify all players including alive player list
        private void ChangeState(State state)
        {
            this.state = state;
            //Notify host of state
            _messagesOut.Add(new Message(
                "host",
                CommandClient.State,
                state.ToString()
                ));

            //Notify players of player list
            foreach (Player player in AlivePlayers)
            {
                Message newStateMessage = new Message(
                    player.name,
                    CommandClient.State,
                    AlivePlayersString
                    );

                newStateMessage.subCommand = state.ToString();

                _messagesOut.Add(newStateMessage);
            }

            //Reset all players
            foreach (Player player in _players)
            {
                player.Reset();
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

        public void FinishNight()
        {

            //Sanity check, find the player most bitten by werewolves
            Player? murderedPlayer = null;
            int mostVotes = 0;
            foreach (Player player in AlivePlayers)
            {
                //Count werewolves attacking
                if (mostVotes < player.werewolvesAttacking)
                {
                    mostVotes = player.werewolvesAttacking;
                    murderedPlayer = player;
                }

                //Check if player is due to die anyway
                if (player.deathTimer == 0)
                {
                    MurderPlayer(player);
                }
            }

            //Death by werewolf
            if (murderedPlayer != null)
            {
                BitePlayer(murderedPlayer);
            }

            ChangeState(State.Day);
            CheckEndgame(true);
        }

        //Specifically kill player by werewolf
        private void BitePlayer(Player player)
        {
            //Countdown tough guy if not already counting
            if (player.role.name == "Tough Guy" && player.deathTimer < 0)
            {
                player.deathTimer = 1;
            }
            //Turn cursed into werewolf
            else if(player.role.name == "Cursed")
            {
                player.role = new WerewolfRole();
                //Alert player of their new role
                _messagesOut.Add(new Message(
                    player.name,
                    CommandClient.Role,
                    player.RoleDetails
                    ));
            }
            else
            {
                MurderPlayer(player);
            }
        }
        private void MurderPlayer(Player murderedPlayer)
        {
            //Dont kill invincible players
            if (murderedPlayer.invincible) { return; }


            murderedPlayer.alive = false;
            _messagesOut.Add(new Message(murderedPlayer.name, CommandClient.Murdered));

            //Activate the apprentice Seer
            if (murderedPlayer.role.name == "Seer")
            {
                //Check if playing with apprentice
                Player? apprentice = GetPlayerByRole("Apprentice Seer");
                if (apprentice != null)
                {
                    apprentice.role.hasNightTask = true;
                    _messagesOut.Add(new Message(apprentice.name, CommandClient.Role, apprentice.RoleDetails));
                }
            }
        }

        private List<string> SelectVote(Message message)
        {
            List<string> voteList = new List<string>();

            //Find the selected player
            Player? selectedPlayer = _players.Find(player => player.name == message.data[0]);
            if (selectedPlayer == null) { return voteList; }

            //Select/Deselect
            if (message.subCommand == "select")
            {
                //Cant select same person twice
                if (!selectedPlayer.votedBy.Contains(message.player))
                {
                    selectedPlayer.votes++;
                    selectedPlayer.votedBy.Add(message.player);
                }
            }
            else if (message.subCommand == "deselect")
            {
                if (selectedPlayer.votes > 0)
                {
                    selectedPlayer.votes--;
                    selectedPlayer.votedBy.Remove(message.player);
                }
            }

            //Get names of players with how many voting
            foreach (Player player in AlivePlayers)
            {
                voteList.Add($"{player.name};{player.votes}");
            }

            //Notify each werewolf of the new selection
            foreach (Player player in AlivePlayers)
            {
                _messagesOut.Add(new Message(
                    player.name,
                    CommandClient.SelectedPlayerList,
                    voteList
                    ));
            }

            return voteList;
        }

        private void SubmitVote(Message message)
        {
            //Confirm Submission and verify not already voted
            Player? me = _players.Find(player => player.name == message.player);
            if (me == null || me.ready) { return; }
            me.ready = true;

            _messagesOut.Add(new Message(
                message.player,
                CommandClient.Submitted
                ));

            //Find the selected player
            Player? selectedPlayer = _players.Find(player => player.name == message.data[0]);
            if (selectedPlayer == null) { return; }

            selectedPlayer.lockedVotes++;

            //Send locked votes list to host
            List<string> lockedVoteList = new List<string>();
            foreach (Player player in AlivePlayers)
            {
                lockedVoteList.Add($"{player.name};{player.lockedVotes}");
            }
            _messagesOut.Add(new Message(
                "host",
                CommandClient.SelectedPlayerList,
                lockedVoteList
                ));

            //Check for end of voting
            int totalLockedVotes = 0;
            int highestLockedVotes = 0;
            bool isTie = false;
            Player highestVotedPlayer = null;
            foreach (Player player in AlivePlayers)
            {
                totalLockedVotes += player.lockedVotes;
                if (player.lockedVotes > highestLockedVotes)
                {
                    highestLockedVotes = player.lockedVotes;
                    highestVotedPlayer = player;
                }
                else if (player.lockedVotes == highestLockedVotes)
                {
                    isTie = true;
                }
            }

            //No majority
            //and not all players voted
            if (highestLockedVotes <= Math.Floor(AlivePlayers.Count / 2.0))
            {
                if (totalLockedVotes < AlivePlayers.Count)
                {
                    return;
                }
            }


            //At this point the majority is found or all have voted
            if (highestVotedPlayer != null && !isTie)
            {
                MurderPlayer(highestVotedPlayer);
            }

            CheckEndgame(true);
        }

        public Team CheckEndgame(bool canFinalise)
        {
            Team winningTeam = Team.None;

            //Check werewolf win
            if (Werewolves.Count >= AlivePlayers.Count / 2.0)
            {
                winningTeam = Team.Werewolf;
            }

            //Check villager win
            else if (Werewolves.Count == 0)
            {
                winningTeam = Team.Villager;
            }

            if (winningTeam != Team.None)
            {
                if (canFinalise)//When testing this is false
                {
                    Endgame(winningTeam);
                }
            }

            return winningTeam;
        }

        private void Endgame(Team winningTeam)
        {
            //Tell the host
            _messagesOut.Add(new Message(
                "host",
                CommandClient.EndGame,
                winningTeam.ToString()
                ));

            //Tell each player
            foreach (Player player in _players)
            {
                _messagesOut.Add(new Message(
                    player.name,
                    CommandClient.EndGame,
                    winningTeam.ToString()
                    ));
            }

            state = State.Lobby;
        }
    }
}
