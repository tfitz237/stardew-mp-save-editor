using System;
using System.Linq;
namespace stardew
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = args.Count() > 0 ? args[0].Replace("\\", "/").TrimEnd() : "./samples/TemplateNa_185230783";
            var game = new SaveGame(path);
            var cabins = game.Cabins;
            var cabinClone = game.CreateNewCabin();
            Console.WriteLine(cabinClone);
            game.AddCabin(cabinClone);                     
            game.SaveFile();
        }

    }
}
