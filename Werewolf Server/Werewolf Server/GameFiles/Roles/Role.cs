namespace Werewolf_Server
{
    public struct NightTaskResult
    {
        public List<string> data;
        public Message? secondaryMessage;
        public NightTaskResult()
        {
            this.data = new List<string>();
        }
    }

    public enum Team { None, Villager, Werewolf, Tanner, Vampire }
    public abstract class Role
    {
        public string name;
        public string description;
        public string nightDescription;//What to do at night
        public Team team;
        public bool hasNightTask;
        public bool canMultiClick;
        public bool canSelectLast = true;

        //Return info (e.g. seer), otherwise return empty
        public abstract NightTaskResult NightTask(Message message, List<Player> currentPlayers);


    }
}
