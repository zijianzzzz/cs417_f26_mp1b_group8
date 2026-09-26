using System.Collections;
using TMPro;
using UnityEngine;

namespace MichaelManor
{
    /// <summary>
    /// Final-game celebration triggered when Michael's last ritual is complete.
    /// </summary>
    public sealed class WinCelebrationController : MonoBehaviour
    {
        [SerializeField] private GameObject celebrationRoot;
        [SerializeField] private ParticleSystem[] particles;
        [SerializeField] private AudioSource victoryAudio;
        [SerializeField] private TMP_Text congratulationsText;
        [SerializeField] private float congratulationsDuration = 5f;
        [SerializeField] private float textFadeDuration = 0.65f;

        private bool hasWon;
        private Coroutine textRoutine;

        public bool HasWon => hasWon;

        public void Configure(
            GameObject root,
            ParticleSystem[] celebrationParticles,
            AudioSource audioSource = null,
            TMP_Text announcementText = null)
        {
            celebrationRoot = root;
            particles = celebrationParticles;
            victoryAudio = audioSource;
            congratulationsText = announcementText;
        }

        private void Awake()
        {
            if (celebrationRoot != null)
            {
                celebrationRoot.SetActive(false);
            }
        }

        [ContextMenu("Trigger Win Celebration")]
        public void TriggerWin()
        {
            if (hasWon)
            {
                return;
            }

            hasWon = true;

            if (celebrationRoot != null)
            {
                celebrationRoot.SetActive(true);
            }

            if (particles != null)
            {
                foreach (ParticleSystem particle in particles)
                {
                    if (particle != null)
                    {
                        particle.Play(true);
                    }
                }
            }

            if (victoryAudio != null)
            {
                victoryAudio.Play();
            }

            if (congratulationsText != null)
            {
                if (textRoutine != null)
                {
                    StopCoroutine(textRoutine);
                }

                textRoutine = StartCoroutine(ShowCongratulations());
            }

            Debug.Log("Win Celebration triggered.");
        }

        [ContextMenu("Reset Win Celebration")]
        public void ResetCelebration()
        {
            hasWon = false;

            if (textRoutine != null)
            {
                StopCoroutine(textRoutine);
                textRoutine = null;
            }

            if (particles != null)
            {
                foreach (ParticleSystem particle in particles)
                {
                    if (particle != null)
                    {
                        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    }
                }
            }

            if (celebrationRoot != null)
            {
                celebrationRoot.SetActive(false);
            }

            if (congratulationsText != null)
            {
                SetTextAlpha(0f);
                congratulationsText.gameObject.SetActive(false);
            }
        }

        private IEnumerator ShowCongratulations()
        {
            congratulationsText.gameObject.SetActive(true);
            SetTextAlpha(0f);

            float fade = Mathf.Min(textFadeDuration, congratulationsDuration * 0.4f);
            float elapsed = 0f;
            while (elapsed < fade)
            {
                elapsed += Time.deltaTime;
                SetTextAlpha(Mathf.SmoothStep(0f, 1f, elapsed / fade));
                yield return null;
            }

            SetTextAlpha(1f);
            float holdDuration = Mathf.Max(0f, congratulationsDuration - fade * 2f);
            if (holdDuration > 0f)
            {
                yield return new WaitForSeconds(holdDuration);
            }

            elapsed = 0f;
            while (elapsed < fade)
            {
                elapsed += Time.deltaTime;
                SetTextAlpha(Mathf.SmoothStep(1f, 0f, elapsed / fade));
                yield return null;
            }

            SetTextAlpha(0f);
            congratulationsText.gameObject.SetActive(false);
            textRoutine = null;
            Debug.Log("Congratulations text hidden after five seconds.");
        }

        private void SetTextAlpha(float alpha)
        {
            Color color = congratulationsText.color;
            color.a = alpha;
            congratulationsText.color = color;
        }
    }
}
