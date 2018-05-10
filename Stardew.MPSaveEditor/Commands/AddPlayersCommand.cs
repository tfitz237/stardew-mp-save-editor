using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using McMaster.Extensions.CommandLineUtils;
using StardewValley.MPSaveEditor.Models;
using StardewValley.MPSaveEditor.Helpers;

namespace StardewValley.MPSaveEditor.Commands {

    [Command(Name = "AddPlayers", Description = "Add Players", ThrowOnUnexpectedArgument = false)]
    public class AddPlayersCommand {



        public int newCabinCount {get; set;}
        
        public string saveFilePath { get; set; }        
        
        public int OnExecute() {
            try {
                saveFilePath = CommandHelpers.GetSaveFile(String.Format("C:/Users/{0}/AppData/Roaming/StardewValley/Saves", Environment.UserName));
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
                return CommandHelpers.Success;
            }
            catch (Exception ex) {
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine(ex.StackTrace);
                return CommandHelpers.Failure;     
            }
        }

    }
}