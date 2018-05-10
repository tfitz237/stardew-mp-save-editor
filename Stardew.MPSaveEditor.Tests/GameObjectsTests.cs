using System;
using Xunit;
using StardewValley.MPSaveEditor.Models;
namespace Tests
{
    public class GameObjectsTests
    {
        [Fact]
        public void TestCheckForOverlapFullOverlap()
        {
            // Assert
            int width = 5;
            int height = 3;

            int x1 = 35;
            int y1 = 14;
            int x2 = 35;
            int y2 = 14;

            // Arrange
            var sut = new GameObjects(new SaveGame("../../../samples/BB_185160008"));

            // Assert
            var result = sut.CheckForOverlap(x1,y1,width,height, x2,y2, width, height);
            Assert.False(result);
        }

        [Fact]
        public void TestCheckForOverlapNoOverlap()
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
            var result = sut.CheckForOverlap(x1,y1,width,height, x2,y2, width, height);
            Assert.True(result);
        }

        [Fact]
        public void TestCheckForOverlapSmallOverlap()
        {
            // Assert
            int width = 5;
            int height = 3;

            int x1 = 29;
            int y1 = 41;
            int x2 = 33;
            int y2 = 43;

            // Arrange
            var sut = new GameObjects(new SaveGame("../../../samples/BB_185160008"));

            // Assert
            var result = sut.CheckForOverlap(x1,y1,width,height, x2,y2, width, height);
            Assert.False(result);
        }
    }
}
