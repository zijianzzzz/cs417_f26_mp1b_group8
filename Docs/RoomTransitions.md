# Ken to Michael

Michael is now the final escape room. Its existing ritual completion and victory celebration end the game; the former outgoing portal to Minh's room has been removed.

Open `Assets/Shared/Scenes/StartScreen.unity` and enter Play Mode, then select **START GAME** (or press Enter) to enter Ken's room. The enabled build order is StartScreen, Ken, Michael. You can still open `Assets/Ken/Scenes/Ken's_room.unity` directly to test the puzzle. Merely including Michael in the build list does not load his room at startup. See [start screen setup](StartScreen.md).

Once all three matching relics are installed and their opening animations finish, the existing `VampireEscape` win message appears. Its progress label changes to the next-room instructions. Click the **left controller thumbstick** or press **N** on the keyboard. A button held before winning must be released and pressed again. While loading, further presses are ignored.

`Assets/Ken/VampireProps/RoomSceneTransition.cs` is attached to the `VampireRelics` prefab beside `VampireEscape`. It listens to `OnEscaped`, checks `HasWon` again before loading, and reuses `ControllerSampleActions/LeftHand/Primary2DAxisClick`. Its serialized destination is `Assets/Michael/Scenes/MichaelManorHall.unity`. No new input action asset is required. `TryLoadNextRoom()` can also be called from a future UI button, with the same win gate.

The transition uses `LoadSceneMode.Single`: Unity unloads Ken's scene and creates Michael's scene with its own player rig, cameras, lights, colliders and puzzle state. No origin offsets or manually hidden room copies are needed. Do not mark either room or its player rig `DontDestroyOnLoad`. This is a one-way transition; returning to Ken or preserving completed-room state is not implemented.

`EditorBuildSettings.asset` lists the foyer first, followed by Ken and Michael. The existing Web build profile inherits this global scene list.

If Unity was open when these files changed, stop Play Mode, let scripts compile, and reopen Ken's scene normally before testing. Avoid saving an older in-memory prefab over the updated prefab.

## Checks

- Before winning, left thumbstick/N must not leave Ken's room.
- Complete all three locks and wait for the win heading and continuation instructions.
- Hold the continuation input while winning: release it before pressing again.
- Continue and confirm that Michael's room appears and Ken's room disappears from the Hierarchy.
- Confirm there is only Michael's player rig and no duplicate active camera/AudioListener from Ken.
- Test movement and grabbing in Michael's room on the target headset. Keyboard continuation is available for desktop inspection.

An unavailable destination produces a visible message and keeps the player in Ken's scene.

## Arrival and simulator offsets

Michael's XR Origin has `MichaelManor.RoomSpawnAlignment`. The XR Interaction Simulator persists across scene changes, including its simulated head position relative to the rig. A player who moved away from the tracking origin in Ken's room can otherwise arrive outside Michael's floor even though the new rig is at `(0, 0, -12.5)`.

The component holds the XR body transformer until the first completed game input update. After TrackedPoseDriver has applied the headset pose, it translates the origin so the player's horizontal head/feet position matches the authored spawn, preserving eye height. It then synchronizes colliders, updates BreakOut's return position, and resumes locomotion. It does not modify the shared XR prefab, disable gravity, or move room geometry.

Reproduction: with a simulated tracking offset `(12, 0, -6)`, the old transition put the head at `(12, 1.361, -18.5)`, outside the floor's x ±9 / z ±16 bounds; eye height fell to approximately -175.7 after six simulated seconds. With alignment, the same offset places the head at `(0, 1.361, -12.5)` and its height remains stable over the same interval. Isolated Unity 6000.5.6f1 Play Mode checks passed for both this offset and a zero offset, including a solid floor below the camera, resumed locomotion, and the corrected BreakOut return position. Physical headset testing remains pending.

## Automated validation (2026-09-23)

Unity 6000.5.6f1 compiled the implementation in a separate project copy. A Play Mode harness verified prefab/controller-action references, pre-win API and keyboard blocking, held-button protection, the continuation prompt, and a fresh N press loading Michael alone while unloading Ken and removing Ken's puzzle/transition components. The harness supplied `HasWon` and invoked the existing win event; it did not play through the three physical puzzles. Headset controls, comfort, and visual layout still need a manual check.

The headless editor logged an exception in `UnityEditor.Search.SearchDatabase` during indexing; the transition checks completed successfully. The test copy routed synthetic keyboard events to the game explicitly because headless mode has no focused Game view. Those test-only input settings were not applied to the project.
