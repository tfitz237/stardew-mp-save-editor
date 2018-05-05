using System;
using System.Xml.Linq;
using System.Linq;

namespace stardew
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = args.Count() > 0 ? args[0].Replace("\\", "/").TrimEnd() : "./samples/TemplateNa_185230783";
            XNamespace ns = "http://www.w3.org/2001/XMLSchema-instance";
            var d = XDocument.Load(path).Element("SaveGame").Elements();
            var player = d.First(x => x.Name == "player");
            var locations = d.First(x => x.Name == "locations").Elements();
            var farm = locations.Single(x => x.Attribute(ns + "type")?.Value == "Farm");
            var buildings = farm.Element("buildings").Elements();
            var cabins = buildings.Where(x => x.Element("indoors").Attribute(ns + "type")?.Value == "Cabin");
            var blankCabin = cabins.First(x => x.Element("indoors")
                                            .Element("farmhand")
                                            .Element("name").IsEmpty
                                        );
            var blankFarmHand = blankCabin.Element("indoors").Element("farmhand");
            
            Console.WriteLine(blankCabin.ToString());
        }
    }
}
