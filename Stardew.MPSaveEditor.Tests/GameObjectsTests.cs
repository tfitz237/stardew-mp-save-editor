using System;
using Xunit;
using StardewValley.MPSaveEditor.Models;
namespace Tests
{
    public class GameObjectsTests
    {
        [Fact]
        public void TestCheckSquareFullOverlap()
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
            var result = sut.CheckSquare(x1,y1,width,height, x2,y2, width, height);
            Assert.False(result);
        }
    }
}
