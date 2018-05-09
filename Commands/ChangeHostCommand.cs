using System;
using ManyConsole;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace StardewValley.MPSaveEditor.Commands
{

    public class ChangeHostCommand : ConsoleCommand {

        private const int Success = 0;
        private const int Failure = 2;

        public string saveFilePath { get; set; }
        public ChangeHostCommand() {
            IsCommand("ChangeHost", "Switch the hosts of a save file");
            HasLongDescription("Switch a cabin's farmhand with the host's slot");
            HasOption("s|save=", "Save File Path",
                s => saveFilePath = s);
        }

         public XElement SelectFarmhand(SaveGame saveGame) {
            string userSelection = "null";
            int farmhandNumber;
            Dictionary<string, string> saveFiles = new Dictionary<String, String>();
            
            Console.WriteLine("Select a farmhand: ");
            var farmhands = saveGame.Farmhands;            
            var farmhandNames = saveGame.FarmhandNames;
            
            saveFiles = new Dictionary<string, string>();
            int farmhandCount = 0;
            foreach(var name in farmhandNames) {
                farmhandCount++;
                Console.WriteLine(String.Format("{0}. {1}", farmhandCount, name));
            }
            while (!Int32.TryParse(userSelection, out farmhandNumber)) {
                userSelection = Console.ReadLine();
            }
            

            return saveGame.GetFarmhandByName(farmhandNames.ElementAt(farmhandNumber - 1));
        }       

        public override int Run(string[] remainingArguments) {
            try {
                saveFilePath = saveFilePath ?? AddPlayersCommand.GetSaveFile(String.Format("C:/Users/{0}/AppData/Roaming/StardewValley/Saves", Environment.UserName));
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