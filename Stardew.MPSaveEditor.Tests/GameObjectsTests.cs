using System;
using Xunit;
using StardewValley.MPSaveEditor.Models;
using System.Linq;
using System.Collections.Generic;

namespace Tests
{
    public class GameObjectsTests
    {
        [Fact]
        public void TestFindOverlapsFullOverlap()
        {
            // Assert
            int width = 5;
            int height = 3;

            int x1 = 35;
            int y1 = 14;
            int x2 = 35;
            int y2 = 14;

            IEnumerable<Tuple<int,int>> expected = new List<Tuple<int,int>> {
                new Tuple<int,int>(35,14),
                new Tuple<int,int>(35,15),
                new Tuple<int,int>(35,16),
                new Tuple<int,int>(36,14),
                new Tuple<int,int>(36,15),
                new Tuple<int,int>(36,16),
                new Tuple<int,int>(37,14),
                new Tuple<int,int>(37,15),
                new Tuple<int,int>(37,16),
                new Tuple<int,int>(38,14),
                new Tuple<int,int>(38,15),
                new Tuple<int,int>(38,16),
                new Tuple<int,int>(39,14),
                new Tuple<int,int>(39,15),
                new Tuple<int,int>(39,16),
            };

            // Arrange
            var sut = new GameObjects(new SaveGame("../../../samples/BB_185160008"));

            // Assert
            var result = sut.FindOverlaps(x1,y1,width,height, x2,y2, width, height);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestFindOverlapsNoOverlap()
        {
            // Assert
            int width = 5;
            int height = 3;

            int x1 = 29;
            int y1 = 41;
            int x2 = 35;
            int y2 = 14;

            // Arrange
            var sut = new GameObjects(new SaveGame("../../../samples/BB_185160008"));

            // Assert
            var result = sut.FindOverlaps(x1,y1,width,height, x2,y2, width, height);
            Assert.False(result.Any());
        }
        
        [Fact]
        public void TestFindOverlapsSmallOverlap()
        {
            // Assert
            int width = 5;
            int height = 3;

            int x1 = 29;
            int y1 = 41;
            int x2 = 33;
            int y2 = 43;

            IEnumerable<Tuple<int,int>> expected = new List<Tuple<int,int>> {
                new Tuple<int,int>(33,43),
            };

            // Arrange
            var sut = new GameObjects(new SaveGame("../../../samples/BB_185160008"));

            // Assert
            var result = sut.FindOverlaps(x1,y1,width,height, x2,y2, width, height);
            Assert.Equal(expected, result);
        }
    }
}
