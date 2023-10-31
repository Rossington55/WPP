namespace Werewolf_Server
{
    public enum CommandServer { Join, Leave, GetPlayers, Host, Start, RemindState, StartNight, WerewolfSelectPlayer, NightSubmit }
    public enum CommandClient
    {
        Connected,
        Joined,
        Left,
        HostFound,
        PlayerList,
        Role,
        WerewolfSelectedPlayerList,
        Submitted,
        Murdered,
        State
    }


    public class Message
    {
        public CommandServer commandServer { get; set; }
        public CommandClient commandClient { get; set; }
        public string subCommand { get; set; }
        public string player { get; set; }
        public List<string> data { get; set; }


        public Message(string player, CommandClient command, List<string> data)
        {
            this.player = player;
            this.data = data;
            this.commandClient = command;
        }
        public Message(string player, CommandServer command, List<string> data)
        {
            this.player = player;
            this.commandServer = command;
        }
        public Message(string player, CommandServer command, string data)
        {
            this.player = player;
            this.commandServer = command;
            this.data = new List<string> { data };
        }
        public Message(string player, CommandClient command, string data)
        {
            this.player = player;
            this.commandClient = command;
            this.data = new List<string> { data };
        }
        public Message(string player, CommandClient command)  {
            this.player = player;
            this.commandClient = command;
        }


        public Message(CommandClient command, List<string> data) {
            this.commandClient = command;
            this.data = data;
        }
        public Message(CommandClient command) {
            this.commandClient = command;
        }

    }
}
