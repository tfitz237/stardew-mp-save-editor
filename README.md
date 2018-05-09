# Stardew Valley Even More Multiplayer
For those of you that need more than four players in a single game of Stardew Valley.

## Getting Started
Running this application will be as simple as double clicking the EXE or running from command line (to supply arguments)

### Prerequisites
To run from source, you must have [Dotnet Core CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore) installed

### Usage
Run the executable from command line.

Note: A copy of the original file and the modified file will be placed in the saves subdirectory.

### Run from command-line:

#### List of commands available:
Shows a list of the commands avaiable
```
Program.exe
```
#### AddPlayers
Will add <Number of New Player Slots> to a \<Save File>
  
If arguments are not passed, user will be prompted for them
```
Program.exe  AddPlayers -p=\<Number of New Player Slots> -s=\<Path to save file>
```

#### ChangeHost
Will change the host in a \<Save File> with one of the farmhands

Farmhand selection will occur
```
Program.exe  ChangeHost -s=\<Path to save file>
```
Any missing arguments will result in prompting for information

#### Finding the saved game
If you do not supply Program with a save file path, you will have to select a save file from your AppData/Roaming/Stardew Valley/Saves folder.




