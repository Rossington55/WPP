namespace Werewolf_Server.GameFiles
{
    public class NamedMessage
    {
        public string connectionName { get; set; }
        public string message { get; set; }

        public NamedMessage(string connectionName, string message)
        {
            this.connectionName = connectionName;
            this.message = message;
        }
    }
}
