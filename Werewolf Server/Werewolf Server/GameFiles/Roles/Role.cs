namespace Werewolf_Server
{
    public enum Team { Villager, Werewolf, Tanner, Vampire }
    public abstract class Role
    {
        public string name;
        public string description;
        public string nightDescription;//What to do at night
        public Team team;
        public bool hasNightTask;
        public bool canMultiClick;

        //Return info (e.g. seer), otherwise return empty
        public abstract string NightTask(List<Player> selectedPlayers);


    }
}
