using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;


namespace StardewValley.MPSaveEditor.Commands {

    public class AddPlayersCommand {

        private const int Success = 0;
        private const int Failure = 2;

        public int newCabinCount;
        public string saveFilePath { get; set; }
        public AddPlayersCommand(string cabinCount, string filePath) {
            Int32.TryParse(cabinCount, out newCabinCount);
            saveFilePath = filePath?.Replace("\\", "/")?.TrimEnd();
            
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
        public int Run() {
            try {
                saveFilePath = saveFilePath ?? GetSaveFile(String.Format("C:/Users/{0}/AppData/Roaming/StardewValley/Saves", Environment.UserName));
                newCabinCount = newCabinCount != 0 ? newCabinCount : GetAddedPlayerCount();
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