using Werewolf_Server.GameFiles.Roles.Active;
using Werewolf_Server.GameFiles.Roles.Passive;
using Werewolf_Server.GameFiles.Roles.Werewolf;

namespace Werewolf_Server.GameFiles.Modes
{
    public class Mode
    {
        public string name { get; set; }
        public int minPlayerCount { get; set; }
        public List<string> roleNames { get; set; }
        public List<Role> roles { get; set; }

        public void GetRoles(int playerCount)
        {
            //Convert stringed role names to C# roles
            roles = new List<Role>();
            foreach (string roleName in roleNames)
            {
                switch (roleName)
                {
                    case "Werewolf":
                        roles.Add(new Werewolf());
                        break;
                    case "Villager":
                        roles.Add(new Villager());
                        break;
                    case "Seer":
                        roles.Add(new Seer());
                        break;
                    case "ApprenticeSeer":
                        roles.Add(new ApprenticeSeer());
                        break;
                    case "Witch":
                        roles.Add(new Witch());
                        break;
                    case "Mason":
                        roles.Add(new Mason());
                        break;
                    case "Revealer":
                        roles.Add(new Revealer());
                        break;
                    case "Lycan":
                        roles.Add(new Lycan());
                        break;
                    case "Priest":
                        roles.Add(new Priest());
                        break;
                }
            }

            //Pad remaining roles with villagers
            while (playerCount > roles.Count)
            {
                roles.Add(new Villager());
            }

        }
    }
}
