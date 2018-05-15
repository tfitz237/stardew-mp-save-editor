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
    public class FarmhandManagementCommand {
        public string saveFilePath { get; set; }
        
        private SaveGame _game {get;set;}
        private Farmhands _farmhands {get; set;}
         public XElement SelectFarmhand(SaveGame saveGame) {
            Dictionary<string, string> saveFiles = new Dictionary<String, String>();
            Console.WriteLine("---------");
            Console.WriteLine("Current Farmhands: ");
            var farmhands = saveGame.Farmhands;            
            var farmhandNames = saveGame.FarmhandNames;
            
            saveFiles = new Dictionary<string, string>();
            int farmhandCount = 0;
            foreach(var name in farmhandNames) {
                farmhandCount++;
                Console.WriteLine(String.Format("{0}. {1}", farmhandCount, name));
            }
            int farmhandNumber = Prompt.GetInt("Select a farmhand for management:", 1);
            

            return saveGame.GetFarmhandByName(farmhandNames.ElementAt(farmhandNumber - 1));
        }      

        public bool SelectProgram(SaveGame game) {
            _game = game;
            Console.WriteLine("---------");
            Console.WriteLine("Farmhand Management System (FMS)");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Select a farmhand program: ");
            foreach(var command in Commands) {
                Console.WriteLine(string.Format("{0}. {1}", command.Key, command.Value));
            }
            var subProgram = Prompt.GetInt("Choose a program");
            _farmhands =  new Farmhands(game);
            if (subProgram == 1) {
                return AddFarmhand();
            }
            if (subProgram == 2) {
                return RemoveFarmhand();
            }

            return false;
        } 

        public bool AddFarmhand() {
            Console.WriteLine("---------");
            Console.WriteLine("Choose a farmhand: ");
            Console.WriteLine("1. <New Farmhand>");
            bool success = true;
            var farmhands = FindFarmhands(false, 1);
            var farmhandNumber = Prompt.GetInt(Purpose[PurposeEnum.AddToGame], 1);
            if (farmhandNumber == 1) {
                success = _farmhands.AddFarmhand();
                _game.SaveFile();                                
            }
            if (farmhandNumber > 1) {
                var farmhand = farmhands.ElementAt(farmhandNumber - 2);
                success = _farmhands.AddFarmhand(farmhand);
            }
            return success;
        }

        public IEnumerable<Farmhand> FindFarmhands(bool inGame, int startingIndex) {
            var farmhandCount = startingIndex;
            IEnumerable<Farmhand> farmhands; 
            if (inGame) {
                farmhands =  _farmhands.FarmhandsInGame;
            } else {
                farmhands = _farmhands.FarmhandsInStorage;
            }
            foreach (var fh in farmhands) {
                farmhandCount++;
                Console.WriteLine(string.Format("{0}. {1}", farmhandCount, fh.Name));        
            }
            return farmhands;
        }


        public bool RemoveFarmhand() {
            Console.WriteLine("---------");
            Console.WriteLine("Would you like to save the farmhand into storage for future use? ");
            Console.WriteLine("1. Yes");
            Console.WriteLine("2. No");
            var storeFarmhand = Prompt.GetInt("Store removed farmhand in storage? ", 1) == 1;
            var farmhands = FindFarmhands(true, 0);     
            var farmhandNumber = Prompt.GetInt(Purpose[storeFarmhand ? PurposeEnum.RemoveAndStore : PurposeEnum.RemoveNoStore], 1);
            var farmhand = farmhands.ElementAt(farmhandNumber - 1);
            _farmhands.RemoveFarmhandFromCabin(farmhand, storeFarmhand);
            _farmhands.SaveFile();
            return true;
            
        }

        public int OnExecute() {
            try {

                saveFilePath = CommandHelpers.GetSaveFile(CommandHelpers.GetSaveFolder());
                var saveGame = new SaveGame(saveFilePath);
                var result = SelectProgram(saveGame);
                if (result) {
                    Console.WriteLine("Done!");
                } else {
                    Console.WriteLine("Something went wrong...");
                }
                    Console.ReadLine();
                return result ? CommandHelpers.Success : CommandHelpers.Failure;
            } 
            catch (Exception ex) {
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine(ex.StackTrace);
                return CommandHelpers.Failure;                  
            }
        }

        public Dictionary<int, string> Commands = new Dictionary<int, string> {
            {1, "Add farmhand to game"},
            {2, "Remove farmhand from game"},
            {3, "Switch farmhand with host of game"},
            {4, "Switch farmhand with stored farmhand"}
        };


        public Dictionary<PurposeEnum, string> Purpose = new Dictionary<PurposeEnum, string> {
            {PurposeEnum.AddToGame, "Select a farmhand to add to the game:"},
            {PurposeEnum.RemoveAndStore, "Select a farmhand to remove from the game and put in storage: "},
            {PurposeEnum.RemoveNoStore, "Select a farmhand to PERMANANTELY remove from the game: "},
            {PurposeEnum.SwitchFromStorage, "Select a farmhand to retrieve from storage for switching: "},
            {PurposeEnum.SwitchFromGame, "Select a farmhand to switch from the game: "}
        };

        public enum PurposeEnum {
            AddToGame,
            RemoveAndStore,
            RemoveNoStore,
            SwitchFromStorage,
            SwitchFromGame
        }
    }
}