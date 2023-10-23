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

        public void RecieveMessage(WebSocket ws, string[] message)
        {
            List<NamedMessage> result;

            switch (message[0])
            {
                //Init
                case "Host":
                    _host = new Connection("host", ws);
                    _host.Broadcast("HostFound;");
                    _game = new Game();
                    break;
                case "Join":
                    AddConnection(ws, message[1]);
                    break;
                case "Leave":
                    RemoveConnection(message[1]);
                    break;

                //Lobby or refresh
                case "RemindState":
                    RemindState(message[1]);
                    break;
                case "Start":
                    result = _game.Start(_connections);
                    ResolveGameResult(result);
                    break;

                //Anything else should be apart of the game
                default:
                    if (_game != null)
                    {
                        result = _game.Update(message[1]);
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


        private void BroadcastAll(string message)
        {
            if (_host != null)
            {
                _host.Broadcast(message);
            }
            BroadcastUsers(message);
        }

        private void BroadcastUsers(string message)
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

            if (existingPlayerId != -1)
            {
                //Player already exists, update socket
                _connections[existingPlayerId].socket = ws;
                conn = _connections[existingPlayerId];
            }
            else
            {
                //Add new player
                _connections.Add(new Connection(name, ws));
                conn = _connections.Last();
            }

            //Refresh player list
            BroadcastAll("Players;" + GetPlayerList());

            //Confirm success with latest connection
            conn.Broadcast("Joined;");

        }

        private void RemoveConnection(string name)
        {
            Connection removeMe = _connections.Find(player => player.connectionName == name);
            removeMe.Broadcast("Left;");
            _connections.Remove(removeMe);

            //Update player list to all
            BroadcastAll("Players;" + GetPlayerList());
        }

        public string GetPlayerList()
        {

            string output = "";
            foreach (Connection conn in _connections)
            {
                output += conn.connectionName;
                output += ",";
            }
            if (output.Contains(","))
            {
                //Remove trailing comma
                output = output.Remove(output.Length - 1);
            }

            return output;
        }

        private void ResolveGameResult(List<NamedMessage> result)
        {
            //Broadcast each player message to the respective connection
            foreach (NamedMessage namedMessage in result)
            {
                Connection? conn;
                if (namedMessage.connectionName == "host")
                {
                    conn = _host;
                }
                else
                {
                    //Find the respective connection
                    conn = _connections.Find(conn => conn.connectionName == namedMessage.connectionName);
                }

                if (conn != null)
                {
                    conn.Broadcast(namedMessage.message);
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

            if (_game != null)//If game is started
            {
                //Broadcast role
                user.Broadcast(_game.GetReminderData(user.player));
            }
            else
            {
                user.Broadcast("Players;" + GetPlayerList());
            }
        }

    }
}
