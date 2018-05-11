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
        public IEnumerable<FarmObject> OtherBuildings { get; set; }
        public IEnumerable<FarmObject> Bushes { get; set; }
        public IEnumerable<XElement> Terrain { get; set; }
        public IEnumerable<FarmObject> Trees { get; set; }
        public IEnumerable<FarmObject> FruitTrees {get; set;}
        public IEnumerable<FarmObject> Grass { get; set; }
        public IEnumerable<FarmObject> HoeDirt { get; set; }
        public IEnumerable<FarmObject> LogsAndRocks { get; set; }  
        public Dictionary<GameObjectTypes,<IEnumerable<FarmObject>> ObjectsList { get; set;} 

        public GameObjects(SaveGame game) {
            var farm = game.Farm;
            Objects = farm.Element("objects").Elements().Select(x => new FarmObject(x, GameObjectTypes.Object));
            Buildings = farm.Element("buildings").Elements().Select(x => new FarmObject(x, GameObjectTypes.Building));
            Bushes = farm.Element("largeTerrainFeatures").Elements().Select(x => new FarmObject(x, GameObjectTypes.Bush));
            Terrain = farm.Element("terrainFeatures").Elements();
            Trees = Terrain.Where(x => x.Element("value")
                .Element("TerrainFeature")
                .Attribute(ns + "type")?.Value == "Tree").Select(x => new FarmObject(x, GameObjectTypes.Tree));
            FruitTrees = Terrain.Where(x => x.Element("value")
                .Element("TerrainFeature")
                .Attribute(ns + "type")?.Value == "FruitTree").Select(x => new FarmObject(x, GameObjectTypes.FruitTree));
            Grass = Terrain.Where(x => x.Element("value")
                .Element("TerrainFeature")
                .Attribute(ns + "type")?.Value == "Grass").Select(x => new FarmObject(x, GameObjectTypes.Grass));
            HoeDirt = Terrain.Where(x => x.Element("value")
                .Element("TerrainFeature")
                .Attribute(ns + "type")?.Value == "HoeDirt").Select(x => new FarmObject(x, GameObjectTypes.HoeDirt));
            LogsAndRocks = farm.Element("resourceClumps").Elements().Select(x => new FarmObject(x, GameObjectTypes.Hardwood));

            OtherBuildings = new List<FarmObject> {
                new FarmObject(GameObjectTypes.Building, 59, 11, 10, 6), // Farmhouse
                new FarmObject(GameObjectTypes.Building, 7, 8, 3 , 2), // Grandpas
                new FarmObject(GameObjectTypes.Building, 72, 14, 2, 1), // Selling Bin
                new FarmObject(GameObjectTypes.Building, 69, 16, 1, 1), // Mailbox
                new FarmObject(GameObjectTypes.Building, 33, 8, 3, 1), // Cave Entrance
                new FarmObject(GameObjectTypes.Building, 25, 10, 7, 6), // Greenhouse
                new FarmObject(GameObjectTypes.Building, 27, 16, 3, 2), // Greenhouse entrance mat
                new FarmObject(GameObjectTypes.Building, 50, 0, 29, 10), // rectangle including dog bowl and unusable upper right area
                new FarmObject(GameObjectTypes.Building, 40, 0, 2, 9), // Farm northern entrance
                new FarmObject(GameObjectTypes.Building, 76, 15, 4, 3), // Farm eastern entrance
                new FarmObject(GameObjectTypes.Building, 40, 60, 2, 5) // Farm southern entrance
            };

            ObjectsList = new Dictionary<GameObjectTypes, IEnumerable<FarmObject>> {
                {GameObjectTypes.Objects, Objects},
                {GameObjectTypes.Buildings, Buildings},
                {GameObjectTypes.Bushes, Bushes},
                {GameObjectTypes.Trees, Trees},
                {GameObjectTypes.FruitTrees, FruitTrees},
                {GameObjectTypes.Grass, Grass},
                {GameObjectTypes.HoeDirt, HoeDirt},
                {GameObjectTypes.LogsAndRocks, LogsAndRocks},
                {GameObjectTypes.OtherBuildings, OtherBuildings}
            };
        }   

        public IEnumerable<Tuple<int,int>> OptimalLocations = GenerateOptimalLocations();

        static public IEnumerable<Tuple<int, int>> GenerateOptimalLocations(){
            IEnumerable<Tuple<int, int>> optimalLocations = new List<Tuple<int, int>>();
            // open space between farm and greenhouse
            int xMin = 33; // left side of open space
            int yMin = 10; // top of open space
            int xMax = 57; // right side of open space
            int yMax = 17; // bottom of open space
            for (int i = xMin; i <= xMax; i++){
                for (int j = yMin; j <= yMax; i++){
                    optimalLocations.Append(new Tuple<int, int>(i, j));
                }
            }
            return optimalLocations;
        }

        public bool MoveToValidLocation(Cabin originalCabin) {
            var cabin = new FarmObject(originalCabin);
            Tuple<int,int, bool> location;
            location = FindValidLocation(cabin);           
            if (location == null) {
                return false;
            }
            if (location.Item3) {
                ClearLocationForCabin(location.Item1, location.Item2, cabin);                
            }
            return true;
        }

        public void ClearLocationForCabin(int x, int y, FarmObject cabin) {
            var overlapLists = ObjectsList.SelectMany(x => x.Value.FindAllOverlaps(x, cabin, true));
            foreach(var list in overlapLists) {
                var listType = list.First().Item3.Type;
                ObjectsLists[listType].RemoveAll(list.Select(x => x.Item3.Element));
            }           
        }

        // Returns X, Y, NeedsRemoval
        public Tuple<int,int,bool> FindValidLocation(FarmObject cabin) {
            // First, check if the inital location has any overlaps
            var initialOverlaps = ObjectsList.Select(x => x.Value.FindAllOverlaps(x, cabin));
            if (!initialOverlaps.Any()) {
                // And if there aren't any, return the cabin's initial X,Y
                return new Tuple<int,int, bool>(cabin.TileX, cabin.TileY, false);
            }

            // Then we start checking "optimal" locations for the Cabin
            // Example: to the left of the farmhouse can hold probably 4-8 cabins easily (at least on a fresh map)

            var optimalLocation = FindByOptimalLocations(cabin);
            if (optimalLocation != null) {
                return optimalLocation;
            }
            // If no optimal location works, start looking everywhere
            
            // Get a list of all X,Y Coordinates that have FarmObjects
            // This will help us determine if there are any invalid locations we can make valid 
            var farmObjects = FindFarmObjects();   

            // If nowhere works, return nothing, no location is valid, inform the player.
            return null;
        }

        public Tuple<int,int,bool> FindByOptimalLocations(FarmObject cabin) {
            Tuple<int,int,bool> optimalCabinLocation = null;
            var optimalLocationCabins = OptimalLocations.Select(x => new FarmObject(GameObjectTypes.Building, x.Item1, x.Item2, 5, 3));
                                                                                    // Cabin X , Y, List of Overlaps with X, Y, FarmObject @ overlap
            var overlapsByCabinLocations = optimalLocationCabins.Select(cabin => new Tuple<int, int, IEnumerable<Tuple<int, int, FarmObject>>(cabin.TileX, cabin.TileY, ObjectsList.SelectMany(x => x.Value.FindAllOverlaps(x, cabin)));
            // Now that we know the optimalOverlaps, we can check if any of these overlaps FarmObject.CanBeRemoved
            // If every item can be removed from the optimalOverlaps, we pass back the X,Y,true
            // If not, keep looking.
            foreach (var possibleCabinLocation in overlapsByCabinLocations) {
                var possible = true;               
                foreach (var overlap in possibleCabinLocation.Item3) { 
                    var farmObject = overlap.Item3;                    
                    if (!farmObject.CanBeRemoved) {
                       possible = false;
                       break;
                    }                                       
                }
                
                if (possible) {
                    var needsRemoval = possibleCabinLocation.Item3.Any();
                    optimalCabinLocation = new Tuple<int,int,bool>(possibleCabinLocation.Item1, possibleCabinLocation.Item2, needsRemoval);
                    break;
                }
            }
            return optimalCabinLocation;
        }

        public IEnumerable<Tuple<int,int,FarmObject>> FindFarmObjects() {
            var list = new List<Tuple<int,int,FarmObject>>();
            foreach(var lst in ObjectsList) {
                foreach (var obj.Value in lst) {
                    foreach (var range in obj.TileXYRange) {
                        list.Add(new Tuple<int,int,FarmObject>(range.Item1, range.Item1, obj));
                    }
                }
            } 
            return list;           
        }

        public IEnumerable<Tuple<int,int, FarmObject>> FindAllOverlaps(IEnumerable<FarmObject> objects, FarmObject farmObject, bool onlyRemovable = false) {
            var overlapsList = new List<Tuple<int,int, FarmObject>>();
            foreach(var obj in objects) {
                if (farmObject.CanBeRemoved) {
                    var overlaps = FindOverlaps(obj, farmObject);
                    if (overlaps.Any()) {
                        overlapsList.AddRange(overlaps.Select(x => new Tuple<int,int,FarmObject>(x.Item1, x.Item2, obj)));
                    }
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

        public IEnumerable<Tuple<int,int>> FindOverlaps(FarmObject object1, FarmObject object2)
        {
            return object1.TileXYRange.Where(x => object2.TileXYRange.Contains(x));
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

        private List<GameObjectTypes> CanBeRemovedList = new List<GameObjectTypes> {
            GameObjectTypes.Tree,
            GameObjectTypes.Grass,
            GameObjectTypes.Bush,
            GameObjectTypes.BigRock,
            GameObjectTypes.Hardwood              
        };
        private List<GameObjectTypes> CantBeRemovedList = new List<GameObjectTypes> {
            GameObjectTypes.Building,
            GameObjectTypes.Meteorite, 
            GameObjectTypes.FruitTree            
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
            Width = cabin.Width;
            Height = cabin.Height;
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
            Enumerable.Range(TileX, Width)
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
        FruitTree,
        Building,
        Bush,
        Grass,
        HoeDirt,
        Stone, // Inside "Object"
        Twig, // Inside "Object"
        Object,
        BigRock,
        Hardwood,
        Meteorite,
    }
}