using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using McMaster.Extensions.CommandLineUtils;

namespace StardewValley.MPSaveEditor.Commands
{
    [Command(Name = "AddPlayers", Description = "Add Players", ThrowOnUnexpectedArgument = false)]
    public class ChangeHostCommand {

        private const int Success = 0;
        private const int Failure = 2;

        public string saveFilePath { get; set; }

         public XElement SelectFarmhand(SaveGame saveGame) {
            Dictionary<string, string> saveFiles = new Dictionary<String, String>();
            Console.WriteLine("---------");
            Console.WriteLine("Farmhand: ");
            var farmhands = saveGame.Farmhands;            
            var farmhandNames = saveGame.FarmhandNames;
            
            saveFiles = new Dictionary<string, string>();
            int farmhandCount = 0;
            foreach(var name in farmhandNames) {
                farmhandCount++;
                Console.WriteLine(String.Format("{0}. {1}", farmhandCount, name));
            }
            int farmhandNumber = Prompt.GetInt("Select a farmhand:", 1);
            

            return saveGame.GetFarmhandByName(farmhandNames.ElementAt(farmhandNumber - 1));
        }       

        public int OnExecute() {
            try {
                saveFilePath = AddPlayersCommand.GetSaveFile(String.Format("C:/Users/{0}/AppData/Roaming/StardewValley/Saves", Environment.UserName));
                var saveGame = new SaveGame(saveFilePath);
                var farmhand = SelectFarmhand(saveGame);
                saveGame.SwitchHost(farmhand);
                saveGame.SaveFile();
                Console.Write("Done!");
                Console.ReadLine();
                return Success;
            } 
            catch (Exception ex) {
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine(ex.StackTrace);
                return Failure;                  
            }
        }
    }
}