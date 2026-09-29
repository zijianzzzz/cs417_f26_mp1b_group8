# CS 417 MP1B — Group 8

Team Unity VR project, starting from Zijian's saved MP1A project on September 21, 2026, including local scene/settings edits and Resources assets.

## Contributors

- Zijian Zhong (zijianz7@illinois.edu)
- Ziyang Li (ziyangl7@illinois.edu)

## Open the project

1. Install Git, Git LFS, and Unity **6000.5.6f1** through Unity Hub. Install Android Build Support (SDK/NDK and OpenJDK) for Quest builds.
2. Clone the repository:

   ```sh
   git lfs install
   git clone https://github.com/zijianzzzz/cs417_f26_mp1b_group8.git
   cd cs417_f26_mp1b_group8
   git lfs pull
   ```

3. In Unity Hub, add this repository folder as a project. It directly contains `Assets`, `Packages`, and `ProjectSettings`; there is no nested `mp1a` folder.
4. Let Unity finish importing assets and resolving packages. Open `Assets/Shared/Scenes/StartScreen.unity`.
5. Enter Play Mode and check the Console. The first import regenerates `Library` and can take several minutes.

The baseline uses URP 17.5.0, Input System 1.20.0, XR Interaction Toolkit 3.5.1, and OpenXR 1.17.1. Use the committed package manifest and lockfile; coordinate any editor or package upgrades with the team.

## Team contributions

Read [CONTRIBUTING.md](CONTRIBUTING.md) before adding a scene. Each teammate owns a separate scene and asset folder. For now, work directly on `main`, pull teammate updates before pushing, and coordinate shared scene edits. Commit asset `.meta` files alongside the assets.

Owner folders: `Assets/Ken` and `Assets/Michael`. Shared XR assets, samples, fonts, input actions, and project configuration remain outside these folders. The project-wide input asset is `Assets/InputSystem_Actions.inputactions`.

The repository owner must invite teammates through GitHub repository Settings → Collaborators. Write access is needed to push branches to this repository.

## Existing gameplay and documentation

- [MP1A gameplay reference and controls](Docs/MP1A-Reference.md)
- [Vampire puzzle setup](Docs/VampireRelics.md)
- [Project context and inherited development notes](Docs/AI/UnityProjectContext.md)

The build starts in a separate foyer, `StartScreen`. Select **START GAME** with the XR UI trigger or mouse, or press **Enter**, to enter `Ken's_room`. After solving all three seals and seeing the win message, click the **left controller thumbstick** or press **N** to enter `MichaelManorHall`. Only one scene is loaded at a time, so both rooms can stay at the origin. See [start screen setup](Docs/StartScreen.md) and [room transition checks](Docs/RoomTransitions.md). The project keeps its inherited Unity application settings; agree on any MP1B application-name or Android package-identifier changes before a release.

Michael's ritual opens a connected final chamber, **Blood Moon Seal**. Catch three bats with right-hand ray/grip, pull LEFT, catch three more, then pull RIGHT to open the final exit and win. The chamber combines Ken's bat collection with Michael's ordered levers inside the same scene. See [combined challenge](Docs/FinalCombinedChallenge.md).

## Physics masses

Across Ken's and Michael's room props, the lowest Rigidbody mass is **0.25 kg** (Moonstone and several of Ken's decoys), and the highest is **1.5 kg** (Blood Sigil). The heaviest-to-lightest ratio is **1.5 / 0.25 = 6:1**, below 20:1. Other examples include Ken's relic keys at 0.3 kg, the watering can at 0.6 kg, and Michael's Silver Fang at 0.8 kg. Moonstone and Blood Sigil have matching mass values in the scene and their carry prefabs. Collision behavior still needs a Play Mode/headset check.

Including imported sample scenes and prefabs under `Assets`, the serialized mass range is **0.25–2 kg**, or **8:1**. The 2 kg maximum belongs to the XR Interaction Toolkit sample `Torus-Cut.prefab`; it is not referenced by the two game rooms or their shared assets.

This repository starts with a fresh Git history. MP1A's repository remains separate. Generated caches, builds, debug backups, and individual submission documents/videos are not part of this baseline. The room transition has passed an isolated Unity Play Mode check with a simulated win; full puzzle and headset validation remain manual checks.
