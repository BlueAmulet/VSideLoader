# VSideLoader

A very basic sideloader for Valheim.  
Supports texture dumping and loading at the moment.

## Installation  
Get the latest package from [Releases](https://github.com/BlueAmulet/VSideLoader/releases) and unpack it in the same folder as valheim.exe  
If you don't already have BepInEx, grab it from [Valheim Thunderstore](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/) and place the **contents** of 'BepInExPack_Valheim' in the same folder as valheim.exe  
If you don't know where valheim.exe is, right click Valheim in your Steam library, go to Manage, and Browse Local Files

https://github.com/BlueAmulet/VSideLoader/releases

## Usage  
VSideLoader will create the following directories:  
BepInEx\plugins\Textures\Dump  
BepInEx\plugins\Textures\Load  
It will also create a config file at BepInEx\config\VSideLoader.cfg

VSideLoader adds a new command called "reloadtextures", to quickly reload textures in game

Dump: (default false)  
If enabled, dumps all textures to the `BepInEx\plugins\Textures\Dump` folder  
Load: (default true)  
If enabled, loads all textures in the `BepInEx\plugins\Textures\Load` folder  
DetectCollision: (default false)  
Enables texture name collision detection, mainly for debugging purposes.  
Some textures will report as having duplicate names, despite having the same image contents.  
SmokeFix: (default true)  
The texture used for smoke and fog is named 'dirt', this renames it to 'smoke' to avoid texture collisions.  
TextureFilter: (default Trilinear)  
Sets the texture filter of loaded textures, if you want to retain the pixelated look of the game, set this to Point.  
UseTextureName: (default true)  
If enabled, use the internal texture name for dumping and loading. Otherwise uses material and property name.  
BlackList:  
A set of texture names not to dump.  
IgnoreName:  
A set of internal texture names to ignore, falling back on material and property name.  
BetterLighting: (default true)  
Replaces the toon shader with the default deferred shader, removing lighting banding.  
Set to false to restore the game's retro look.

## Building  
You will need [Visual Studio 2019](https://visualstudio.microsoft.com/vs/community/) and .NET Framework 4.7.2  
VSideLoader.csproj assumes Valheim is installed in `C:\Program Files (x86)\Steam\steamapps\common\Valheim`  
If it isn't, open VSideLoader.csproj in a text editor and replace it with the correct location