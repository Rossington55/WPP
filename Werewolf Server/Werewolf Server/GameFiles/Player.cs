
namespace Werewolf_Server
{
    public class Player
    {
        public Role role;
        public bool alive;
        public int votes;
        public bool ready;
        public string name;
        public int werewolvesAttacking;//How many werewolves are currently attacking me

        public Player(string name, Role role)
        {
            this.role = role;
            this.alive = true;
            this.votes = 0;
            this.ready = false;
            this.name = name;
            werewolvesAttacking = 0;
        }
        public List<string> RoleDetails
        {
            get
            {
                return new List<string>() {
                    role.name,                  //0
                    role.description,           //1
                    role.team.ToString(),       //2
                    role.nightDescription,      //3
                role.hasNightTask.ToString(),   //4
                role.canMultiClick.ToString()   //5
                };
            }
        }
    }
}
