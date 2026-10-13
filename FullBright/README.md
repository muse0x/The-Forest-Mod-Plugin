# the forest — fullbright

game is dark as hell. made this standalone fullbright mod so you can actually see in caves and at night. no cheats, no extra bloat, just light.

### controls
- **F7** — toggle fullbright
- **F8** — open / close menu

### install
1. install [BepInEx 5](https://github.com/BepInEx/BepInEx/releases) (64-bit) if you haven't already
2. grab `TheForestFullBright.dll` (from `/Release` folder or the Releases tab)
3. drop it into:
   ```
   The Forest/BepInEx/plugins/
   ```
4. launch the game and press **F7**. done.

### features
- flat ambient lift (brightens dark caves without blowing out daylight)
- kills distance fog (you can actually see past 20 meters)
- optional shadowless sun light slider
- 4 quick presets: subtle / mid / bright / max
- auto-saves your config

### build
```sh
dotnet build -c Release
```
output goes to `bin/Release/TheForestFullBright.dll`.
