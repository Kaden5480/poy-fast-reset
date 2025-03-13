# Fast Reset Guide
# Contents
- [Basic Usage](#basic-usage)
- [Advanced Usage](#advanced-usage)
    - [The UI](#the-ui)
    - [State Management](#state-management)
    - [Profiles](#profiles)
    - [Catches](#catches)

# Basic Usage
To get started with Fast Reset, all you need to know are the basic keybinds and functionality.

The default keybinds are:
- `F8` Save state
- `F4` Restore state

Saving a state will save your player's position and camera rotation. The "scene state" will also be stored.
This tracks information such as the rotation of the mill on Old Mill and the rotations/positions of wheels/beams on maps
like Tutor's Tower and Seaside Tribune. There are more types of "things" which Fast Reset tracks, and you
can learn more about this in [State Management](#state-management).

Restoring a state will restore the state you saved with `F8`.

## Audio Cues
Fast Reset has 3 audio cues currently.

- A successful save will result in a "ping" sound.
- A successful restore will result in a "click" sound.
- Any failure will result in a "shake" sound.

A failure sound could mean that there is no state to restore from,
you have selected invalid options in the UI, you caught one of the [exceptions](#catches)
or (hopefully in rare cases) some kind of unexpected error occurred.

<br>
This is all you should really need to know to take advantage of this mod.
If you want to learn more of the advanced features, continue to the next section.

# Advanced Usage

## The UI
Fast Reset's UI can be accessed by default by pressing `Left Shift`+`F8`.

You can also close the UI with the same keybind, or by pressing the "Close" button.

The UI displays information about the currently available states
along with providing configuration options for controlling
how Fast Reset will behave.

### Player state
This section of the UI displays which player states are currently loaded.
If "Normal" is available, your normal state can be restored from.
If "Routing flag mode" is available, your temporary state can be restored from.

### Scene state
This section of the UI displays which scene states are currently loaded.
If "Normal" is available, your normal state can be restored from.
If "Routing flag mode" is available, your temporary state can be restored from.

### When saving
When using the "save" keybind you have the option of
saving the player and scene states separately.

- `Save player state` Whether to save the player state.
- `Save scene state` Whether to save the scene state.

### When restoring
When using the "restore" keybind, you have the option
of restoring the player and scene states separately.

- `Restore player state` Whether the player state will be restored.
- `Restore scene state` Whether the scene state will be restored.
- `Use initial player state` The initial state of the player
  will be restored instead of your normal/temporary state.
- `Use initial scene state` The initial state of the scene
  will be restored instead of your normal/temporary state.
- `Reset wind` On wuthering crest, reset the wind to use
  the maximum possible delay before starting up again.

Do read [Catches](#catches) as there are exceptions for restoring state.

## State Management
### Tracked objects
Fast reset tracks a variety of objects for the scene state.

These are currently:
- The state of the mill on Old Mill
- The multi-hit brittle ice on ST
- Crumbling holds
- Wheels/beams on maps such as Tutor's Tower and Seaside Tribune.

### States
In Fast Reset, there are many ways of managing player and scene states.
But first, the different states need to be explained.

#### The initial state
An "initial state" is always saved upon entering a scene.
This holds information on the state the scene was in when it first loaded
and also the player's spawn point. This is different between scenes, of course,
and will update automatically. This state isn't saved to a file.

#### The normal (saved) state
A "saved state" is saved by using the "save" keybind.
This state is saved to a file and will always be accessible
when you reload the scene.

#### The temporary state (routing flag mode)
A "temporary state" can also be saved using the "save" keybind. This state
is only accessible in routing flag mode and will be cleared once the scene
changes/is restarted.

If you have a saved state and not a temporary state, the saved
state will be used when restoring.

#### The separation of player and scene state
Player and scene states can be saved/restored separately.
This provides lots of flexibility and depth.

For example:
- You could save a player state on Old Mill, but are unhappy with the
  mill's rotation, so you disable saving player state, and enable saving scene
  state, just to update the state of the mill.
- You are happy with a scene state, such as the crumbling holds being removed
  on Old Langr, but you don't like your reset point. So you can disable saving
  scene state while keeping saving player state enabled.
- You want to restore the player's spawn point upon loading into a scene,
  but want to load your saved/temporary scene state, so you choose to use
  the initial player state, but not the initial scene state.

Do read [Catches](#catches) as there are exceptions for restoring state.

## Profiles
Profiles contain their own saved states. By default, the "Default" profile
is used.

To give an idea what a "profile" means in Fast Reset, here is an example
of saved data for some custom profiles:
```
Pocketwatch%/
├── Peak_19_OldLangr.dat
├── Peak_6_OldGroveSkelf.dat
└── Peak_3_OldMill.dat

Pocketwatch% Pipe Only/
├── Peak_LighthouseNew.dat
└── Tind_1_WalkersPillar.dat
```
As you can see, each profile has its own saved states for each scene.
Such as Pocketwatch% having saved states for Old Langr and Old Grove's Skelf,
while Pocketwatch% Pipe Only has saved data for The Lighthouse.

Whichever profile you have selected, Fast Reset will save/restore from
the saved states available under the profile.

Whenever you switch profiles, Fast Reset will use the selected profile's data instead.

By selecting "Manage Profiles" at the bottom of the UI you will
enter a different view where you can add/select/delete your profiles.

To add a profile, a name must be entered.
Then you can press "Add Profile" to add the profile.

To select a profile, press "Use" under the name of the profile.

To delete a profile, press "Delete" instead. You will be required
to confirm deletion before deleting a profile.
You cannot delete the profile that's currently in use.

> [!IMPORTANT]
> Keep in mind that deleting a profile removes ALL of its saved data.
> This cannot be undone.

## Catches
There are some catches when using Fast Reset that try to limit
over-exploiting.

These are:
- You can only save while standing at the base
  of a peak. In routing flag mode, you can save anywhere you can stand.
- Saving/restoring is disabled in yfyd/fs mode.
- Saving/restoring is disabled while resting on a wall with crampons,
  attached to a rope, or in the bivouac.
- You cannot restore a scene state without restoring a player state
  unless you are in routing flag mode. You still can restore a player
  state without restoring a scene state though.
