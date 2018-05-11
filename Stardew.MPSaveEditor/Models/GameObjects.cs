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
        public List<FarmObject> OtherBuildings { get; set; }
        public IEnumerable<FarmObject> Bushes { get; set; }
        public IEnumerable<XElement> Terrain { get; set; }
        public IEnumerable<FarmObject> Trees { get; set; }
        public IEnumerable<FarmObject> FruitTrees {get; set;}
        public IEnumerable<FarmObject> Grass { get; set; }
        public IEnumerable<FarmObject> HoeDirt { get; set; }
        public IEnumerable<FarmObject> LogsAndRocks { get; set; }
        public IEnumerable<IEnumerable<FarmObject>> ObjectsList { get; set;}
        public SaveGame.FarmType FarmType {get;set;}

        public XElement Farm {get;set;}
        public GameObjects(SaveGame game) {
            FarmType = game.Type;
            Farm = game.Farm;
            SetObjects();
            SetOtherBuildings();



            ObjectsList = new List<IEnumerable<FarmObject>> {
                Objects,
                Buildings,
                Bushes,
                Trees,
                FruitTrees,
                Grass,
                HoeDirt,
                LogsAndRocks,
                OtherBuildings
            };
        }   

        public void SetObjects() {
            Objects = Farm.Element("objects").Elements().Select(x => new FarmObject(x, GameObjectTypes.Object));
            Buildings = Farm.Element("buildings").Elements().Select(x => new FarmObject(x, GameObjectTypes.Building));
            Bushes = Farm.Element("largeTerrainFeatures").Elements().Select(x => new FarmObject(x, GameObjectTypes.Bush));
            Terrain = Farm.Element("terrainFeatures").Elements();
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
            LogsAndRocks = Farm.Element("resourceClumps").Elements().Select(x => new FarmObject(x, GameObjectTypes.LogsAndRocks));

        }


        public void SetOtherBuildings() {
            OtherBuildings = new List<FarmObject> {
                new FarmObject(GameObjectTypes.OtherBuildings, 59, 11, 10, 6), // Farmhouse
                new FarmObject(GameObjectTypes.OtherBuildings, 7, 8, 3 , 2), // Grandpas
                new FarmObject(GameObjectTypes.OtherBuildings, 72, 14, 2, 1), // Selling Bin
                new FarmObject(GameObjectTypes.OtherBuildings, 69, 16, 1, 1), // Mailbox
                new FarmObject(GameObjectTypes.OtherBuildings, 33, 8, 3, 1), // Cave Entrance
                new FarmObject(GameObjectTypes.OtherBuildings, 25, 10, 7, 6), // Greenhouse
                new FarmObject(GameObjectTypes.OtherBuildings, 27, 16, 3, 2), // Greenhouse entrance mat
                new FarmObject(GameObjectTypes.OtherBuildings, 50, 0, 29, 10), // rectangle including dog bowl and unusable upper right area
                new FarmObject(GameObjectTypes.OtherBuildings, 40, 0, 2, 9), // Farm northern entrance
                new FarmObject(GameObjectTypes.OtherBuildings, 76, 15, 4, 3), // Farm eastern entrance
                new FarmObject(GameObjectTypes.OtherBuildings, 40, 60, 2, 5) // Farm southern entrance
            };

            if (FarmType == SaveGame.FarmType.Regular) {
                OtherBuildings.Add(new FarmObject(GameObjectTypes.OtherBuildings, 33, 49, 8, 10));  // Large Pond           
                OtherBuildings.Add(new FarmObject(GameObjectTypes.OtherBuildings, 70, 28, 6, 6));   // Tiny Pond
            }
            if (FarmType == SaveGame.FarmType.Forest) {
                OtherBuildings.Add(new FarmObject(GameObjectTypes.OtherBuildings, 38, 31, 17, 14)); // Large Pond
                OtherBuildings.Add(new FarmObject(GameObjectTypes.OtherBuildings, 42, 52, 10, 6)); // South Pond
                OtherBuildings.Add(new FarmObject(GameObjectTypes.OtherBuildings, 60, 42, 13, 10)); // South East Pond
                OtherBuildings.Add(new FarmObject(GameObjectTypes.OtherBuildings, 70, 28, 6, 6)); // East Pond           
            }
            if (FarmType == SaveGame.FarmType.Wilderness) {
                OtherBuildings.Add(new FarmObject(GameObjectTypes.OtherBuildings, 39, 24, 19, 16)); // Large Pond 
            }
        }
        static public IEnumerable<Tuple<int, int>> GenerateOptimalLocations(){
            List<Tuple<int, int>> optimalLocations = new List<Tuple<int, int>>();
            // open space between farm and greenhouse
            int xMin = 33; // left side of open space
            int yMin = 10; // top of open space
            int xMax = 57; // right side of open space
            int yMax = 17; // bottom of open space
            for (int i = xMin; i <= xMax; i++){
                for (int j = yMin; j <= yMax; j++){
                    optimalLocations.Add(new Tuple<int, int>(i, j));
                }
            }
            return optimalLocations;
        }


        public IEnumerable<Tuple<int, int>> GenerateEntireMapLocations(){
           List<Tuple<int, int>> allLocations = new List<Tuple<int, int>>();
            List<int> xMin = new List<int>();
            List<int> yMin = new List<int>();
            List<int> yMax = new List<int>();
            List<int> xMax = new List<int>();


            var exclusions = new List<Func<int,int,bool>> ();
            if (FarmType == SaveGame.FarmType.Regular) {
                // main area
                xMin.Add(7);
                yMin.Add(16); 
                xMax.Add(67); 
                yMax.Add(54);
            }
            else if (FarmType == SaveGame.FarmType.Forest) {
                // main area
                xMin.Add(23);
                yMin.Add(18); 
                xMax.Add(71); 
                yMax.Add(53);
            }
            else if (FarmType == SaveGame.FarmType.Wilderness) {
                // main area
                xMin.Add(29);
                yMin.Add(18); 
                xMax.Add(66); 
                yMax.Add(53);               
            }
            // Tough to find viable spots
            else if (FarmType == SaveGame.FarmType.River) {
                
            }
            // Tough to find viable spots
            else if (FarmType == SaveGame.FarmType.HillTop) {

            }         
            for(int c = 0; c < xMin.Count(); c++)
            for (int i = xMin[c]; i <= xMax[c]; i++){
                for (int j = yMin[c]; j <= yMax[c]; j++){
                    allLocations.Add(new Tuple<int, int>(i, j));                   
                }
            }
            return allLocations;
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
            originalCabin.TileX = location.Item1;
            originalCabin.TileY = location.Item2;   
            return true;
        }

        public void ClearLocationForCabin(int x, int y, FarmObject c) {
            var cabin = new FarmObject(GameObjectTypes.Building, x, y, c.Width, c.Height);
            var overlapLists = ObjectsList.SelectMany(t => FindAllOverlaps(t, cabin, true));
            foreach(var obj in overlapLists) {               
                if (obj.Item3.Element.Parent != null)                   
                    obj.Item3.Element.Remove();             
            }           
        }

        // Returns X, Y, NeedsRemoval
        public Tuple<int,int,bool> FindValidLocation(FarmObject cabin) {
            // First, check if the inital location has any overlaps
            var initialOverlaps = ObjectsList.Select(x => FindAllOverlaps(x, cabin));
            if (!initialOverlaps.Any()) {
                // And if there aren't any, return the cabin's initial X,Y
                return new Tuple<int,int, bool>(cabin.TileX, cabin.TileY, false);
            }
            var optimalLocation = FindPossibleLocations(GenerateOptimalLocations(), cabin);
            if (optimalLocation != null) {
                return optimalLocation;
            }
            // If no optimal location works, start looking everywhere
            
            var location = FindPossibleLocations(GenerateEntireMapLocations(), cabin);
            if (location != null) {
                return optimalLocation;
            }
            // If nowhere works, return nothing, no location is valid, inform the player.
            return null;
        }

        public Tuple<int,int,bool> FindPossibleLocations(IEnumerable<Tuple<int,int>> locations, FarmObject cabin) {
            Tuple<int,int,bool> optimalCabinLocation = null;
            var listOfPotentialCabins = locations.Select(x => new FarmObject(GameObjectTypes.Building, x.Item1, x.Item2, 5, 3));                                                                                // Cabin X , Y, List of Overlaps with X, Y, FarmObject @ overlap
            var overlapsByCabinLocations = listOfPotentialCabins.Select(c => new Tuple<int, int, IEnumerable<Tuple<int, int, FarmObject>>>(c.TileX, c.TileY, ObjectsList.SelectMany(x => FindAllOverlaps(x, c))));
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
                foreach (var obj in lst) {
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
                if ((onlyRemovable && obj.CanBeRemoved) || !onlyRemovable) {
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

    public enum GameObjectTypes {
        Tree,
        FruitTree,
        Building,
        OtherBuildings,
        Bush,
        Grass,
        HoeDirt,
        Stone, // Inside "Object"
        
        Twig, // Inside "Object"
        Object,
        LogsAndRocks,
        Meteorite,
    }
}