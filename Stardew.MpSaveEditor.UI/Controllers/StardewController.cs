using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StardewValley.MPSaveEditor.Helpers;
using StardewValley.MPSaveEditor.Models;
namespace Stardew.MPSaveEditor.UI.Controllers
{
    [Route("api/[controller]")]
    public class StardewController : Controller
    {

        [HttpGet("getSaveFiles")]
        public IActionResult GetSaveFiles()
        {
            return Ok(CommandHelpers.FindSaveFiles());
        }

        [HttpPost("addPlayers")]
        public IActionResult AddPlayers([FromBody] SaveFileSelection data) {
            var playerCount = data.NumberOfPlayers;
            SaveGame game;
            try {
                game = new SaveGame(data.SaveFile);
                while(playerCount > 0) {
                    game.CreateNewCabin();
                    playerCount--;
                } 
                return Ok(new ResultMessage {
                    Result = true,
                    Message = $"Successfully created {data.NumberOfPlayers} cabins and player slots"
                });
            } 
            catch {
                return Ok(new ResultMessage {
                    Result = false,
                    Message = "Failed to open the save file"
                });
            }

        }
        
        public class SaveFileSelection {
            public string SaveFile {get;set;}
            public int NumberOfPlayers {get;set;}
        }

        public class ResultMessage {
            public bool Result {get; set;}
            public string Message {get; set;}
        }
    }
}
