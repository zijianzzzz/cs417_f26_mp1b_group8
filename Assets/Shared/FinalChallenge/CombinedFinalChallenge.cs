using TMPro;
using UnityEngine;

namespace MichaelManor
{
    // Owns access and final victory; the bat and lever components retain their original mechanics.
    public sealed class CombinedFinalChallenge : MonoBehaviour
    {
        [SerializeField] private ManorThreeStagePuzzle manorRitual;
        [SerializeField] private ManorKeyReleasePuzzle leverPuzzle;
        [SerializeField] private BloodBatCollection batCollection;
        [SerializeField] private BloodBatCollectible[] bats;
        [SerializeField] private GameObject interactionRoot;
        [SerializeField] private WinCelebrationController celebration;
        [SerializeField] private TMP_Text entranceLabel;
        private bool started;

        public bool IsAvailable { get; private set; }
        public bool IsComplete => IsAvailable && leverPuzzle != null && leverPuzzle.IsSolved;
        public ManorKeyReleasePuzzle LeverPuzzle => leverPuzzle;
        public BloodBatCollection BatCollection => batCollection;

        private void OnEnable()
        {
            if (leverPuzzle != null) leverPuzzle.Solved += CompleteChallenge;
        }

        private void OnDisable()
        {
            if (leverPuzzle != null) leverPuzzle.Solved -= CompleteChallenge;
        }

        private void Start()
        {
            started = true;
            ResetChallenge();
        }

        public void BeginChallenge()
        {
            if (IsAvailable || manorRitual == null || !manorRitual.ExitIsOpen) return;
            IsAvailable = true;
            leverPuzzle.enabled = true;
            interactionRoot.SetActive(true);
            batCollection.enabled = true;
            // Re-enabled lever views must receive the reset too, including after a completed round.
            leverPuzzle.ResetPuzzle();
            if (entranceLabel != null) entranceLabel.text = "FINAL RITUAL\nENTER THROUGH THE OPEN DOOR";
        }

        public void ResetChallenge()
        {
            // Michael's Awake resets his ritual before all scene components have cached their poses.
            if (!started) return;
            IsAvailable = false;
            leverPuzzle.ResetPuzzle();
            leverPuzzle.enabled = false;
            batCollection.ResetCollection();
            batCollection.enabled = false;
            foreach (BloodBatCollectible bat in bats)
                if (bat != null) bat.ResetBat();
            interactionRoot.SetActive(false);
            celebration.ResetCelebration();
            if (entranceLabel != null) entranceLabel.text = "FINAL RITUAL\nSEALED UNTIL THE MANOR RITUAL IS COMPLETE";
        }

        private void CompleteChallenge(ManorKeyReleasePuzzle puzzle)
        {
            if (!IsAvailable || !puzzle.IsSolved) return;
            batCollection.enabled = false;
            interactionRoot.SetActive(false);
            celebration.TriggerWin();
            if (entranceLabel != null) entranceLabel.text = "BOTH RITUALS MASTERED\nYOU ESCAPED";
        }
    }
}
