using System.Net.WebSockets;
using System.Text;
using Werewolf_Server.GameFiles;

namespace Werewolf_Server
{
    public class GameManager
    {
        private Connection? _host;
        private List<Connection> _connections;
        private Game _game;

        public GameManager()
        {
            _connections = new List<Connection>();
        }

        public void RecieveMessage(WebSocket ws, Message message)
        {
            List<Message> result;

            switch (message.commandServer)
            {
                //Init
                case CommandServer.Host:
                    _host = new Connection("host", ws);
                    _host.Broadcast(new Message(CommandClient.HostFound));
                    break;
                case CommandServer.Join:
                    AddConnection(ws, message.player);
                    break;
                case CommandServer.Leave:
                    RemoveConnection(message.player);
                    break;

                //Lobby or refresh
                case CommandServer.RemindState:
                    RemindState(message.player);
                    break;
                case CommandServer.Start:
                    _game = new Game();
                    result = _game.Start(_connections, message.subCommand);
                    ResolveGameResult(result);
                    break;
                case CommandServer.Close:
                    _game = null;
                    _connections.Clear();
                    break;

                //Anything else should be apart of the game
                default:
                    if (_game != null)
                    {
                        result = _game.Update(message);
                        ResolveGameResult(result);
                    }
                    break;

                    /*                //Start

                                    //Night
                                    case "StartNight":
                                        BroadcastAll("State;Night");
                                        _game.state = State.Night;
                                        _game.NightInit();
                                        break;
                                    case "WerewolfClick":
                                        _game.WerewolfClick(message[1], message[2], message[3] == "true");
                                        break;
                                    case "NightSubmit":
                                        _game.NightSubmit(message[1], message[2]);
                                        break;
                    */
            }
        }


        private void BroadcastAll(Message message)
        {
            if (_host != null)
            {
                _host.Broadcast(message);
            }
            BroadcastUsers(message);
        }

        private void BroadcastUsers(Message message)
        {
            foreach (Connection conn in _connections)
            {
                conn.Broadcast(message);
            }
        }

        private void AddConnection(WebSocket ws, string name)
        {
            //Check for reconnect
            int existingPlayerId = _connections.FindIndex(Connection => Connection.connectionName == name);
            Connection conn;


            if (existingPlayerId != -1)//Existing Player
            {
                //Player already exists, update socket
                //Kick out the old socket
                _connections[existingPlayerId].Broadcast(new Message(
               CommandClient.Left
                ));

                _connections[existingPlayerId].socket = ws;
                conn = _connections[existingPlayerId];
            }
            else//New player
            {
                //Dont allow in if game's already started
                if (_game != null && _game.state != State.Lobby)
                {
                    conn = new Connection(name, ws);
                    conn.Broadcast(new Message(
                        name,
                            CommandClient.None,
                            ""
                        ));
                    return;
                }
                //Add new player
                _connections.Add(new Connection(name, ws));
                conn = _connections.Last();
            }

            //Refresh player list (when not in play)
            if (_game == null || _game.state == State.Lobby)
            {
                BroadcastAll(new Message(
                    CommandClient.PlayerList,
                    GetPlayerList()
                    ));
            }

            //Confirm success with latest connection
            conn.Broadcast(new Message(CommandClient.Joined));

        }

        private void RemoveConnection(string name)
        {
            Connection removeMe = _connections.Find(player => player.connectionName == name);
            removeMe.Broadcast(new Message(
               CommandClient.Left
                ));
            _connections.Remove(removeMe);

            //Update player list to all
            BroadcastAll(new Message(
               CommandClient.PlayerList,
               GetPlayerList()
                ));
        }

        public List<string> GetPlayerList()
        {
            List<string> playerList = new List<string>();
            foreach (Connection conn in _connections)
            {
                playerList.Add(conn.connectionName);
            }

            return playerList;
        }

        private void ResolveGameResult(List<Message> result)
        {
            //Broadcast each player message to the respective connection
            foreach (Message message in result)
            {
                Connection? conn;
                if (message.player == "host")
                {
                    conn = _host;
                }
                else
                {
                    //Find the respective connection
                    conn = _connections.Find(conn => conn.connectionName == message.player);
                }

                if (conn != null)
                {
                    Thread.Sleep(50);
                    conn.Broadcast(message);
                }

            }
        }

        //Remind role, and state if applicable
        private void RemindState(string name)
        {
            //Find the correct connection
            int existingUserId = _connections.FindIndex(Connection => Connection.connectionName == name);
            if (existingUserId == -1) { return; }
            Connection user = _connections[existingUserId];

            if (_game != null && _game.state != State.Lobby)//If game is started
            {
                List<Message> result = _game.GetReminderData(user.player);
                foreach (Message message in result)
                {
                    Thread.Sleep(50);
                    user.Broadcast(message);
                }
            }
            else
            {
                //Just broadcast the player list
                user.Broadcast(new Message(
                CommandClient.PlayerList,
                GetPlayerList()
                ));
            }
        }

    }
}
