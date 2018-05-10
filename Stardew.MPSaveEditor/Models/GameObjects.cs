using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace StardewValley.MPSaveEditor.Models
{
    public class GameObjects {
        XNamespace ns = "http://www.w3.org/2001/XMLSchema-instance";
        public IEnumerable<XElement> Objects { get; set; }
        public IEnumerable<XElement> Buildings { get; set; } 
        public IEnumerable<XElement> Bushes { get; set; } 
        public IEnumerable<XElement> Terrain { get; set; }
        public IEnumerable<XElement> Trees { get; set; }
        public IEnumerable<XElement> Grass { get; set; }
        public IEnumerable<XElement> HoeDirt { get; set; }
        public IEnumerable<XElement> BranchesAndRocks { get; set; }   
        public IEnumerable<XElement> Cabins { get; set; }   
        public GameObjects(SaveGame game) {
            var farm = game.Farm;
            Cabins = game.Cabins;
            Objects = farm.Element("objects").Elements();
            Buildings = farm.Element("buildings").Elements();
            Bushes = farm.Element("largeTerrainFeatures").Elements();
            Terrain = farm.Element("terrainFeatures").Elements();
            Trees = Terrain.Where(x => x.Element("value")
                .Element("TerrainFeature")
                .Attribute(ns + "type")?.Value == "Tree");
            Grass = Terrain.Where(x => x.Element("value")
                .Element("TerrainFeature")
                .Attribute(ns + "type")?.Value == "Grass");
            HoeDirt = Terrain.Where(x => x.Element("value")
                .Element("TerrainFeature")
                .Attribute(ns + "type")?.Value == "HoeDirt");
            BranchesAndRocks = farm.Element("resourceClumps").Elements();
        }

        public bool CheckLocation(Cabin cabin) {
            var valid = CheckCabins(cabin.TileX, cabin.TileY, cabin.Width, cabin.Height);
            return valid;
        }

        public bool CheckCabins(int x, int y, int width, int height) {
            foreach(var element in Cabins) {
                var cabin = new Cabin(element);
                if (!CheckSquare(x, y, width, height, cabin.TileX, cabin.TileY, cabin.Width, cabin.Height)) {
                    return false;
                }
            }
            return true;
        }

        public bool CheckSquare(int x1, int y1, int width1, int height1, int x2, int y2, int width2, int height2) {
            // make sure 1st x, y doesn't overlap with 2nd
    
            
            var x1Range = Enumerable.Range(x1, x1 + width1).ToList();
            var x2Range = Enumerable.Range(x2, x2 + width2).ToArray();

            var y1Range = Enumerable.Range(y1, y1 + width1).ToArray();
            var y2Range = Enumerable.Range(y2, x2 + width2).ToArray();

            var xyRange1 = x1Range.SelectMany(g => y1Range.Select(c => new Tuple<int, int>(g, c)));
            var xyRange2 = x2Range.SelectMany(g => y2Range.Select(c => new Tuple<int, int>(g, c)));
            
            return true;
        }
    }
}