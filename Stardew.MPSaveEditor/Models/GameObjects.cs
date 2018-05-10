using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace StardewValley.MPSaveEditor.Models
{
    public class GameObjects {
        XNamespace ns = "http://www.w3.org/2001/XMLSchema-instance";
        public IEnumerable<FarmObject> Objects { get; set; }
        public IEnumerable<FarmObject> Buildings { get; set; } 
        public IEnumerable<FarmObject> Bushes { get; set; }
        public IEnumerable<XElement> Terrain { get; set; }
        public IEnumerable<FarmObject> Trees { get; set; }
        public IEnumerable<FarmObject> Grass { get; set; }
        public IEnumerable<FarmObject> HoeDirt { get; set; }
        public IEnumerable<FarmObject> LogsAndRocks { get; set; }  
        public IEnumerable<IEnumerable<FarmObject>> ObjectsList { get; set;} 
        public GameObjects(SaveGame game) {
            var farm = game.Farm;
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
            LogsAndRocks = farm.Element("resourceClumps").Elements().Select(x => new FarmObject(x, GameObjectTypes.Hardwood));
            ObjectsList = new List<IEnumerable<FarmObject>> {
                Objects,
                Buildings,
                Bushes,
                Trees,
                Grass,
                HoeDirt,
                LogsAndRocks
            };
        }   

        public IEnumerable<Tuple<int,int>> OptimalLocations = new List<Tuple<int,int>> {
            new Tuple<int,int>(0,0)
        };



        public bool MoveToValidLocation(Cabin cabin) {
            var validList = new List<bool>();
            foreach(var list in ObjectsList) {
                validList.Add(!FindAllOverlaps(list, cabin).Any());
            }
            if (validList.Any(x => !x)) {
                var location = FindValidLocation(cabin);
            }
            return validList.Any(x => !x);
        }

        // Returns X, Y, NeedsRemoval
        public Tuple<int,int,bool> FindValidLocation(Cabin cabin) {
            // First, check if the inital location has any overlaps
            var initialOverlaps = ObjectsList.Select(x => FindAllOverlaps(x, cabin));
            if (!initialOverlaps.Any()) {
                // And if there aren't any, return the cabin's initial X,Y
                return new Tuple<int,int, bool>(cabin.TileX, cabin.TileY, false);
            }
            // Then get a list of all invalid locations and their associated FarmObject 
            // This will help us determine if there are any invalid locations we can make valid 
            var invalidLocations = FindInvalidLocations();           
            // Then we start checking "optimal" locations for the Cabin
            // Example: to the left of the farmhouse can hold probably 4-8 cabins easily (at least on a fresh map)
            var optimalLocations = OptimalLocations;
            var optimalOverlaps = ObjectsList.Select(x => FindAllOverlaps(x, cabin));
            // Now that we know the optimalOverlaps, we can check if any of these overlaps FarmObject.CanBeRemoved
            // If every item can be removed from the optimalOverlaps, we pass back the X,Y,true
            // If not, keep looking.
            // If no optimal location works, start looking everywhere
            // If no where works, return nothing, no location is valid, inform the player.
            return null;
        }

        public IEnumerable<Tuple<int,int,FarmObject>> FindInvalidLocations() {
            var list = new List<Tuple<int,int,FarmObject>>();
            foreach(var lst in ObjectsList) {
                foreach (var obj in lst) {
                    foreach (var range in obj.TileXYRange) {
                        list.Add(new Tuple<int,int,FarmObject>(range.Item1, range.Item1, obj));
                    }
                }
            } 
            return list;           
        }

        public IEnumerable<Tuple<int,int, FarmObject>> FindAllOverlaps(IEnumerable<FarmObject> objects, Cabin cabin) {
            var overlapsList = new List<Tuple<int,int, FarmObject>>();
            foreach(var obj in objects) {
                var overlaps = FindOverlaps(obj.TileX, obj.TileY, obj.Width, obj.Height, cabin.TileX, cabin.TileY, cabin.Width, cabin.Height);
                if (overlaps.Any()) {
                    overlapsList.AddRange(overlaps.Select(x => new Tuple<int,int,FarmObject>(x.Item1, x.Item2, obj)));
                }
            }
            return overlapsList;
        }

        public IEnumerable<Tuple<int,int>> FindOverlaps(int x1, int y1, int width1, int height1, int x2, int y2, int width2, int height2)
        {
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
            return xyRange1.Where(x => xyRange2.Contains(x));
        }
    }


    public class FarmObject {

        XElement Element { get; set; }
        public int TileX { get; set; }
        public int TileY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public IEnumerable<Tuple<int,int>> TileXYRange { get; set; }
        public GameObjectTypes Type { get; set; }
        public bool CanBeRemoved { 
            get {
                switch(Type) {
                    case GameObjectTypes.Object:                   
                    case GameObjectTypes.HoeDirt:
                    case GameObjectTypes.Building:
                    case GameObjectTypes.Meteorite: 
                        // TODO: Do more checks. Check HoeDirt for crops, check if the object is player placed                   
                        return false;
                    case GameObjectTypes.Tree:
                    case GameObjectTypes.Grass:
                    case GameObjectTypes.Bush:
                    case GameObjectTypes.BigRock:
                    case GameObjectTypes.Hardwood:                    
                        return true;
                    
                }

                return false;
            }
        }

        public FarmObject(XElement element, GameObjectTypes type) {
            Type = type;
            Element = element;            
            SetValues();
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
                case GameObjectTypes.Building:                  
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
        Bush,
        Grass,
        HoeDirt,
        Stone, // Inside "Object"
        Twig, // Inside "Object"
        Object,
        BigRock,
        Hardwood,
        Meteorite
    }
}