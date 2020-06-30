# fastmodemultiplier
This was probably the simplest first thing I figured I could make with BepInEx. All it does it create a config file that allows you to increase the rate of time.

# Installation
Extract `fastmodemultiplier.dll` to your `Cultist Simulator\BepInEx\plugins` folder.

# Usage
Open `Cultist Simulator\BepInEx\config\justastranger.fastmodemultiplier.cfg` in your favorite text editor.
Find the line that says `Multiplier = 1` and change that 1 to whatever value you want that's greater than or equal to 1. 

# Building
Clone the repository and create a `Libs` folder inside the solution folder (alongside `fastmodemultiplier.sln`) and acquire and place the following files into that folder:
 - Found in `Cultist Simulator\cultistsimulator_Data\Managed`
   - UnityEngine.dll
   - UnityEngine.CoreModule.dll
   - Assembly-CSharp.dll
 - Distributed by BepInEx
   - BepInEx.dll
   - BepInEx.Harmony.dll
   - 0Harmony.dll

Barring any unforseen trickery, everything should build fine. This really is the simplest possible mod I could think of.