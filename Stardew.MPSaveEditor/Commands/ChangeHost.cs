// Deprecated: see SwitchFarmhandByHost in FarmhandMangement.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using McMaster.Extensions.CommandLineUtils;
using StardewValley.MPSaveEditor.Models;
using StardewValley.MPSaveEditor.Helpers;

namespace StardewValley.MPSaveEditor.Commands
{
    [Command(Name = "AddPlayers", Description = "Add Players", ThrowOnUnexpectedArgument = false)]
    public class ChangeHostCommand {
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

                saveFilePath = CommandHelpers.GetSaveFile(CommandHelpers.GetSaveFolder());
                var game = new SaveGame(saveFilePath);
                var farmhand = SelectFarmhand(game);
                game.SwitchHost(farmhand);
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