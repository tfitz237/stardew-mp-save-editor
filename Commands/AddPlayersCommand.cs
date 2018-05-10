using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using McMaster.Extensions.CommandLineUtils;

namespace StardewValley.MPSaveEditor.Commands {

    [Command(Name = "AddPlayers", Description = "Add Players", ThrowOnUnexpectedArgument = false)]
    public class AddPlayersCommand {

        private const int Success = 0;
        private const int Failure = 2;

        public int newCabinCount {get; set;}
        
        public string saveFilePath { get; set; }
        public int GetAddedPlayerCount() {
            String userSelection = "";
            Console.WriteLine("How many player slots would you like to add? ");
            while(!int.TryParse(userSelection, out var ignore)) {
                userSelection = Console.ReadLine();
            }
            return int.Parse(userSelection);
        }
        
        public static String GetSaveFile(String path) {
            int userSelection = -1;
            Dictionary<int, String> saveFiles = new Dictionary<int, String>();
            Console.WriteLine("---------");
            Console.WriteLine("Save Files");
            while(!saveFiles.ContainsKey(userSelection)) {
                saveFiles = new Dictionary<int, String>();
                int fileCount = 0;
                foreach(String saveFolder in Directory.GetDirectories(path)) {
                    fileCount++;
                    String saveFileName = Regex.Matches(saveFolder, @"[^\\]*$").First().ToString();
                    String saveFilePath = String.Format("{0}/{1}", saveFolder, saveFileName);
                    saveFiles.Add(fileCount, saveFilePath);
                    Console.WriteLine(String.Format("{0}. {1}", fileCount, saveFileName));
                }
                userSelection = Prompt.GetInt("Select a save file:", 0);
            }

            return saveFiles[userSelection];
        }
        public int OnExecute() {
            try {
                saveFilePath = GetSaveFile(String.Format("C:/Users/{0}/AppData/Roaming/StardewValley/Saves", Environment.UserName));
                newCabinCount = Prompt.GetInt("How many player slots would you like to add? ", 1);
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
}