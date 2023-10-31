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
