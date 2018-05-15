

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace StardewValley.MPSaveEditor.Models {

    public class Farmhands {
        private string templatePath = AppContext.BaseDirectory + "\\template\\template";
        private SaveGame _game {get;set;}
        private string _fileName {get;set;}
        private XDocument _doc { get; set;}
        private IEnumerable<XElement> _farmhands { get; set; }
        public Farmhands(SaveGame game) 
        {
            _game = game;
            FindFarmhands(game);
        }




        public void FindFarmhands(SaveGame game) {
            var farmhandNames = game.FarmhandNames.ToList();
            farmhandNames.Add(game.Host.Element("name").Value);
            Swap(farmhandNames, 0, farmhandNames.Count() - 1);
            var fileName = "";
            var hasFarmhands = false;
            foreach(var farmhand in farmhandNames) {
                fileName = $"{farmhand}_{game.UniqueId}_farmhands";
                hasFarmhands = LoadFile(fileName);
                if (hasFarmhands)                                         
                    break;
            }
            if (!hasFarmhands) {
                    _doc = new XDocument(new XElement("Farmhands"));                
                    _farmhands = _doc.Element("Farmhands")?.Elements();   
                    _fileName = $"{game.Host.Element("name").Value}_{game.UniqueId}_farmhands";;
            } else {
                _fileName = fileName;
            }
            
            PopulateFarmhands();
        }

        public void PopulateFarmhands() {
            FarmhandsInGame = _game.Cabins.Select(x => new Farmhand(x, isCabin: true));
            FarmhandsInStorage = _farmhands.Select(x => new Farmhand(x));

        }

        public bool AddFarmhand(Farmhand farmhand = null) {
            if (farmhand == null || farmhand.InStorage) {
                var cabin = _game.FindEmptyCabin();
                if (cabin == null) {
                    cabin = _game.CreateNewCabin(farmhand);
                } 
                else if (farmhand != null) {
                    cabin.SwitchFarmhand(new XElement(farmhand.Element));
                    farmhand.Cabin = cabin;
                    farmhand.Element.Remove();
                }               
                return true;
            }
            return false;
        }

        public void StoreFarmhand(Farmhand farmhand) {
            var element = _farmhands.FirstOrDefault(x => x.Element("name")?.Value == farmhand.Name);
            if (element == null) {
                _doc.Element("Farmhands").Add(new XElement(farmhand.Element));
            } else {
                element.ReplaceAll(farmhand.Element.Nodes());
            }
        }

        public void RemoveFarmhandFromStorage(Farmhand farmhand) {
            if (farmhand.InStorage) {
                farmhand.Element.Remove();
            }
        }
        public void RemoveFarmhandFromCabin(Farmhand farmhand, bool storeFarmhand = true) {
            if (farmhand.InGame) {
                if (storeFarmhand) {
                    StoreFarmhand(farmhand);
                }
                var newFarmhand = CreateBlankFarmhandElement();
                farmhand.Cabin.SwitchFarmhand(newFarmhand);
            }
        }

        public XElement CreateBlankFarmhandElement() {
                XDocument template = XDocument.Load(templatePath);
                var blankFarmhand = template.Element("template")
                .Element("Building").Element("indoors").Element("farmhand");
                return new XElement(blankFarmhand);
        }

        public void SwitchFarmhands(Farmhand inGame, Farmhand inStorage) {
            inGame.Cabin.SwitchFarmhand(inStorage.Element);
        }


        public IEnumerable<Farmhand> AllFarmhands => FarmhandsInStorage.Concat(FarmhandsInGame);
        public IEnumerable<Farmhand> FarmhandsInStorage {get;set;}

        public IEnumerable<Farmhand> FarmhandsInGame {get;set;}

        public bool LoadFile(string fileName) {
            try {
                _doc = XDocument.Load($"./farmhands/{fileName}");
                _farmhands = _doc.Element("Farmhands")?.Elements();               
                if (_farmhands == null || !_farmhands.Any()) {
                    return false;
                }
                return true;
            } catch {
                return false;
            }
        }

        public void SaveFile() {
            System.IO.Directory.CreateDirectory("farmhands");
            _doc.Save($"./farmhands/{_fileName}");
        }
        public static List<T> Swap<T>(List<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
            return list;
        }
    }

    public class Farmhand {
        public XElement Element {get;set;}

        public string Name => Element.Element("name").IsEmpty ? null : Element.Element("name").Value;
        public Cabin Cabin {get;set;}

        public bool InGame => Cabin != null;

        public bool InStorage => Cabin == null;

        public Farmhand(XElement element, bool isCabin = false) {
            if (isCabin) {
                Cabin = new Cabin(element);
                Element = Cabin.Farmhand;
            } else {
                Element = element;
            }

        }
        public Farmhand(Cabin cabin) {
            Cabin = cabin;
            Element = Cabin.Farmhand;
        }


    }

}