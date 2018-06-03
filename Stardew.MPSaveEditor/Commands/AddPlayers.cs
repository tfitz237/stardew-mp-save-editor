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
                saveFilePath = CommandHelpers.GetSaveFile(CommandHelpers.GetSaveFolder());
                newCabinCount = Prompt.GetInt("How many player slots would you like to add? ", 1);
                var game = new SaveGame(saveFilePath);
                while (newCabinCount > 0) {
                    var cabins = game.Cabins;
                    game.CreateNewCabin();
                    newCabinCount--;
                }

                CommandHelpers.SaveFile(game); 
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