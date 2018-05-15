# Stardew Valley Even More Multiplayer
For those of you that need more than four players in a single game of Stardew Valley.

## Getting Started
Run this application from a terminal, all necessary runtimes and dependencies will be included in the release.

### Prerequisites
To run from source, you must have [Dotnet Core CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore) installed

### Usage
Run the executable from command line.

Note: A copy of the original file and the modified file will be placed in the saves subdirectory.

### Run from command-line:
To run Webapp on localhost:5000, `dotnet run`

To run Electron app, `dotnet electronize start`

#### List of commands available:
Shows a list of the commands avaiable
```
Program.exe
```
#### AddPlayers
Will add \<Number of New Player Slots> to a \<Save File>
Save File selection and player slot number will count
```
Program.exe  AddPlayers
```

#### ChangeHost
Will change the host in a \<Save File> with one of the farmhands

Farmhand selection and Save File selection will occur
```
Program.exe  ChangeHost
```
Any missing arguments will result in prompting for information

#### Finding the saved game
The program will search for saved files in the default save location for Stardew Valley saves:  %AppData%/Roaming/Stardew Valley/Saves folder.
