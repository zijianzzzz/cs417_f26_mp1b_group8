using TMPro;
using UnityEngine;

namespace MichaelManor
{
    /// Drives the hall task board from ritual, Gate, and Moon Crypt state so progress
    /// and the next objective stay readable in VR. Recomputed each frame from state, so
    /// the board can never drift from the systems it reports on.
    public sealed class ManorQuestScoreboard : MonoBehaviour
    {
        [SerializeField] private ManorThreeStagePuzzle ritual;
        [SerializeField] private FiveChamberQuestController quest;
        [SerializeField] private ManorMoonCryptPuzzle moonCrypt;
        [SerializeField] private ManorGatePortal[] gates;
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private TMP_Text instructionText;

        private bool usedAnyGate;
        private bool enteredMoonCrypt;
        private int lastKey = int.MinValue;
        private string progressValue;
        private string instructionValue;

        public string ProgressValue => progressValue;
        public string InstructionValue => instructionValue;

        public void Configure(
            ManorThreeStagePuzzle ritualController,
            FiveChamberQuestController questController,
            ManorMoonCryptPuzzle moonCryptPuzzle,
            ManorGatePortal[] hallGates,
            TMP_Text progress,
            TMP_Text instruction)
        {
            ritual = ritualController;
            quest = questController;
            moonCrypt = moonCryptPuzzle;
            gates = hallGates;
            progressText = progress;
            instructionText = instruction;
        }

        private void OnEnable()
        {
            if (gates != null)
                foreach (ManorGatePortal gate in gates)
                    if (gate != null) gate.ActivationRequested += HandleGateUsed;
            if (ritual != null) ritual.PuzzleReset += HandleReset;
            if (quest != null) quest.QuestReset += HandleReset;
        }

        private void OnDisable()
        {
            if (gates != null)
                foreach (ManorGatePortal gate in gates)
                    if (gate != null) gate.ActivationRequested -= HandleGateUsed;
            if (ritual != null) ritual.PuzzleReset -= HandleReset;
            if (quest != null) quest.QuestReset -= HandleReset;
        }

        private void HandleGateUsed(ManorGatePortal gate)
        {
            usedAnyGate = true;
            if (gate.IsMoonCryptGate) enteredMoonCrypt = true;
        }

        private void HandleReset()
        {
            usedAnyGate = false;
            enteredMoonCrypt = false;
            lastKey = int.MinValue;
        }

        private void LateUpdate()
        {
            if (ritual == null) return;
            int stage = ritual.CurrentStage;
            int explored = quest != null ? quest.ExploredCount : 0;
            int objective = CurrentObjective(stage);
            int key = (stage * 8 + explored) * 16 + objective;
            var final = ritual.FinalChallenge;
            int finalState = final == null ? 0 : final.IsComplete ? 3 : ritual.ExitIsOpen ? 2 : 1;
            key = key * 4 + finalState;
            if (key != lastKey)
            {
                lastKey = key;
                int total = ritual.StageCount;
                int remaining = Mathf.Max(0, total - stage);
                progressValue =
                    $"RITUAL PROGRESS    {stage} / {total}\n" +
                    $"KEYS REMAINING     {remaining}\n" +
                    $"LOCKS REMAINING    {remaining}\n" +
                    $"CHAMBERS EXPLORED  {explored} / {FiveChamberQuestController.RequiredChamberCount}";
                instructionValue = Objectives[objective];
                if (objective == 6 && final != null)
                    instructionValue = final.IsComplete
                        ? "YOU ESCAPED\nBOTH RITUALS MASTERED"
                        : ritual.ExitIsOpen
                            ? "MANOR RITUAL COMPLETE\nENTER THE FINAL CHAMBER\nTHROUGH THE EXIT"
                            : "MANOR RITUAL COMPLETE\nTHE FINAL CHAMBER IS OPENING";
            }

            // The ritual controller still writes its own legacy text on stage events; keep ours on top.
            if (progressText != null && !ReferenceEquals(progressText.text, progressValue)) progressText.text = progressValue;
            if (instructionText != null && !ReferenceEquals(instructionText.text, instructionValue)) instructionText.text = instructionValue;
        }

        private static readonly string[] Objectives =
        {
            "STEP I\nPLACE THE SILVER FANG\nIN THE WATCHER LOCK",
            "STEP II\nREAD THE CODEX IN THE OPEN CHEST\nTHEN TOUCH A GLOWING GATE RUNE",
            "STEP II\nFIND THE MOON-MARKED CHAMBER\nITS GATE RUNE GLOWS BLUE-WHITE",
            "STEP II\nIN THE MOON CRYPT PRESS\nWOLF  ->  MOON  ->  BLOOD",
            "STEP II\nTAKE THE MOONSTONE\nTO THE CELESTIAL LOCK",
            "STEP III\nTAKE THE BLOOD SIGIL\nTO THE EXIT LOCK",
            "THE RITUAL IS COMPLETE\nTHE EXIT IS OPEN"
        };

        private int CurrentObjective(int stage)
        {
            if (stage <= 0) return 0;
            if (stage >= ritual.StageCount) return 6;
            if (stage == 2) return 5;
            if (moonCrypt != null && moonCrypt.IsSolved) return 4;
            if (enteredMoonCrypt) return 3;
            return usedAnyGate ? 2 : 1;
        }
    }
}
