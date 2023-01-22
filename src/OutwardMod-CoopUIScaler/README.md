## Ultra Widescreen and Co-op UI Fixes (Updated version)

Allows switching to vertical splitscreen.
Allows to use inventory and trade screens when playing in co-op using multi monitor setup.
Moves map and loading screens to the 1st player's screen, so they're not split between monitors.

The mod should also fix ui scaling when not using splitscreen,
for example when playing on ultrawide displays - UI should not be weirdly large.

### Configuration

This mod uses [BepInEx configuration](https://docs.bepinex.dev/articles/user_guide/configuration.html),
the file can be edited using [BepInEx.ConfigurationManager](https://github.com/BepInEx/BepInEx.ConfigurationManager)
or manually at `BepInEx\config\com.laymain.outward.mods.coopuiscaler.cfg`

```ini
[General]

## Split screen type
# Setting type: SplitType
# Default value: Vertical
# Acceptable values: Vertical, Horizontal
SplitType = Vertical

## Move global UI to player 1's screen
# Setting type: Boolean
# Default value: true
MoveGlobalUIToPlayer1 = true

## UI scale factor
# Setting type: Single
# Default value: 1
# Acceptable value range: From 0,5 to 2
ScaleFactor = 1
```

### Installation

[Manually](https://outward.fandom.com/wiki/Installing_Mods#BepInEx_(Manual_installation))
or using [Thunderstore](https://outward.fandom.com/wiki/Installing_Mods#Thunderstore)

### Credits

All credits remain to SirMuffin9, I have just applied minor changes to make this mod work again.

_I'll remove this version once the author has fixed his own one._

