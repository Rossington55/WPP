using System.Text.Json;

namespace Werewolf_Server
{
    public abstract class MessageData
    {
        public string Command { get; set; }
        public string Sender { get; set; }

        public MessageData(string command, string sender) {
            this.Command = command;
            this.Sender = sender;    
        }

        public string serialize()
        {
            string jsonString = JsonSerializer.Serialize(this);
            return jsonString;
        }

    }
}
