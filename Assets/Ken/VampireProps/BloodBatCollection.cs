using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// Owns the house scoreboard and the one-time blood-vial reward for this round.
public class BloodBatCollection : MonoBehaviour
{
    [SerializeField] private Transform rightController;
    [SerializeField] private GameObject bloodVial;
    [SerializeField] private TMP_Text countLabel;
    [SerializeField] private TMP_Text rewardLabel;
    [SerializeField, Min(1)] private int requiredBats = 3;
    [SerializeField] private bool releaseVial = true;
    public int CaughtCount { get; private set; }
    public bool VialReleased { get; private set; }
    public event System.Action<int> CatchRecorded;

    private void Awake()
    {
        if (rightController == null || (releaseVial && bloodVial == null) || countLabel == null || rewardLabel == null)
        {
            Debug.LogError("Assign the right controller, blood vial and house bat scoreboard on BloodBatCollection.", this);
            enabled = false;
            return;
        }
        ResetCollection();
    }

    public bool IsRightHand(IXRInteractor interactor)
    {
        return isActiveAndEnabled && interactor != null && rightController != null &&
            interactor.transform.IsChildOf(rightController);
    }

    internal void RecordCatch()
    {
        if (!isActiveAndEnabled) return;
        CaughtCount++;
        if (releaseVial && !VialReleased && CaughtCount >= requiredBats)
        {
            VialReleased = true;
            bloodVial.SetActive(true);
        }
        Refresh();
        CatchRecorded?.Invoke(CaughtCount);
    }

    public void ResetCollection()
    {
        CaughtCount = 0;
        VialReleased = false;
        if (releaseVial && bloodVial != null) bloodVial.SetActive(false);
        Refresh();
    }

    private void Refresh()
    {
        if (countLabel == null || rewardLabel == null) return;
        countLabel.text = "BATS CAUGHT: " + CaughtCount;
        if (!releaseVial)
        {
            rewardLabel.text = "RITUAL CHARGE  " + Mathf.Min(CaughtCount, requiredBats) + " / " + requiredBats;
            return;
        }
        rewardLabel.text = VialReleased ? "BLOOD VIAL READY" : "MORE BLOOD BATS NEEDED\n" + CaughtCount + " / " + requiredBats;
        rewardLabel.color = VialReleased ? new Color(.3f, 1f, .55f) : new Color(1f, .8f, .35f);
    }
}
