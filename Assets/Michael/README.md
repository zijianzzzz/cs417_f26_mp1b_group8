# Michael Manor Hall

Michael's MP1b room is self-contained under `Assets/Michael`.

## Open the room

- Scene: `Assets/Michael/Scenes/MichaelManorHall.unity`
- Unity: `6000.5.6f1`
- Runtime packages are already provided by the group project (URP, Input System,
  XR Interaction Toolkit, OpenXR, and TextMesh Pro).

The room integration adds this scene after Ken's room in `EditorBuildSettings.asset`.
After Ken's win message, left thumbstick click or keyboard N loads this room and
unloads Ken's room. See `Docs/RoomTransitions.md` for setup and validation.

## Room objective

Michael Manor is a five-chamber ritual escape with three ordered Key Props and Locks:

1. Press the portrait runes BAT -> WOLF -> MOON to lift the case over the Silver Fang,
   then place the Fang in the Watcher Lock. A chest opens to reveal the Midnight
   Codex and a five-passage map, and five hall Gates light up.
2. Touch a Gate rune to travel to its chamber (Return Runes bring you back). Four
   chambers are false leads with their own lever/button Reveals and red herrings; the
   Moon-marked chamber (blue-white rune) is the Moon Crypt.
3. In the Moon Crypt press WOLF -> MOON -> BLOOD (wrong orders flash red and reset)
   within the 2:30 timer. A slab slides aside; place the Moonstone in the Celestial Lock.
4. Pull the LEFT -> RIGHT levers to release the Blood Sigil, return to the hall, and
   place it in the exit pedestal. The seal retracts, the door opens, the win
   celebration plays, and `Congratulations` shows for five seconds.

The hall board tracks ritual progress, keys and locks remaining, chambers explored,
puzzles solved, clues found, Moon Shards (8 optional collectibles), and the current
objective. Hold the world-space RESTART control for two seconds to reset the room.
Inspection props (blacklight, hand mirror, magnifying glass) and angle-revealed
writing hide optional clues; 13 grabbable FALSE RELIC red herrings are rejected by
every Lock.

## Final room

Michael's room is the final escape room. After the last ritual, the exit door lifts
and the existing victory celebration plays. The player remains in Michael's room;
there is no outgoing scene portal or next-room keyboard shortcut. The existing
RESTART control can reset the room for another attempt.

## Presentation controls

These keyboard shortcuts work in Play Mode and make the room easy to demonstrate
without a headset:

- `K`: reset and play all three Lock insertions, the door, and the win sequence
- `W`: replay the win celebration
- `R`: reset the puzzle, door, and celebration
- `P`: spawn an orbiting object
- `L`: toggle the alternate hall lighting
- `B`: toggle the break-out/return presentation
- `Q`: quit Play Mode or the built player

The VR path remains the primary interaction: use the XR controllers to move through
the room, grab the Silver Fang, and place it in the socket.

## Layout

- `Manor/`: room-specific scripts, materials, models, textures, and furniture assets
- `Dependencies/`: only the legacy runtime prefabs, scripts, audio, shader, and
  skybox files referenced by this scene
- `Scenes/`: the completed Michael Manor Hall scene

Michael's compatibility scripts use the `MichaelManor` namespace so they do not
collide with teammate scripts that use common names such as `BreakOut` or
`LightSwitch`.

Poly Haven source and CC0 attribution notes are stored next to the imported assets.
