using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace StardewValley.MPSaveEditor.Models {
    public class Cabin {
        private Random random = new Random();
        private String templatePath = AppContext.BaseDirectory + "\\template\\template";
        public XElement Element { get; set; }
        public Cabin(XElement cabin = null) {
            if (cabin == null) {
                CreateNewCabin();
            } else {
                Element = cabin;
            }
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
        public void CreateNewCabin() {
            XDocument template = XDocument.Load(templatePath);
            var blankCabin = template.Element("template")
                .Element("Building");
            Element = new XElement(blankCabin);
            ReplaceCabinName();
            ReplaceMultiplayerId();
            ReplaceCabinType();
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

        public void ReplaceCabinType() {
            List<String> cabinTypes = new List<String>(new String[] {"Log Cabin", "Stone Cabin", "Plank Cabin"});
            var cabinType = cabinTypes[random.Next(0, 3)];
            Element.Element("buildingType").Value = cabinType;
        }

        public void UpdateFarmhand(XElement host) {
            Farmhand.Element("farmName").Value = host.Element("farmName").Value;
            Farmhand.Element("money").Value = host.Element("money").Value;
        }

        public XElement Farmhand => Element.Element("indoors").Element("farmhand");
        
    }
}