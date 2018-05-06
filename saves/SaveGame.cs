using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace stardew {
     public class SaveGame {
        XNamespace ns = "http://www.w3.org/2001/XMLSchema-instance";
        private XDocument _doc {get;set;}
        private IEnumerable<XElement> _saveGame {get; set;}
        private Random random = new Random();
        private String templatePath = "samples\\template";
        public SaveGame (string path) {
            try {
                _doc = XDocument.Load(path);
                _saveGame = _doc?.Element("SaveGame")?.Elements(); 
                if (_saveGame == null || !_saveGame.Any()) {
                    throw new Exception("Game file not parsed correctly");
                }
            } catch(Exception exception) {
                throw exception;
            }
        }
        
        public XElement Host { get => _saveGame
            .First(x => x.Name == "player");
        }

        public XElement Farm { get => _saveGame
            .First(x => x.Name == "locations")
            .Elements()
            .Single(x =>
                x.Attribute(ns + "type")?.Value == "Farm"); 
        }
        
        public IEnumerable<XElement> Buildings { get => Farm
            .Element("buildings")
            .Elements();
        }

        public IEnumerable<XElement> Cabins { get => Buildings
            .Where(x => 
                x.Element("indoors")
                .Attribute(ns + "type")?.Value == "Cabin"); 
        }
        
        public void AddCabin(XElement cabin) {
            Farm.Element("buildings").Add(cabin);
        }

        public XElement CreateNewCabin() {
            XDocument template = XDocument.Load(templatePath);
            var blankCabin = template.Element("template")
                .Element("Building");
            return new XElement(blankCabin);
        }

        public XElement ReplaceCabinName(XElement cabin) {
            var uniqueName = String.Format("Cabin{0}", Guid.NewGuid().ToString());
            Console.WriteLine(uniqueName);
            cabin.Element("indoors").Element("uniqueName").Value = uniqueName;
            getFarmhand(cabin)
                .Element("homeLocation").Value = uniqueName;
            return cabin;
        }

        public XElement ReplaceMultiplayerId(XElement cabin) {
            byte[] buffer = new byte[8];
            random.NextBytes(buffer);
            var uniqueMultiplayerId = BitConverter.ToInt64(buffer, 0);
            getFarmhand(cabin)
                .Element("UniqueMultiplayerID")
                .Value= uniqueMultiplayerId.ToString();
            return cabin;
        }

        private XElement UpdateFarmhand(XElement cabin) {
            var farmhand = getFarmhand(cabin);
            farmhand.Element("farmName").Value = Host.Element("farmName").Value;
            farmhand.Element("money").Value = Host.Element("money").Value;
            return cabin;
        }
            
        public void SaveFile() {
            _doc.Save("./saves/JustSaved.xml");
        }

        private XElement getFarmhand(XElement cabin) {
            return cabin.Element("indoors")
                .Element("farmhand");
        }
    }
}