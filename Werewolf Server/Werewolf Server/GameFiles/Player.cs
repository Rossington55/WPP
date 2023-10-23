
namespace Werewolf_Server
{
    public class Player
    {
        public Role role;
        public bool alive;
        public int votes;
        public bool ready;
        public string name;

        public Player(string name, Role role)
        {
            this.role = role;
            this.alive = true;
            this.votes = 0;
            this.ready = false;
            this.name = name;
        }
        public string RoleString { get
            {
                return $"Role;{role.name}," +
                    $"{role.description}," +
                    $"{role.team}," +
                    $"{role.nightDescription}," +
                    $"{role.hasNightTask}," +
                    $"{role.canMultiClick}";
            }
        }
    }
}
