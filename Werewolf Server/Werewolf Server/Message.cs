using System.Text.Json.Serialization;

namespace Werewolf_Server
{
    public enum CommandServer
    {
        None,                   //0
        Join,                   //1
        Leave,                  //2
        GetPlayers,             //3
        Host,                   //4
        Start,                  //5
        RemindState,            //6
        StartNight,             //7
        WerewolfSelectPlayer,   //8 
        NightSubmit,            //9
        SelectVote,             //10 Choosse someone to vote but not locked in
        SubmitVote,             //11 Lock in a vote
        StartDay,             //11 Lock in a vote
    }
    public enum CommandClient
    {
        None,               //0
        Connected,          //1
        Joined,             //2
        Left,               //3 
        HostFound,          //4
        PlayerList,         //5
        Role,               //6
        SelectedPlayerList, //7
        Submitted,          //8
        Murdered,           //9
        State,              //10
        EndGame,            //11
        Alert,              //12 - Used by spellcaster/old hag
        StartingGame,       //13
    }


    public class Message
    {
        public CommandServer commandServer { get; set; }
        public CommandClient commandClient { get; set; }
        public string subCommand { get; set; }
        public string player { get; set; }
        public List<string> data { get; set; }

        [JsonConstructor]
        public Message() { }

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
        public Message(string player, CommandClient command)
        {
            this.player = player;
            this.commandClient = command;
        }


        public Message(CommandClient command, List<string> data)
        {
            this.commandClient = command;
            this.data = data;
        }
        public Message(CommandClient command)
        {
            this.commandClient = command;
        }

    }
}
