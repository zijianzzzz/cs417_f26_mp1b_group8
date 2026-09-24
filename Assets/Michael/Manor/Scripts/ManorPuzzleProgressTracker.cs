using TMPro;
using UnityEngine;

namespace MichaelManor
{
    public sealed class ManorPuzzleProgressTracker : MonoBehaviour
    {
        [SerializeField] private ManorKeyReleasePuzzle firstPuzzle;
        [SerializeField] private ManorMoonCryptPuzzle moonPuzzle;
        [SerializeField] private ManorKeyReleasePuzzle thirdPuzzle;
        [SerializeField] private ManorThreeStagePuzzle ritual;
        [SerializeField] private TMP_Text display;
        [SerializeField] private bool[] clues = new bool[3];
        public int PuzzlesSolved { get; private set; }
        public int CluesFound { get; private set; }
        public TMP_Text Display => display;

        public void Configure(ManorKeyReleasePuzzle first, ManorMoonCryptPuzzle moon, ManorKeyReleasePuzzle third, ManorThreeStagePuzzle ritualController, TMP_Text text)
        { firstPuzzle = first; moonPuzzle = moon; thirdPuzzle = third; ritual = ritualController; display = text; ResetProgress(); }
        private void OnEnable() { if (ritual != null) ritual.PuzzleReset += ResetProgress; }
        private void OnDisable() { if (ritual != null) ritual.PuzzleReset -= ResetProgress; }
        private void Update()
        {
            Recalculate();
        }
        public void RecalculateForTest() => Recalculate();
        private void Recalculate()
        {
            int solved = (firstPuzzle != null && firstPuzzle.IsSolved ? 1 : 0) + (moonPuzzle != null && moonPuzzle.IsSolved ? 1 : 0) + (thirdPuzzle != null && thirdPuzzle.IsSolved ? 1 : 0);
            if (solved != PuzzlesSolved) { PuzzlesSolved = solved; Refresh(); }
        }
        public bool DiscoverClue(int index)
        {
            if (index < 0 || index >= clues.Length || clues[index]) return false;
            clues[index] = true; CluesFound++; Refresh(); return true;
        }
        public void ResetProgress()
        { clues = new bool[3]; PuzzlesSolved = 0; CluesFound = 0; Refresh(); }
        private void Refresh() { if (display != null) display.text = $"PUZZLES  {PuzzlesSolved} / 3     CLUES  {CluesFound} / 3"; }
    }
}
