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

        public FarmType Type => (FarmType)Int32.Parse(_saveGame.First(x => x.Name == "whichFarm").Value); 

        public string FileName  => Path.GetFileName(_path);
        
        public XElement Host => _saveGame
            .First(x => x.Name == "player");
        
        public XElement Farm => _saveGame
            .First(x => x.Name == "locations")
            .Elements()
            .Single(x =>
                x.Attribute(ns + "type")?.Value == "Farm"); 
                
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
        
        public IEnumerable<string> FarmhandNames => Farmhands
        .Where(x => !x.Element("name").IsEmpty)
            .Select(x => 
                x.Element("name").Value);
        
        public void CreateNewCabin() {
            var cabin = new Cabin();
            cabin.CreateNewCabin();
            cabin.UpdateFarmhand(Host);
            var moved = MoveToValidLocation(cabin);
            if (moved) {
                Farm.Element("buildings").Add(cabin.Element);
            }
        }
            
        public void SaveFile() {
            System.IO.Directory.CreateDirectory("saves");
            var dir = $"{FileName}_{DateTime.Now.ToString("MMddyyHHmm")}";
            System.IO.Directory.CreateDirectory($"saves\\{dir}");
            _originalDoc.Save($"./saves/{dir}/{FileName}_ORIGINAL");
            _doc.Save($"./saves/{dir}/{FileName}");
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

        public XElement GetFarmhandByName(string name) {
            return Farmhands.FirstOrDefault(x => x.Element("name").Value == name);
        }

        public void SwitchHost(XElement farmhand) {
            var host = new XElement(Host);
            farmhand = new XElement(farmhand);
            farmhand.Element("eventsSeen").ReplaceAll(host.Element("eventsSeen").Nodes());
            farmhand.Element("caveChoice").Value = host.Element("caveChoice").Value;
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
            var cabin = FindCabinByFarmhand(farmhand);
            Host.ReplaceAll(farmhand.Nodes());
            cabin.Element("indoors").Element("farmhand").ReplaceAll(host.Nodes());           
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