using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using McMaster.Extensions.CommandLineUtils;
using StardewValley.MPSaveEditor.Commands;

namespace StardewValley.MPSaveEditor
{
     [Command(ThrowOnUnexpectedArgument = false)]
    class Program
    {
        static int Main(string[] args)
        {   

            var app = new CommandLineApplication();
            app.HelpOption();
            var newPlayerOption = app.Option("-p|-players=", "Number of players to add", CommandOptionType.SingleOrNoValue);
            var saveFileOption = app.Option("-s|-save=", "Save file path", CommandOptionType.SingleOrNoValue);
            Console.WriteLine("Stardew Valley Multiplayer Save Editor");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("Programs:");
            Console.WriteLine("1. Add Players to Save File");
            Console.WriteLine("2. Change Host of Save File");
            app.OnExecute(() => {
                var program = Prompt.GetInt("Select a program:", 1);
                switch(program) {
                    case 1:
                        new AddPlayersCommand(newPlayerOption.Value(), saveFileOption.Value()).Run();
                        break;
                    case 2:
                        new ChangeHostCommand(saveFileOption.Value()).Run();
                        break;
                }

            });
            return app.Execute(args);

        }
    }
}
