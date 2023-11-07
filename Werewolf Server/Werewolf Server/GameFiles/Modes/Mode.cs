using Werewolf_Server.GameFiles.Roles.Active;
using Werewolf_Server.GameFiles.Roles.Manipulated;
using Werewolf_Server.GameFiles.Roles.Manipulator;
using Werewolf_Server.GameFiles.Roles.Protector;
using Werewolf_Server.GameFiles.Roles.Seer;
using Werewolf_Server.GameFiles.Roles.Villager;
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
                    //Passive
                    case "Villager":
                        roles.Add(new VillagerRole());
                        break;
                    case "Mason":
                        roles.Add(new Mason());
                        break;
                    case "Tanner":
                        roles.Add(new Tanner());
                        break;
                    case "Mayor":
                        roles.Add(new Mayor());
                        break;

                    //Werewolf
                    case "Werewolf":
                        roles.Add(new WerewolfRole());
                        break;
                    case "Sorceress":
                        roles.Add(new Sorceress());
                        break;
                    case "Minion":
                        roles.Add(new Minion());
                        break;

                    //Seer
                    case "Seer":
                        roles.Add(new Seer());
                        break;
                    case "Mystic Seer":
                        roles.Add(new MysticSeer());
                        break;
                    case "Apprentice Seer":
                        roles.Add(new ApprenticeSeer());
                        break;
                    case "Mentalist":
                        roles.Add(new Mentalist());
                        break;
                    case "Revealer":
                        roles.Add(new Revealer());
                        break;
                    case "Aura Seer":
                        roles.Add(new AuraSeer());
                        break;

                    //Protector
                    case "Bodyguard":
                        roles.Add(new Bodyguard());
                        break;
                    case "Priest":
                        roles.Add(new Priest());
                        break;

                    //Manipulator
                    case "Witch":
                        roles.Add(new Witch());
                        break;
                    case "Cult Leader":
                        roles.Add(new CultLeader());
                        break;
                    case "Diseased":
                        roles.Add(new Diseased());
                        break;
                    case "Old Hag":
                        roles.Add(new OldHag());
                        break;
                    case "Spellcaster":
                        roles.Add(new Spellcaster());
                        break;
                    case "Cupid":
                        roles.Add(new Cupid());
                        break;

                    //Manipulated
                    case "Tough Guy":
                        roles.Add(new ToughGuy());
                        break;
                    case "Cursed":
                        roles.Add(new Cursed());
                        break;
                    case "Drunk":
                        roles.Add(new Drunk());
                        break;
                    case "Lycan":
                        roles.Add(new Lycan());
                        break;

                    //Killer
                    case "Huntress":
                        roles.Add(new Huntress());
                        break;
                }
            }

            //Pad remaining roles with villagers
            while (playerCount > roles.Count)
            {
                roles.Add(new VillagerRole());
            }

        }
    }
}
