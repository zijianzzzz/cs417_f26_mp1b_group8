using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// Uses the same XRI Select/grip input as a key, but collects immediately instead of remaining held.
[RequireComponent(typeof(Rigidbody))]
public class BloodBatCollectible : XRBaseInteractable
{
    [SerializeField] private BloodBatCollection collection;
    [SerializeField] private Transform orbitCenter;
    [SerializeField] private GameObject model;
    [SerializeField] private Collider catchCollider;
    [SerializeField] private Transform leftWing;
    [SerializeField] private Transform rightWing;
    [SerializeField] private Renderer[] hoverRenderers;
    [SerializeField, Min(.1f)] private float orbitRadius = .35f;
    [SerializeField] private float degreesPerSecond = 18f;
    [SerializeField] private float startingAngle;
    [SerializeField, Min(.1f)] private float respawnSeconds = 5f;
    private Rigidbody body;
    private MaterialPropertyBlock highlight;
    private bool pendingCatch;
    private bool waitingToRespawn;
    private float respawnAt;
    private float flightTime;
    public bool IsAvailable => !waitingToRespawn && !pendingCatch;

    protected override void Awake()
    {
        base.Awake();
        body = GetComponent<Rigidbody>();
        highlight = new MaterialPropertyBlock();
        highlight.SetColor("_BaseColor", new Color(1f, .85f, .2f));
        if (collection == null || orbitCenter == null || model == null || catchCollider == null)
        {
            Debug.LogError("Assign the collection, orbit center, model and catch collider on BloodBatCollectible.", this);
            enabled = false;
        }
    }

    public override bool IsHoverableBy(IXRHoverInteractor interactor)
        => IsAvailable && collection.IsRightHand(interactor) && base.IsHoverableBy(interactor);

    public override bool IsSelectableBy(IXRSelectInteractor interactor)
        => IsAvailable && collection.IsRightHand(interactor) && base.IsSelectableBy(interactor);

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (IsAvailable && collection.IsRightHand(args.interactorObject)) pendingCatch = true;
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        SetHighlight(true);
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        if (!isHovered) SetHighlight(false);
    }

    private void SetHighlight(bool active)
    {
        foreach (var renderer in hoverRenderers)
            if (renderer != null) renderer.SetPropertyBlock(active ? highlight : null);
    }

    public void ResetBat()
    {
        pendingCatch = false;
        waitingToRespawn = false;
        respawnAt = 0f;
        if (model != null) model.SetActive(true);
        if (catchCollider != null) catchCollider.enabled = true;
        SetHighlight(false);
    }

    private void Update()
    {
        if (waitingToRespawn)
        {
            if (Time.time < respawnAt) return;
            waitingToRespawn = false;
            model.SetActive(true);
            catchCollider.enabled = true;
        }
        flightTime += Time.deltaTime;
        float flap = Mathf.Sin(flightTime * 5f) * 22f;
        if (leftWing != null) leftWing.localRotation = Quaternion.Euler(0, 0, -flap);
        if (rightWing != null) rightWing.localRotation = Quaternion.Euler(0, 0, flap);
    }

    private void FixedUpdate()
    {
        if (waitingToRespawn || pendingCatch) return;
        float angle = (startingAngle + flightTime * degreesPerSecond) * Mathf.Deg2Rad;
        var offset = new Vector3(Mathf.Cos(angle) * orbitRadius, Mathf.Sin(flightTime * 1.4f) * .08f, Mathf.Sin(angle) * orbitRadius);
        body.MovePosition(orbitCenter.position + offset);
        body.MoveRotation(Quaternion.LookRotation(new Vector3(-Mathf.Sin(angle), 0, Mathf.Cos(angle))));
    }

    private void LateUpdate()
    {
        if (!pendingCatch) return;
        // Finish XRI's selection callbacks before cancelling the selection and hiding its target.
        pendingCatch = false;
        waitingToRespawn = true;
        respawnAt = Time.time + respawnSeconds;
        if (interactionManager != null) interactionManager.CancelInteractableSelection((IXRSelectInteractable)this);
        catchCollider.enabled = false;
        model.SetActive(false);
        SetHighlight(false);
        collection.RecordCatch();
    }
}
