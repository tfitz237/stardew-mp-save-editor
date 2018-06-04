using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.IO;


namespace StardewValley.MPSaveEditor.Models {
    public class Cabin {
        static Dictionary<int, String> cabinTypes = new Dictionary<int,String>{[1] = "Log Cabin", [2] = "Stone Cabin", [3] = "Plank Cabin"};
        private Random random = new Random();
        private String templatePath = AppContext.BaseDirectory + "\\template\\template";
        public XElement Element { get; set; }

        public Cabin(XElement cabin) {
            Element = cabin;
        }

        public Cabin(int cabinType) {
            CreateCabin(cabinType);
        }

        public int TileX { 
            get {
                return Int32.Parse(Element.Element("tileX").Value);
            }
            set {
                Element.Element("tileX").Value = value.ToString();
            }
        }
        public int TileY { 
            get {
                return Int32.Parse(Element.Element("tileY").Value);
            }
            set {
                Element.Element("tileY").Value = value.ToString();
            }
        }
        public int Height => Int32.Parse(Element.Element("tilesHigh").Value);
        public int Width => Int32.Parse(Element.Element("tilesWide").Value);


        public void CreateCabin(int cabinType) {
            XDocument template = XDocument.Load(templatePath);
            var blankCabin = template.Element("template")
                .Element("Building");
            Element = new XElement(blankCabin);
            ReplaceCabinName();
            ReplaceMultiplayerId();
            ReplaceCabinType(cabinType);
        }

        public void ReplaceCabinName() {
            var uniqueName = $"Cabin{Guid.NewGuid()}";
            Element.Element("indoors").Element("uniqueName").Value = uniqueName;
            Farmhand.Element("homeLocation").Value = uniqueName;
        }

        public void ReplaceMultiplayerId() {
            byte[] buffer = new byte[8];
            random.NextBytes(buffer);
            var uniqueMultiplayerId = BitConverter.ToInt64(buffer, 0);
            Farmhand
                .Element("UniqueMultiplayerID")
                .Value= uniqueMultiplayerId.ToString();
        }

        public void ReplaceCabinType(int cabinType) {
            var cabinTypeName = cabinTypes.TryGetValue(cabinType, out var value) ? value : cabinTypes[random.Next(1, 4)];
            Element.Element("buildingType").Value = cabinTypeName;
        }

        public void UpdateFarmhand(XElement host) {
            Farmhand.Element("farmName").Value = host.Element("farmName").Value;
            Farmhand.Element("money").Value = host.Element("money").Value;
            Farmhand.Element("slotCanHost").Value = "true";
        }

        public void SwitchFarmhand(XElement farmhand) {
            Farmhand.ReplaceAll(farmhand.Nodes());
        }

        public void SwitchCabin(Cabin cabin) {
            var element = new XElement(cabin.Element);
            var x = TileX;
            var y = TileY;
            TileX = x;
            TileY = y;
            cabin.UpdateFarmhand(Farmhand);
            Element.ReplaceAll(cabin.Element.Nodes());
        }

        public XElement Farmhand => Element.Element("indoors").Element("farmhand");
        
    }
}