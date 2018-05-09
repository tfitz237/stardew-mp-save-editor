using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace stardew
{
    class Program
    {
        static int GetAddedPlayerCount() {
            String userSelection = "";
            Console.WriteLine("How many player slots would you like to add? ");
            while(!int.TryParse(userSelection, out var ignore)) {
                userSelection = Console.ReadLine();
            }
            return int.Parse(userSelection);
        }

        static String GetSaveFile(String path) {
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

        static void Main(string[] args)
        {   
            var newPlayers = args.Count() > 0 && Int32.TryParse(args[0], out var ignore) ? Int32.Parse(args[0]) : GetAddedPlayerCount();
            var path = args.Count() > 1 ? args[1].Replace("/", "/").TrimEnd() : GetSaveFile(String.Format("C:/Users/{0}/AppData/Roaming/StardewValley/Saves", Environment.UserName));
            
            var game = new SaveGame(path);
            while (newPlayers > 0) {
                var cabins = game.Cabins;
                var cabinClone = game.CreateNewCabin();
                cabinClone = game.ReplaceCabinName(cabinClone);
                cabinClone = game.ReplaceMultiplayerId(cabinClone);
                cabinClone = game.GetCabinType(cabinClone);
                //Console.WriteLine(cabinClone);
                game.AddCabin(cabinClone);
                newPlayers--;
            }
            
            game.SaveFile();
            Console.Write("Done!");
            Console.ReadLine();
        }

    }
}
