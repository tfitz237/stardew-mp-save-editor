using System;
using System.Xml.Linq;
using System.Linq;

namespace stardew
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = args[0].Replace("\\", "/").TrimEnd();
            Console.WriteLine(path);
            XNamespace ns = "http://www.w3.org/2001/XMLSchema-instance";
            var d = XDocument.Load(path).Descendants();
            var player = d.First(x => x.Name == "player");
            var locations = d.First(x => x.Name == "locations").Descendants();
            var farm = locations.Single(x => x.Attribute(ns + "type")?.Value == "Farm");
            var buildings = farm.Element("buildings").Descendants();
            var cabins = buildings.Where(x => x.Element("indoors").Attribute(ns + "type")?.Value == "Cabin");
            var blankCabin = cabins.Single(x => x.Element("indoors")
                                            .Element("farmhand")
                                            .Element("name").Value == null
                                        );
            var blankFarmHand = blankCabin.Element("indoors").Element("farmhand");
            
            Console.WriteLine(blankCabin.ToString());
        }
    }
}
