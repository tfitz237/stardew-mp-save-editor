using System;
using System.Linq;
namespace stardew
{
    class Program
    {
        static void Main(string[] args)
        {   
            var newPlayers = args.Count() > 0 ? Int32.Parse(args[0]) : 1;
            var path = args.Count() > 1 ? args[1].Replace("\\", "/").TrimEnd() : "./samples/TemplateNa_185230783";
            var game = new SaveGame(path);
            
            while (newPlayers > 0) {
                var cabins = game.Cabins;
                var cabinClone = game.CreateNewCabin();
                cabinClone = game.ReplaceCabinName(cabinClone);
                cabinClone = game.ReplaceMultiplayerId(cabinClone);
                //Console.WriteLine(cabinClone);
                game.AddCabin(cabinClone);
                newPlayers--;
            }
            
            game.SaveFile();
        }

    }
}
