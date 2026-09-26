using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MichaelManor
{
    public sealed class ManorKeyReleasePuzzle : MonoBehaviour
    {
        [SerializeField] private ManorThreeStagePuzzle ritual;
        [SerializeField] private int requiredRitualStage = -1;
        [SerializeField] private int[] requiredOrder;
        [SerializeField] private Transform barrier;
        [SerializeField] private Vector3 openLocalPosition;
        [SerializeField] private ManorKeyArtifact releasedKey;
        [SerializeField] private TMP_Text status;
        [SerializeField] private float moveDuration = 1f;
        [SerializeField] private BloodBatCollection batCharge;
        [SerializeField, Min(1)] private int batsPerLever = 3;
        private Vector3 closedPosition;
        private int sequencePosition;
        private Coroutine routine;
        public bool IsSolved { get; private set; }
        public int SequencePosition => sequencePosition;
        public Transform Barrier => barrier;
        public event Action<ManorKeyReleasePuzzle> Solved;
        public event Action<int, bool> ButtonFeedbackRequested;
        public event Action ButtonFeedbackReset;

        public void Configure(ManorThreeStagePuzzle ritualController, int unlockStage, int[] order,
            Transform movingBarrier, Vector3 openedPosition, ManorKeyArtifact key, TMP_Text statusText)
        {
            ritual = ritualController; requiredRitualStage = unlockStage; requiredOrder = order;
            barrier = movingBarrier; openLocalPosition = openedPosition; releasedKey = key; status = statusText;
            closedPosition = barrier != null ? barrier.localPosition : Vector3.zero;
        }
        private void Awake() { if (barrier != null) closedPosition = barrier.localPosition; ResetPuzzle(); }
        private void OnEnable()
        {
            if (ritual != null) ritual.PuzzleReset += ResetPuzzle;
            if (batCharge != null) batCharge.CatchRecorded += RefreshBatCharge;
        }
        private void OnDisable()
        {
            if (ritual != null) ritual.PuzzleReset -= ResetPuzzle;
            if (batCharge != null) batCharge.CatchRecorded -= RefreshBatCharge;
        }

        public bool Press(int index)
        {
            if (!isActiveAndEnabled || IsSolved || routine != null || requiredOrder == null || requiredOrder.Length == 0) return false;
            if (requiredRitualStage >= 0 && (ritual == null || ritual.CurrentStage < requiredRitualStage))
            {
                if (status != null) status.text = "SEALED - COMPLETE THE CELESTIAL LOCK";
                ButtonFeedbackRequested?.Invoke(index, false);
                return false;
            }
            if (batCharge != null && batCharge.CaughtCount < batsPerLever)
            {
                RefreshBatCharge(batCharge.CaughtCount);
                ButtonFeedbackRequested?.Invoke(index, false);
                return false;
            }
            if (index != requiredOrder[sequencePosition])
            {
                sequencePosition = 0;
                if (batCharge != null) batCharge.ResetCollection();
                if (status != null) status.text = "WRONG ORDER - BEGIN AGAIN";
                ButtonFeedbackRequested?.Invoke(index, false);
                return false;
            }
            sequencePosition++;
            // The next lever needs a new batch of catches, even if the player caught extra bats first.
            if (batCharge != null && sequencePosition < requiredOrder.Length)
            {
                batCharge.ResetCollection();
            }
            if (status != null) status.text = $"SEQUENCE  {sequencePosition} / {requiredOrder.Length}";
            ButtonFeedbackRequested?.Invoke(index, true);
            if (sequencePosition == requiredOrder.Length) routine = StartCoroutine(OpenRoutine());
            else if (batCharge != null) RefreshBatCharge(batCharge.CaughtCount);
            return true;
        }

        private IEnumerator OpenRoutine()
        {
            Vector3 start = barrier != null ? barrier.localPosition : Vector3.zero; float elapsed = 0f;
            while (barrier != null && elapsed < moveDuration)
            { elapsed += Time.deltaTime; barrier.localPosition = Vector3.Lerp(start, openLocalPosition, Mathf.SmoothStep(0f, 1f, elapsed / moveDuration)); yield return null; }
            if (barrier != null) barrier.localPosition = openLocalPosition;
            if (releasedKey != null) releasedKey.gameObject.SetActive(true);
            SetReleasedKeyGrabbable(true);
            IsSolved = true; routine = null;
            if (status != null) status.text = releasedKey != null ? releasedKey.ArtifactId.ToUpperInvariant() + " RELEASED" : "FINAL EXIT OPEN - YOU ESCAPED";
            Solved?.Invoke(this);
        }

        public void SolveForTest()
        {
            if (requiredRitualStage >= 0 && ritual != null && ritual.CurrentStage < requiredRitualStage) return;
            sequencePosition = requiredOrder.Length;
            if (barrier != null) barrier.localPosition = openLocalPosition;
            if (releasedKey != null) releasedKey.gameObject.SetActive(true);
            SetReleasedKeyGrabbable(true);
            IsSolved = true; if (status != null) status.text = releasedKey != null ? releasedKey.ArtifactId.ToUpperInvariant() + " RELEASED" : "FINAL EXIT OPEN - YOU ESCAPED"; Solved?.Invoke(this);
        }

        public void ForceSolveForTest()
        {
            int oldRequirement = requiredRitualStage; requiredRitualStage = -1;
            SolveForTest(); requiredRitualStage = oldRequirement;
        }

        public void ResetPuzzle()
        {
            if (routine != null) StopCoroutine(routine); routine = null; sequencePosition = 0; IsSolved = false;
            if (barrier != null) barrier.localPosition = closedPosition;
            SetReleasedKeyGrabbable(false);
            if (batCharge != null) batCharge.ResetCollection();
            if (status != null) status.text = requiredRitualStage < 0 ? $"SEQUENCE  0 / {requiredOrder?.Length ?? 0}" : "SEALED UNTIL THE CELESTIAL LOCK";
            if (batCharge != null) RefreshBatCharge(0);
            ButtonFeedbackReset?.Invoke();
        }

        private void RefreshBatCharge(int caught)
        {
            if (IsSolved || routine != null || status == null) return;
            string lever = sequencePosition == 0 ? "LEFT" : "RIGHT";
            status.text = caught >= batsPerLever
                ? lever + " LEVER CHARGED - PULL IT"
                : "CATCH " + (batsPerLever - caught) + " MORE BATS TO CHARGE " + lever;
        }

        private void SetReleasedKeyGrabbable(bool available)
        {
            XRGrabInteractable grab = releasedKey != null ? releasedKey.GetComponent<XRGrabInteractable>() : null;
            if (grab != null) grab.enabled = available;
        }
    }
}
