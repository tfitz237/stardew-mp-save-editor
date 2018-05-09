using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ManyConsole;
namespace stardew
{
    class Program
    {



        static int Main(string[] args)
        {   
            var commands = GetCommands();

            return ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);


            // var newPlayers = args.Count() > 0 && Int32.TryParse(args[0], out var ignore) ? Int32.Parse(args[0]) : GetAddedPlayerCount();
            // var path = args.Count() > 1 ? args[1].Replace("/", "/").TrimEnd() : GetSaveFile(String.Format("C:/Users/{0}/AppData/Roaming/StardewValley/Saves", Environment.UserName));
            
            // var game = new SaveGame(path);
            // while (newPlayers > 0) {
            //     var cabins = game.Cabins;
            //     var cabinClone = game.CreateNewCabin();
            //     cabinClone = game.ReplaceCabinName(cabinClone);
            //     cabinClone = game.ReplaceMultiplayerId(cabinClone);
            //     cabinClone = game.GetCabinType(cabinClone);
            //     //Console.WriteLine(cabinClone);
            //     game.AddCabin(cabinClone);
            //     newPlayers--;
            // }
            
            // game.SaveFile();

        }
        public static IEnumerable<ConsoleCommand> GetCommands()
        {
            return ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
        }
    }
}
