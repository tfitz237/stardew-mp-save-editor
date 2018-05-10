using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using McMaster.Extensions.CommandLineUtils;
using StardewValley.MPSaveEditor.Commands;

namespace StardewValley.MPSaveEditor
{
    class Program
    {
        static int Main(string[] args)
        {   

            Dictionary<int, string> Commands = new Dictionary<int, string> {
                { 1 , "AddPlayers" },
                { 2, "ChangeHost" }
            };

            var app = new CommandLineApplication();
            app.HelpOption();
            app.ThrowOnUnexpectedArgument = false;
            var programArgument = app.Argument("program", "Select the program you would like to run");  
            Console.WriteLine("Stardew Valley Multiplayer Save Editor");
            Console.WriteLine("--------------------------------------");
            app.OnExecute(() => {
                var program = 0;
                if (programArgument.Value == null) {           
                    foreach (var command in Commands) {
                        Console.WriteLine($"{command.Key}.   {command.Value}");
                    }   
                    program = Prompt.GetInt("Select a program:", 1);
                } else {
                    program = Commands.ContainsValue(programArgument.Value) ? Commands.First(x => x.Value == programArgument.Value).Key : -1;
                }
                switch(program) {
                    case 1:
                        CommandLineApplication.Execute<AddPlayersCommand>();
                        break;
                    case 2:
                        CommandLineApplication.Execute<ChangeHostCommand>();
                        break;
                }

            });
            return app.Execute(args);

        }
    }
}
