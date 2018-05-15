using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace StardewValley.MPSaveEditor.Models {
     public class SaveGame {
        XNamespace ns = "http://www.w3.org/2001/XMLSchema-instance";
        private XDocument _doc {get;set;}
        private XDocument _originalDoc {get; set;}
        private IEnumerable<XElement> _saveGame {get; set;}
        private string _path {get;set;}  

        private string _fileName  {get; set;}

        private DateTime _timestamp {get; set;}     
        public SaveGame (string path) {
            LoadFile(path);
            _timestamp = DateTime.Now;

        }
        public void LoadFile(string path) {
            try {    
                _path = path;
                _doc = XDocument.Load(_path);
                _originalDoc =XDocument.Load(_path);
                _fileName = Path.GetFileName(_path);
                FileName = _fileName;
                _saveGame = _doc?.Element("SaveGame")?.Elements();         
                if (_saveGame == null || !_saveGame.Any()) {
                    throw new Exception("Game file not parsed correctly");
                }
            } catch(Exception exception) {
                throw exception;
            } 
        }

        public FarmType Type => (FarmType)Int32.Parse(_saveGame.First(x => x.Name == "whichFarm").Value); 
        public string UniqueId => _saveGame.FirstOrDefault(x => x.Name == "uniqueIDForThisGame")?.Value;
        public string FileName { get; private set;}
        
        public XElement Host => _saveGame
            .First(x => x.Name == "player");
        
        public XElement Farm => _saveGame
            .First(x => x.Name == "locations")
            .Elements()
            .Single(x =>
                x.Attribute(ns + "type")?.Value == "Farm"); 
        public XElement FarmHouse => _saveGame
            .First(x => x.Name == "locations")
            .Elements()
            .Single(x =>
                x.Attribute(ns + "type")?.Value == "FarmHouse"); 
                               
        public IEnumerable<XElement> Buildings => Farm
            .Element("buildings")
            .Elements();
        
        public IEnumerable<XElement> Cabins => Buildings
            .Where(x => 
                x.Element("indoors")?
                 .Attribute(ns + "type")?.Value == "Cabin"); 
                
        public IEnumerable<XElement> Farmhands => Cabins
            .Select(x => 
                x.Element("indoors")
                 .Element("farmhand"));
        
        public int PlayerSlots => Cabins.Count();
        public IEnumerable<string> FarmhandNames => Farmhands
        .Where(x => !x.Element("name").IsEmpty)
            .Select(x => 
                x.Element("name").Value);
        
        public Cabin CreateNewCabin(Farmhand farmhand = null) {
            var cabin = new Cabin();
            cabin.CreateCabin(farmhand);
            cabin.UpdateFarmhand(Host);
            var moved = MoveToValidLocation(cabin);
            if (moved) {
                Farm.Element("buildings").Add(cabin.Element);
                return cabin;
            }
            return null;
        }
            
        public void SaveFile(bool overwriteSaveFile = false) {
            System.IO.Directory.CreateDirectory("saves");
            var dir = $"{_fileName}_{_timestamp.ToString("MMddyyHHmm")}";
            System.IO.Directory.CreateDirectory($"saves\\{dir}");
            _originalDoc.Save($"./saves/{dir}/{_fileName}_ORIGINAL");
            _doc.Save($"./saves/{dir}/{_fileName}");
            if (overwriteSaveFile) {
                _doc.Save(_path);
            }
        }

        public bool MoveToValidLocation(Cabin cabin) {
            var objects = new GameObjects(this);
            var moved = objects.MoveToValidLocation(cabin);
            if (!moved) {
                Console.WriteLine("There were no valid locations found for the new cabin on your farm.");
            } 
            return moved;
        }

        public XElement FindCabinByFarmhand(XElement farmhand) {
            return Cabins.FirstOrDefault(x => x.Element("indoors").Element("farmhand").Element("name").Value == farmhand.Element("name").Value);
        }

        public Cabin FindEmptyCabin() {
            var cabin = Cabins.FirstOrDefault(x => x.Element("indoors").Element("farmhand").Element("name").IsEmpty);
            if (cabin == null) 
                return null;
            return new Cabin(cabin);
        }

        public XElement GetFarmhandByName(string name) {
            return Farmhands.FirstOrDefault(x => x.Element("name").Value == name);
        }


        public void SwitchHost(XElement farmhand) {
            var host = new XElement(Host);
            farmhand = new XElement(farmhand);
            var cabin = FindCabinByFarmhand(farmhand);
            farmhand.Element("eventsSeen").ReplaceAll(host.Element("eventsSeen").Nodes());
            farmhand.Element("caveChoice").Value = host.Element("caveChoice").Value;
            farmhand.Element("songsHeard").ReplaceAll(host.Element("songsHeard").Nodes());
            farmhand.Element("mailReceived").ReplaceAll(host.Element("mailReceived").Nodes());
            farmhand.Element("stats").Element("daysPlayed").Value = host.Element("stats").Element("daysPlayed").Value;
            farmhand.Element("stats").Element("DaysPlayed").Value = host.Element("stats").Element("DaysPlayed").Value;
            var farmhandUpgradeLevel = new XElement(farmhand.Element("houseUpgradeLevel"));
            var farmhandRecentBed = new XElement(farmhand.Element("mostRecentBed"));
            var farmhandRecentPosition = new XElement(farmhand.Element("Position"));
            var farmhandHome = new XElement(farmhand.Element("homeLocation"));
            var cabinNPCs = new XElement(cabin.Element("indoors").Element("characters"));
            farmhand.Element("houseUpgradeLevel").Value = host.Element("houseUpgradeLevel").Value;
            farmhand.Element("mostRecentBed").ReplaceAll(host.Element("mostRecentBed").Nodes());
            farmhand.Element("Position").ReplaceAll(host.Element("Position").Nodes());
            farmhand.Element("homeLocation").Value = host.Element("homeLocation").Value;
            FarmHouse.Element("characters").ReplaceAll(cabinNPCs.Nodes());
            host.Element("mostRecentBed").ReplaceAll(farmhandRecentPosition.Nodes());
            host.Element("Position").ReplaceAll(farmhandRecentBed.Nodes());
            host.Element("houseUpgradeLevel").Value = farmhandUpgradeLevel.Value;
            host.Element("homeLocation").Value = farmhandHome.Value;
            Host.ReplaceAll(farmhand.Nodes());
            cabin.Element("indoors").Element("farmhand").ReplaceAll(host.Nodes());    
            cabin.Element("indoors").Element("characters").ReplaceAll(FarmHouse.Element("characters").Nodes());
                   
        }

        public void RemoveCabin(XElement cabin) {
            var farmhand = cabin.Element("indoors").Element("farmhand");
            if (farmhand.Element("name").IsEmpty) {
                cabin.Remove();
            } else {
                var xdoc = new XDocument(farmhand);
                Console.WriteLine("Saving backup of farmhand...");
                System.IO.Directory.CreateDirectory("saves");
                var dir = $"{_fileName}_{_timestamp.ToString("MMddyyHHmm")}";
                System.IO.Directory.CreateDirectory($"saves\\{dir}");
                xdoc.Save($"./saves/{dir}/{_fileName}_Farmhand_{farmhand.Element("name").Value}");
            }
            
        }
    
        public enum FarmType {
            Regular = 0,
            River = 1,
            Forest = 2,
            HillTop = 3,
            Wilderness = 4
        }
    }   
}