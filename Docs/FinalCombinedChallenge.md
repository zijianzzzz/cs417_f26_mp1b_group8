# Blood Moon Seal

Status: **Final combined puzzle finished** (2026-09-26). Implementation and
automated Play Mode checks are complete; physical headset validation remains a
manual check as described below.

The final chamber is saved under `Final Combined Ritual` in
`Assets/Michael/Scenes/MichaelManorHall.unity`, directly beyond Michael's exit.
It shares his scene, XR Origin, locomotion, and right-hand input. No extra scene
load is needed. The entrance stays physically sealed and its interactions remain
inactive until all three manor locks and the exit-door animation finish.

## Player sequence

1. Finish Michael's ritual and walk through its open exit.
2. Aim the right-hand ray at a bat and press grip. Catch three bats.
3. Select the LEFT lever. This consumes the charge.
4. Catch three more bats, then select RIGHT.
5. The final gate rises, then the victory announcement and sound play.

Bats respawn after 2.5 seconds. Hover highlights, a catch counter, a charge
counter, lever color feedback, and a current-action label identify the relevant
targets. A lever without enough charge is rejected. A charged lever pressed out
of order clears both sequence progress and charge. Extra early catches cannot
precharge the second lever. RESTART clears the entire manor and final challenge.

## Combined mechanic contribution

Ken contributes right-hand ray/grip bat targeting and a catch threshold. Michael
contributes the two-lever LEFT-to-RIGHT sequence from BloodSigilLeverPuzzle. Each
correct lever step requires a new bat charge, so the player alternates between
both learned mechanics to unlock one final gate. This is not a pair of independent
completion flags. The stone vault, blood-red lighting, bats, and ritual labels
continue the vampire/manor theme.

## Implementation

`CombinedFinalChallenge` owns access, reset, and final victory. The reused
`BloodBatCollection` has an optional charge-only mode; its default still releases
Ken's blood vial. `ManorKeyReleasePuzzle` has an optional bat-charge dependency;
Michael's original key-release puzzles retain their existing behavior. The hall
task board directs the player into the final chamber instead of announcing an
early win. Room geometry, references, colliders, labels, and materials are saved
as native Unity assets; no runtime room generation is required.

## Validation and manual check

Validated in an isolated Unity 6000.5.6f1 copy using Play Mode and actual XRI
selection callbacks with a simulated right-hand interactor. The harness finishes
Michael's ritual through its presentation helper; it does not physically solve
the three preceding manor puzzles. Checks cover sealed startup, early input
rejection, one count per catch, respawn, wrong-order reset, fresh charges, animated
gate completion, final victory routing, and complete restart. Collider checks
verify a level floor along the center route, clear open doorways, and wrong-hand
bat rejection. A rendered room preview was reviewed for readable labels.

The batch editor also reports a pre-existing UnityEditor.Search.SearchDatabase
indexing exception. This is an editor search error outside the gameplay scripts;
the gameplay checks can still complete. Physical Quest ray/grip input, comfort,
audio balance, and walking the full route remain headset checks.

If the scene was already open when updated, stop Play Mode and reload it from
disk before testing. Avoid saving the old in-memory scene over the new chamber.
