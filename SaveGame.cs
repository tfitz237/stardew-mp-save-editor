using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.IO;
namespace stardew {
     public class SaveGame {
        XNamespace ns = "http://www.w3.org/2001/XMLSchema-instance";
        private XDocument _doc {get;set;}
        private XDocument _originalDoc {get; set;}
        private IEnumerable<XElement> _saveGame {get; set;}
        private string _path {get;set;}
        private Random random = new Random();
        private String templatePath = "template/template";
        public SaveGame (string path) {
            try {
                _path = path;
                _doc = XDocument.Load(path);
                _originalDoc =XDocument.Load(path);

                _saveGame = _doc?.Element("SaveGame")?.Elements(); 
                if (_saveGame == null || !_saveGame.Any()) {
                    throw new Exception("Game file not parsed correctly");
                }
            } catch(Exception exception) {
                throw exception;
            }
        }

        public string FileName { get => Path.GetFileName(_path);}
        
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
        
        public IEnumerable<XElement> Farmhands { 
            get => Cabins.Select(x => x.Element("indoors").Element("farmhand"));
        }

        public IEnumerable<string> FarmhandNames {
            get => Farmhands.Select(x => x.Element("name").Value);
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
            var uniqueName = $"Cabin{Guid.NewGuid()}";
            cabin.Element("indoors").Element("uniqueName").Value = uniqueName;
            GetFarmhand(cabin)
                .Element("homeLocation").Value = uniqueName;
            return cabin;
        }

        public XElement ReplaceMultiplayerId(XElement cabin) {
            byte[] buffer = new byte[8];
            random.NextBytes(buffer);
            var uniqueMultiplayerId = BitConverter.ToInt64(buffer, 0);
            GetFarmhand(cabin)
                .Element("UniqueMultiplayerID")
                .Value= uniqueMultiplayerId.ToString();
            return cabin;
        }

        public XElement GetCabinType(XElement cabin) {
            List<String> cabinTypes = new List<String>(new String[] {"Log Cabin", "Stone Cabin", "Plank Cabin"});
            var cabinType = cabinTypes[random.Next(0, 3)];
            cabin.Element("buildingType").Value = cabinType;
            return cabin;
        }

        private XElement UpdateFarmhand(XElement cabin) {
            var farmhand = GetFarmhand(cabin);
            farmhand.Element("farmName").Value = Host.Element("farmName").Value;
            farmhand.Element("money").Value = Host.Element("money").Value;
            return cabin;
        }
            
        public void SaveFile() {
            _originalDoc.Save($"./saves/{FileName}_ORIGINAL.xml");
            _doc.Save($"./saves/{FileName}_{DateTime.Now.ToString("MMddyyHHmm")}.xml");
        }

        public XElement GetFarmhand(XElement cabin) {
            return cabin.Element("indoors")
                .Element("farmhand");
        }

        public XElement FindCabinByFarmhand(XElement farmhand) {
            return Cabins.FirstOrDefault(x => x.Element("indoors").Element("farmhand").Element("name").Value == farmhand.Element("name").Value);
        }

        public XElement GetFarmhandByName(string name) {
            return Farmhands.FirstOrDefault(x => x.Element("name").Value == name);
        }


        public void SwitchHost(XElement farmhand) {
            var host = new XElement(Host);
            farmhand = new XElement(farmhand);
            var cabin = FindCabinByFarmhand(farmhand);
            Host.ReplaceAll(farmhand.Nodes());
            cabin.Element("indoors").Element("farmhand").ReplaceAll(host.Nodes());
            
        }
    }
}