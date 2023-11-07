namespace Werewolf_Server
{
    public class Player
    {
        public Role role;
        public bool alive;
        public bool ready;
        public string name;
        public bool canVote;
        public int votes;
        public int lockedVotes;
        public int werewolvesAttacking;//How many werewolves are currently attacking me
        public List<string> votedBy;
        public bool invincible;
        public int deathTimer;//If above 0 counts down at the start of each night. If 0 dies tonight. < 0 has no timer
        public bool inCult;//Cult Leader shenanigans
        public Player? linkedPlayer;//Cupid 

        public Player(string name, Role role)
        {
            this.role = role;
            this.alive = true;
            this.votes = 0;
            this.lockedVotes = 0;
            this.ready = false;
            this.name = name;
            this.votedBy = new List<string>();
            this.invincible = false;
            this.deathTimer = -1;
            this.inCult = false;
            this.canVote = true;
            this.linkedPlayer = null;
            werewolvesAttacking = 0;
        }
        public List<string> RoleDetails
        {
            get
            {
                return new List<string>() {
                    role.name,                      // 0
                    role.description,               // 1
                    role.team.ToString(),           // 2
                    role.nightDescription,          // 3
                    role.hasNightTask.ToString(),   // 4
                    role.canMultiClick.ToString(),  // 5
                    role.canSelectLast.ToString()   // 6
                };
            }
        }

        public void Reset()
        {
            this.votes = 0;
            this.lockedVotes = 0;
            this.werewolvesAttacking = 0;
            this.votedBy.Clear();
            this.invincible = false;
        }
    }
}
