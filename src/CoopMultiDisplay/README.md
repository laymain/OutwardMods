## Split screen with multiple displays support

This mod allows you choose the split screen type in:

* **Horizontal** (default)
* **Vertical**
* **MultiDisplay**

### Installation

[Manually](https://outward.fandom.com/wiki/Installing_Mods#BepInEx_(Manual_installation))
or using [Thunderstore](https://outward.fandom.com/wiki/Installing_Mods#Thunderstore).

### Features

Use these shortcuts to change the split screen type

* ``[`] + [H]`` to switch to **Horizontal**
* ``[`] + [V]`` to switch to **Vertical**
* ``[`] + [M]`` to switch to **MultiDisplay**

_To fit with multiple keyboard layouts, you can also use `[Home]` instead of ``[`]`` (backtick)._

### Known issues and limitations

* (WONTFIX) Fullscreen won't work when having `MultiDisplay` enabled, it will switch to borderless windows.
* (WONTFIX) Obviously, this mod is not compatible with the [Ultra Widescreen and Co-op UI Fixes](https://www.nexusmods.com/outward/mods/122) mod.
* (FIXABLE) By now, UI scaling (configured in display settings) is ignored when activating `Vertical` or `MultiDisplay`.
* (FIXABLE) Player 1 may lose focus (only once) just when the mod is activated, keep keyboard input bound to player 1.
* (FIXABLE) Player 2's camera offset at character creation is not correct (such as in vanilla)
* (FIXABLE) Player 1 will always stick to main display and player 2 to secondary display
* (FIXABLE) During cutscenes, player 2's camera remains enabled, so you will see things that you're not supposed to ;)
* (FIXABLE) Changing resolution while playing in `MultiDisplay` may not be taken into account and thus stretch image.

### Thanks

* Keos (Nine Dots Studio developer) for helping me understand how UI is implemented in Outward.
* VirtualLich for beta testing this mod and for his feedbacks.
