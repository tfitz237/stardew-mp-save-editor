using System;
using ManyConsole;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace stardew {

    public class AddPlayersCommand : ConsoleCommand {

        private const int Success = 0;
        private const int Failure = 2;

        public int? newCabinCount { get; set;}
        public string saveFilePath { get; set; }
        public AddPlayersCommand() {
            IsCommand("AddPlayers", "Add Players to Save File");
            HasLongDescription("Add more players/cabins to a MP game. Specify # of players and save file");
            HasOption("p|players=", "Specify the number of players  to add to the MP save",
            t => newCabinCount = t != null ? 
                Convert.ToInt16(t) : 
                (int?)null
            );
            HasOption("s|save=", "Save File Path", 
            s => saveFilePath = s != null ? 
                s.Replace("\\", "/").TrimEnd() : 
                null
            );
        }
        public int GetAddedPlayerCount() {
            String userSelection = "";
            Console.WriteLine("How many player slots would you like to add? ");
            while(!int.TryParse(userSelection, out var ignore)) {
                userSelection = Console.ReadLine();
            }
            return int.Parse(userSelection);
        }
        
        public static String GetSaveFile(String path) {
            String userSelection = "";
            Dictionary<String, String> saveFiles = new Dictionary<String, String>();
            
            Console.WriteLine("Select a save file: ");
            while(!saveFiles.ContainsKey(userSelection)) {
                saveFiles = new Dictionary<String, String>();
                int fileCount = 0;
                foreach(String saveFolder in Directory.GetDirectories(path)) {
                    fileCount++;
                    String saveFileName = Regex.Matches(saveFolder, @"[^\\]*$").First().ToString();
                    String saveFilePath = String.Format("{0}/{1}", saveFolder, saveFileName);
                    saveFiles.Add(fileCount.ToString(), saveFilePath);
                    Console.WriteLine(String.Format("{0}. {1}", fileCount, saveFileName));
                }
                userSelection = Console.ReadLine();
            }

            return saveFiles[userSelection];
        }
        public override int Run(string[] remainingArguments) {
            try {
                saveFilePath = saveFilePath ?? GetSaveFile(String.Format("C:/Users/{0}/AppData/Roaming/StardewValley/Saves", Environment.UserName));
                newCabinCount = newCabinCount ?? GetAddedPlayerCount();
                var game = new SaveGame(saveFilePath);
                while (newCabinCount > 0) {
                    var cabins = game.Cabins;
                    game.CreateNewCabin();
                    newCabinCount--;
                }

                game.SaveFile();
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