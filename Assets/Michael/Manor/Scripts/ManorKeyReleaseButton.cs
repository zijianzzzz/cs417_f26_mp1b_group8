using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MichaelManor
{
    [RequireComponent(typeof(XRSimpleInteractable))]
    public sealed class ManorKeyReleaseButton : MonoBehaviour
    {
        [SerializeField] private ManorKeyReleasePuzzle puzzle;
        [SerializeField] private int buttonIndex;
        [SerializeField] private Renderer buttonRenderer;
        [SerializeField] private Light feedbackLight;
        [SerializeField, Min(0.005f)] private float pressDepth = 0.045f;
        [SerializeField, Min(0.05f)] private float pressDuration = 0.16f;
        [SerializeField, Min(0.05f)] private float rejectedFlashDuration = 0.42f;
        private XRSimpleInteractable interactable;
        private MaterialPropertyBlock propertyBlock;
        private Vector3 restLocalPosition;
        private Color restBaseColor = Color.white;
        private Color restEmissionColor = Color.black;
        private float restLightIntensity;
        private Color restLightColor = Color.white;
        private Coroutine animationRoutine;
        public void Configure(ManorKeyReleasePuzzle owner, int index) { puzzle = owner; buttonIndex = index; }

        public Light FeedbackLight => feedbackLight;

        private void Awake()
        {
            interactable = GetComponent<XRSimpleInteractable>();
            if (buttonRenderer == null) buttonRenderer = GetComponent<Renderer>();
            if (feedbackLight == null) feedbackLight = GetComponentInChildren<Light>(true);
            propertyBlock = new MaterialPropertyBlock();
            restLocalPosition = transform.localPosition;
            CaptureRestAppearance();
            ApplyRestAppearance();
        }

        private void OnEnable()
        {
            if (interactable == null) interactable = GetComponent<XRSimpleInteractable>();
            interactable.selectEntered.AddListener(Selected);
            if (puzzle != null)
            {
                puzzle.ButtonFeedbackRequested += HandleFeedback;
                puzzle.ButtonFeedbackReset += ResetFeedback;
            }
        }

        private void OnDisable()
        {
            if (interactable != null) interactable.selectEntered.RemoveListener(Selected);
            if (puzzle != null)
            {
                puzzle.ButtonFeedbackRequested -= HandleFeedback;
                puzzle.ButtonFeedbackReset -= ResetFeedback;
            }
            StopAnimation();
        }

        private void Selected(SelectEnterEventArgs args) => puzzle?.Press(buttonIndex);
        public bool PressForTest() => puzzle != null && puzzle.Press(buttonIndex);

        private void HandleFeedback(int pressedIndex, bool accepted)
        {
            if (!accepted)
            {
                if (pressedIndex == buttonIndex) StartFeedbackAnimation(false);
                else ApplyRestAppearance();
                return;
            }

            if (pressedIndex == buttonIndex) StartFeedbackAnimation(true);
        }

        private void StartFeedbackAnimation(bool accepted)
        {
            StopAnimation();
            animationRoutine = StartCoroutine(AnimatePress(accepted));
        }

        private IEnumerator AnimatePress(bool accepted)
        {
            Color color = accepted ? new Color(0.20f, 0.88f, 1f) : new Color(1f, 0.08f, 0.04f);
            ApplyAppearance(color, accepted ? 5.5f : 7f);

            Vector3 pressedPosition = restLocalPosition + transform.localRotation * Vector3.up * pressDepth;
            float halfDuration = pressDuration * 0.5f;
            yield return MoveButton(restLocalPosition, pressedPosition, halfDuration);
            yield return MoveButton(pressedPosition, restLocalPosition, halfDuration);

            if (!accepted)
            {
                yield return new WaitForSeconds(Mathf.Max(0f, rejectedFlashDuration - pressDuration));
                ApplyRestAppearance();
            }
            animationRoutine = null;
        }

        private IEnumerator MoveButton(Vector3 from, Vector3 to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
                transform.localPosition = Vector3.LerpUnclamped(from, to, t);
                yield return null;
            }
            transform.localPosition = to;
        }

        private void ResetFeedback()
        {
            StopAnimation();
            transform.localPosition = restLocalPosition;
            ApplyRestAppearance();
        }

        private void StopAnimation()
        {
            if (animationRoutine == null) return;
            StopCoroutine(animationRoutine);
            animationRoutine = null;
            transform.localPosition = restLocalPosition;
        }

        private void CaptureRestAppearance()
        {
            Material material = buttonRenderer != null ? buttonRenderer.sharedMaterial : null;
            if (material != null)
            {
                if (material.HasProperty("_BaseColor")) restBaseColor = material.GetColor("_BaseColor");
                else if (material.HasProperty("_Color")) restBaseColor = material.GetColor("_Color");
                if (material.HasProperty("_EmissionColor")) restEmissionColor = material.GetColor("_EmissionColor");
            }
            if (feedbackLight != null)
            {
                restLightIntensity = feedbackLight.intensity;
                restLightColor = feedbackLight.color;
            }
        }

        private void ApplyRestAppearance()
        {
            ApplyRendererColors(restBaseColor, restEmissionColor);
            if (feedbackLight != null)
            {
                feedbackLight.color = restLightColor;
                feedbackLight.intensity = restLightIntensity;
            }
        }

        private void ApplyAppearance(Color color, float intensity)
        {
            ApplyRendererColors(color, color * 2f);
            if (feedbackLight != null)
            {
                feedbackLight.color = color;
                feedbackLight.intensity = intensity;
            }
        }

        private void ApplyRendererColors(Color baseColor, Color emissionColor)
        {
            if (buttonRenderer == null) return;
            buttonRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_BaseColor", baseColor);
            propertyBlock.SetColor("_Color", baseColor);
            propertyBlock.SetColor("_EmissionColor", emissionColor);
            buttonRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}
