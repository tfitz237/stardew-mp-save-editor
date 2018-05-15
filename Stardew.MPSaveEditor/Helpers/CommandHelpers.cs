using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using McMaster.Extensions.CommandLineUtils;
using StardewValley.MPSaveEditor.Models;

namespace StardewValley.MPSaveEditor.Helpers
{   
    public static class CommandHelpers {
        public static int Success = 0;
        public static int Failure = 2;
        
        public static String GetSaveFolder() {
            String saveFolderPath;
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows)) {
                saveFolderPath = String.Format("{0}Users/{1}/AppData/Roaming/StardewValley/Saves",
                    Path.GetPathRoot(Environment.SystemDirectory).Replace("\\", "/"), Environment.UserName);
            }
            else {
                saveFolderPath = String.Format("home/{0}/.config/StardewValley/Saves", Environment.UserName);
            }
            return saveFolderPath;
        }

        public static String GetSaveFile(String path) {
            int userSelection = -1;
            Dictionary<int, String> saveFiles = new Dictionary<int, String>();
            Console.WriteLine("---------");
            Console.WriteLine("Save Files");
            while (!saveFiles.ContainsKey(userSelection)) {
                saveFiles = new Dictionary<int, String>();
                int fileCount = 0;
                foreach (String saveFolder in Directory.GetDirectories(path)) {
                    fileCount++;
                    String saveFileName = Regex.Matches(saveFolder, @"[^\\]*$").First().ToString();
                    String saveFilePath = String.Format("{0}/{1}", saveFolder, saveFileName).Replace("\\", "/");
                    saveFiles.Add(fileCount, saveFilePath);
                    Console.WriteLine(String.Format("{0}. {1}", fileCount, saveFileName));
                }
                userSelection = Prompt.GetInt("Select a save file:", 1);
            }

            return saveFiles[userSelection];
        }

        public static void SaveFile(SaveGame game) {
            Console.WriteLine("---------");
            Console.WriteLine("Would you like to overwrite the save file in the Stardew Valley/Saves folder?");
            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine("-- There will always be an ORIGINAL file saved in Stardew.MPSaveEditor/saves directory (by timestamp).");
            Console.WriteLine("-- If this is a SP game converted to MP, this overwrite will ensure your SP game will be hostable")
            Console.WriteLine("-- Type 'yes' without the quotes to overwrite.");
            Console.WriteLine("-- Type anything else (or nothing) to not overwrite the save file.");
            
            Console.Write("Overwrite file? ");
            var overwriteFile = false;
            if (Console.ReadLine().ToLower() == "yes") {
                overwriteFile = true;
            }
            game.SaveFile(overwriteFile);
            Console.WriteLine("-------------------------------------------------------------------------------");
            if (overwriteFile) {
                Console.WriteLine("Game Saved to your local Stardew Valley/Saves folder.");
                Console.WriteLine("ORIGINAL Backup can be found in Stardew.MPSaveEditor/saves");
            } else {
                Console.WriteLine("Game saved to the Stardew.MPSaveEditor/saves");
                Console.WriteLine("Copy the file over to your local StardewValley/Saves folder to see the changes.");
                Console.WriteLine("ORIGINAL Backup can be found in Stardew.MPSaveEditor/saves");
            }
            Console.ReadLine();

        }


    }

}