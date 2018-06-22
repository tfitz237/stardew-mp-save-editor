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
        private XDocument _saveGameInfoDoc {get; set;}
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
                _saveGameInfoDoc = XDocument.Load(_path.Replace($"{_fileName}/{_fileName}", $"{_fileName}/SaveGameInfo"));
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
        
        public Cabin CreateNewCabin(Cabin cabin) {
            cabin.UpdateFarmhand(Host);
            UpdateHost();
            var moved = MoveToValidLocation(cabin);
            if (moved) {
                Farm.Element("buildings").Add(cabin.Element);
                return cabin;
            }
            return null;
        }

        public Cabin CreateNewCabin(int cabinType) {
                return CreateNewCabin(new Cabin(cabinType));
        }
            
        public void SaveFile(bool overwriteSaveFile = false) {
            System.IO.Directory.CreateDirectory("saves");
            var dir = $"{_fileName}_{_timestamp.ToString("MMddyyHHmm")}";
            System.IO.Directory.CreateDirectory($"saves\\{dir}");
            _originalDoc.Save($"./saves/{dir}/{_fileName}_ORIGINAL");
            _doc.Save($"./saves/{dir}/{_fileName}");
            if (overwriteSaveFile) {
                _doc.Save(_path);
                _saveGameInfoDoc.Save(_path.Replace($"{_fileName}/{_fileName}", $"{_fileName}/SaveGameInfo"));
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

        public void UpdateHost() {
            Host.Element("slotCanHost").Value = "true";
            _saveGameInfoDoc.Element("Farmer").Element("slotCanHost").Value = "true";
        }

        public XElement SwitchHost(Cabin cabin) {       
            return SwitchHost(cabin.Farmhand, cabin.Element);    
        }
        public XElement SwitchHost(XElement farmhand, XElement cabin = null) {
            var host = new XElement(Host);
            farmhand = new XElement(farmhand);
            cabin = cabin ?? FindCabinByFarmhand(farmhand);
            if (cabin != null) {
                var cabinNPCs = new XElement(cabin.Element("indoors").Element("characters"));
                var cabinId = cabin.Element("indoors").Element("uniqueName")?.Value;
                var farmhouseNPCs = new XElement(FarmHouse.Element("characters"));
                if (!cabinNPCs.IsEmpty) {
                    foreach (var cabinNpc in cabinNPCs?.Elements()) {
                        cabinNpc.Element("defaultMap").Value = "FarmHouse";
                        cabinNpc.Element("DefaultMap").Value = "FarmHouse";                   
                    }
                    FarmHouse.Element("characters").ReplaceAll(cabinNPCs.Nodes());
                }
                if (!farmhouseNPCs.IsEmpty) {
                    foreach (var cabinNpc in cabinNPCs?.Elements()) {
                        cabinNpc.Element("defaultMap").Value = cabinId;
                        cabinNpc.Element("DefaultMap").Value = cabinId;                   
                    }
                    cabin.Element("indoors").Element("characters").ReplaceAll(farmhouseNPCs.Nodes());
                }
            }   
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
            farmhand.Element("houseUpgradeLevel").Value = host.Element("houseUpgradeLevel").Value;
            farmhand.Element("mostRecentBed").ReplaceAll(host.Element("mostRecentBed").Nodes());
            farmhand.Element("Position").ReplaceAll(host.Element("Position").Nodes());
            farmhand.Element("homeLocation").Value = host.Element("homeLocation").Value;
            
            host.Element("mostRecentBed").ReplaceAll(farmhandRecentPosition.Nodes());
            host.Element("Position").ReplaceAll(farmhandRecentBed.Nodes());
            host.Element("houseUpgradeLevel").Value = farmhandUpgradeLevel.Value;
            host.Element("homeLocation").Value = farmhandHome.Value;
            Host.ReplaceAll(farmhand.Nodes());
            cabin.Element("indoors").Element("farmhand").ReplaceAll(host.Nodes());                
            return host;
                   
        }

        public void RemoveCabin(XElement cabin) {
            var farmhand = new Farmhand(cabin, true, isCabin:true);
            if (farmhand.Name == null) {
                cabin.Remove();
            } else {
                var farmhands = new Farmhands(this);
                farmhands.StoreFarmhand(farmhand);
                Console.WriteLine("Storing backup of farmhand...");
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