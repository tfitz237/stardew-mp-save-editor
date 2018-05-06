using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace stardew {
     public class SaveGame {
        XNamespace ns = "http://www.w3.org/2001/XMLSchema-instance";
        private XDocument _doc {get;set;}
        private IEnumerable<XElement> _saveGame {get; set;}
        private Random rand = new Random();
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
        
        public XElement Farm { get => _saveGame
            .First(x => x.Name == "locations")
            .Elements()
            .Single(x =>
                x.Attribute(ns + "type")?.Value == "Farm"); 
        }
        
        public IEnumerable<XElement> Cabins { get => Farm
            .Element("buildings")
            .Elements()
            .Where(x => 
                x.Element("indoors")
                .Attribute(ns + "type")?.Value == "Cabin"); 
            }               
        

        public void AddCabin(XElement cabin) {
            Farm.Element("buildings").Add(cabin);
        }

        public XElement CreateNewCabin() {
            var blankCabin = Cabins.First(x => x.Element("indoors")
                                        .Element("farmhand")
                                        .Element("name").IsEmpty
                                    ); 
            return new XElement(blankCabin);                
        }
        public XElement ReplaceCabinName(XElement cabin) {
            var uniqueName = String.Format("Cabin{0}", Guid.NewGuid().ToString());
            cabin.Element("indoors").Element("uniqueName").Value = uniqueName;
            cabin.Element("indoors").Element("farmhand").Element("homeLocation").Value = uniqueName;
            return cabin;
        }
        public XElement ReplaceMultiplayerId(XElement cabin) {
            byte[] buffer = new byte[8];
            rand.NextBytes(buffer);
            var uniqueMultiplayerId = BitConverter.ToInt64(buffer, 0);
            cabin.Element("indoors").Element("farmhand").Element("UniqueMultiplayerID").Value = uniqueMultiplayerId.ToString();
            return cabin;
        }

        public void SaveFile() {
            _doc.Save("./saves/JustSaved.xml");
        }
      
    }
}