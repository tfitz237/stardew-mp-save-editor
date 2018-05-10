using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace StardewValley.MPSaveEditor.Models
{
    public class GameObjects {
        XNamespace ns = "http://www.w3.org/2001/XMLSchema-instance";
        public IEnumerable<FarmObject> Objects { get; set; }
        public IEnumerable<FarmObject> Stone { get; set; }
        public IEnumerable<FarmObject> Buildings { get; set; } 
        public IEnumerable<FarmObject> Bushes { get; set; }
        public IEnumerable<XElement> Terrain { get; set; }
        public IEnumerable<FarmObject> Trees { get; set; }
        public IEnumerable<FarmObject> Grass { get; set; }
        public IEnumerable<FarmObject> HoeDirt { get; set; }
        public IEnumerable<FarmObject> BranchesAndRocks { get; set; }   
        public IEnumerable<FarmObject> Cabins { get; set; }   
        public GameObjects(SaveGame game) {
            var farm = game.Farm;
            Cabins = game.Cabins.Select(x => new FarmObject(x, GameObjectTypes.Cabin));
            Objects = farm.Element("objects").Elements().Select(x => new FarmObject(x, GameObjectTypes.Object));
            Buildings = farm.Element("buildings").Elements().Select(x => new FarmObject(x, GameObjectTypes.Building));
            Bushes = farm.Element("largeTerrainFeatures").Elements().Select(x => new FarmObject(x, GameObjectTypes.Bush));
            Terrain = farm.Element("terrainFeatures").Elements();
            Trees = Terrain.Where(x => x.Element("value")
                .Element("TerrainFeature")
                .Attribute(ns + "type")?.Value == "Tree").Select(x => new FarmObject(x, GameObjectTypes.Tree));
            Grass = Terrain.Where(x => x.Element("value")
                .Element("TerrainFeature")
                .Attribute(ns + "type")?.Value == "Grass").Select(x => new FarmObject(x, GameObjectTypes.Grass));
            HoeDirt = Terrain.Where(x => x.Element("value")
                .Element("TerrainFeature")
                .Attribute(ns + "type")?.Value == "HoeDirt").Select(x => new FarmObject(x, GameObjectTypes.HoeDirt));
            BranchesAndRocks = farm.Element("resourceClumps").Elements().Select(x => new FarmObject(x, GameObjectTypes.Hardwood));
        }   

        public bool CheckLocation(Cabin cabin) {
            var validList = new List<bool>();
            validList.Add(CheckFarmObjects(Cabins, cabin));
            return validList.Any(x => !x);
        }

        public bool CheckFarmObjects(IEnumerable<FarmObject> objects, Cabin cabin) {
            foreach(var obj in objects) {
                if (!CheckForOverlap(obj.TileX, obj.TileY, obj.Width, obj.Height, cabin.TileX, cabin.TileY, cabin.Width, cabin.Height)) {
                    return false;
                }
            }
            return true;
        }

        public bool CheckForOverlap(int x1, int y1, int width1, int height1, int x2, int y2, int width2, int height2) {
            // make sure 1st x, y doesn't overlap with 2nd

            var xyRange1 = Enumerable.Range(x1, width1)
                    .SelectMany(x => 
                        Enumerable.Range(y1, height1)
                        .Select(y => new Tuple<int, int>(x, y))
                    );
            var xyRange2 = Enumerable.Range(x2, width2)
                    .SelectMany(x => 
                        Enumerable.Range(y2, height2)
                        .Select(y => new Tuple<int, int>(x, y))
                    );
            var overlap = xyRange1.Any(x => xyRange2.Contains(x));

            return !overlap;
        }
    }


    public class FarmObject {

        XElement Element { get; set; }
        public int TileX { get; set; }
        public int TileY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public GameObjectTypes Type {get;set;}

        public FarmObject(XElement element, GameObjectTypes type) {
            Type = type;
            Element = element;
            SetValues();
        }

        public void SetValues() {           
            switch(Type) {
                case GameObjectTypes.Object:
                case GameObjectTypes.Tree:
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
                case GameObjectTypes.BigRock:
                case GameObjectTypes.Hardwood:
                case GameObjectTypes.Meteorite:
                    Width = Int32.Parse(Element.Element("width").Value);
                    Height = Int32.Parse(Element.Element("height").Value);
                    TileX = Int32.Parse(Element.Element("tile").Element("X").Value);
                    TileY = Int32.Parse(Element.Element("tile").Element("Y").Value);
                    break;
                case GameObjectTypes.Cabin:                    
                    Width = Int32.Parse(Element.Element("tilesWide").Value);
                    Height = Int32.Parse(Element.Element("tilesHigh").Value);
                    TileX = Int32.Parse(Element.Element("tileX").Value);
                    TileY = Int32.Parse(Element.Element("tileY").Value);
                    break;
            }
        }

    }


    public enum GameObjectTypes {
        Tree,
        Building,
        Cabin,
        Bush,
        Grass,
        HoeDirt,
        Stone,
        Twig,
        Object,
        BigRock,
        Hardwood,
        Meteorite
    }
}