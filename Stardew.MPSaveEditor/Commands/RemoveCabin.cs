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

    [Command(Name = "RemoveCabin", Description = "Remove Cabin", ThrowOnUnexpectedArgument = false)]
    public class RemoveCabinCommand {



        public int newCabinCount {get; set;}
        
        public string saveFilePath { get; set; }        
        
        public int OnExecute() {
            try {
                saveFilePath = CommandHelpers.GetSaveFile(CommandHelpers.GetSaveFolder());
                var game = new SaveGame(saveFilePath);
                var selectedCabin = SelectCabin(game);
                game.RemoveCabin(selectedCabin);
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
        public XElement SelectCabin(SaveGame saveGame) {
            Dictionary<string, string> saveFiles = new Dictionary<String, String>();
            Console.WriteLine("---------");
            Console.WriteLine("Cabins: ");
            var cabins = saveGame.Cabins;            
            
            saveFiles = new Dictionary<string, string>();
            int cabinCount = 0;
            foreach(var cabin in cabins) {
                cabinCount++;
                var cabinXY = $"X:{cabin.Element("tileX").Value},Y:{cabin.Element("tileY").Value}";
                var farmhandName = cabin.Element("indoors").Element("farmhand").Element("name").IsEmpty ? "None" : cabin.Element("indoors").Element("farmhand").Element("name").Value ;
                Console.WriteLine(String.Format("{0}. Cabin - {1} - Farmhand: {2}", cabinCount, cabinXY, farmhandName));
            }
            int farmhandNumber = Prompt.GetInt("Select a cabin to be removed:", 1);
            

            return cabins.ElementAt(farmhandNumber - 1);
        }     
    }
}