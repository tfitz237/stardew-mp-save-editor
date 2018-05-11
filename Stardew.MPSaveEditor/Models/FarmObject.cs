using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace StardewValley.MPSaveEditor.Models {

    public class FarmObject {
        public XElement Element { get; set; }
        public int TileX { get; set; }
        public int TileY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public IEnumerable<Tuple<int,int>> TileXYRange { get; set; }
        public GameObjectTypes Type { get; set; }

        private List<GameObjectTypes> CanBeRemovedList = new List<GameObjectTypes> {
            GameObjectTypes.Tree,
            GameObjectTypes.Grass,
            GameObjectTypes.Bush,
            GameObjectTypes.LogsAndRocks,           
        };
        private List<GameObjectTypes> CantBeRemovedList = new List<GameObjectTypes> {
            GameObjectTypes.Building,
            GameObjectTypes.Meteorite, 
            GameObjectTypes.FruitTree,
            GameObjectTypes.OtherBuildings           
        };

        public bool CanBeRemoved { 
            get {
                if (CanBeRemovedList.Contains(Type)) {
                    return true;
                }
                if (CantBeRemovedList.Contains(Type)) {
                    return false;
                }
                if (Type == GameObjectTypes.HoeDirt) {
                    return Element.Element("value").Element("TerrainFeature").Element("crop") == null;
                }
                if (Type == GameObjectTypes.Object) {
                    var category = Int32.Parse(Element.Element("value").Element("Object").Element("category").Value);
                    if(category == -9 || category == -8) {
                        return false;
                    }
                    if (category == 0) {
                        var name = Element.Element("value").Element("Object").Element("name").Value;
                        var cantBeRemoved = new List<string> {
                            "Wood Fence",
                            "Gate",
                            "Crab Pot",
                            "Artifact Spot"
                        };
                        if (cantBeRemoved.Contains(name)) {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public FarmObject(XElement element, GameObjectTypes type) {
            Type = type;
            Element = element;            
            SetValues();
            SetTileXYRange();
        }

        public FarmObject(Cabin cabin) {
            Type = GameObjectTypes.Building;
            Element = cabin.Element;
            TileX = cabin.TileX;
            TileY = cabin.TileY;
            Width = cabin.Width + 2;
            Height = cabin.Height + 2;
            SetTileXYRange();
        }

        public FarmObject(GameObjectTypes type, int x, int y, int w, int h) {
            Type = type;
            Element = null;
            TileX = x;
            TileY = y;
            Width = w;
            Height = h;
            SetTileXYRange();            
        }

        public void SetTileXYRange() {
            TileXYRange = Enumerable.Range(TileX, Width)
                    .SelectMany(x =>
                        Enumerable.Range(TileY, Height)
                        .Select(y => new Tuple<int, int>(x, y))
                    );
        }

        public void SetValues() {           
            switch(Type) {
                case GameObjectTypes.Object:
                case GameObjectTypes.Tree:
                case GameObjectTypes.FruitTree:
                case GameObjectTypes.HoeDirt:
                case GameObjectTypes.Grass:
                    Width = 1;
                    Height = 1;
                    TileX = Int32.Parse(Element.Element("key").Element("Vector2").Element("X").Value);
                    TileY = Int32.Parse(Element.Element("key").Element("Vector2").Element("Y").Value);
                    break;
                case GameObjectTypes.Bush:
                    Width = 1; // TODO: Figure out Bush types
                    Height = 1; // TODO: Figure out Bush types to determine W and H
                    TileX = Int32.Parse(Element.Element("tilePosition").Element("X").Value);
                    TileY = Int32.Parse(Element.Element("tilePosition").Element("Y").Value);
                    break;
                case GameObjectTypes.LogsAndRocks:
                case GameObjectTypes.Meteorite:
                    Width = Int32.Parse(Element.Element("width").Value);
                    Height = Int32.Parse(Element.Element("height").Value);
                    TileX = Int32.Parse(Element.Element("tile").Element("X").Value);
                    TileY = Int32.Parse(Element.Element("tile").Element("Y").Value);
                    break;
                case GameObjectTypes.Building:                  
                    Width = Int32.Parse(Element.Element("tilesWide").Value) + 2;
                    Height = Int32.Parse(Element.Element("tilesHigh").Value) + 2;
                    TileX = Int32.Parse(Element.Element("tileX").Value);
                    TileY = Int32.Parse(Element.Element("tileY").Value);
                    break;
            }
        }

    }
}