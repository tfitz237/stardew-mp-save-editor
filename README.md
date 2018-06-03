# Stardew Valley Farmhand Management
This utility is a stand alone save editor which allows users to add and manage more than four players in a multiplayer save file from Stardew Valley. No mods such as SMAPI are required for this functionality, and up to 8 players can be added to a vanilla multiplayer game.

[![Build Status](https://travis-ci.org/tfitz237/stardew-mp-save-editor.svg?branch=master)](https://travis-ci.org/tfitz237/stardew-mp-save-editor)

## Getting Started
Run this application from a terminal, all necessary runtimes and dependencies will be included in the release.

The utility will search for saved files in the default save location for Stardew Valley saves:  "%USERNAME%/AppData/Roaming/Stardew Valley/Saves" on Windows and /home/$USERNAME/.config/StardewValley/Saves on Unix systems.

### Prerequisites
To run from source, you must have [Dotnet Core CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore) installed.

### Usage
Run the executable from command line.

Note: A copy of the original file and the modified file will be placed in the /saves subdirectory every time an action is taken.

### Capabilites

#### Show Current Farmhands 
Lists the host and current farmhands in the selected save file.

#### Add Farmhand
Add an empty farmhand slot and cabin to the selected save file or add an existing farmhand from storage to a new slot.

#### Remove Farmhand
Removes a farmhand from the selected save file with the option to remove the player slot entirely. The user will also have the option to save the farmhand into the utility's storage to be used later.

#### Swap Host
Swaps the host slot with one of the farmhands in the save file.

#### Swap Farmhand in Storage
Swaps a farmhand from the save file with a farmhand in storage.

#### Finding the Saved Games

