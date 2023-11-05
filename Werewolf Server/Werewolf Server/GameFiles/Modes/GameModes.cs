using System.Text.Json;

namespace Werewolf_Server.GameFiles.Modes
{
    public class GameModes
    {
        public Mode currentMode { get; set; }
        List<Mode>? modes { get; set; }

        public GameModes()
        {
            string json = File.ReadAllText("./GameFiles/Modes/GameModes.json");
            modes = JsonSerializer.Deserialize<List<Mode>>(json);
            currentMode = new Mode();
        }

        public Mode GetMode(int playerCount, string modeName)
        {
            if (modeName == "")
            {
                //Select a random mode that matches player count
                currentMode = getRandomMode(playerCount);
            }
            //Add roles as custom
            else if (modeName.Contains("Custom"))
            {
                List<string> roleNames = modeName.Split(";").ToList();
                roleNames.RemoveAt(0);//Remove the word 'custom'
                currentMode = new Mode();
                currentMode.roleNames = roleNames;
            }
            else
            {
                //Find the mode by name
                currentMode = modes.Find(possibleMode => possibleMode.name == modeName);
                if (currentMode == null)
                {
                    currentMode = getRandomMode(playerCount);
                }
            }

            currentMode.GetRoles(playerCount);
            return currentMode;
        }

        private Mode getRandomMode(int playerCount)
        {
            //Find highest mode with minimum players = playercount
            int highestPlayerCount = 0;
            for (int i = modes.Count - 1; i >= 0; i--)
            {
                if (modes[i].minPlayerCount <= playerCount)
                {
                    highestPlayerCount = modes[i].minPlayerCount;
                    break;
                }
            }

            //Select random of the possible modes
            List<Mode> possibleModes = modes.Where(mode => mode.minPlayerCount == highestPlayerCount).ToList();
            Random random = new Random();
            int rand = random.Next(0, possibleModes.Count - 1);
            return possibleModes[rand];
        }
    }
}
