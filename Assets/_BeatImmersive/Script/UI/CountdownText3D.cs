using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CountdownText3D : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text countdownText;

    [Header("Scale")]
    [SerializeField]
    private Vector3 smallScale =
        new Vector3(0.05f, 0.05f, 0.05f);

    [SerializeField]
    private Vector3 bigScale =
        new Vector3(1.35f, 1.35f, 1.35f);

    [SerializeField]
    private Vector3 normalScale =
        Vector3.one;

    [Header("Duration Per Text")]
    [SerializeField, Min(0.01f)]
    private float growDuration = 0.12f;

    [SerializeField, Min(0.01f)]
    private float settleDuration = 0.12f;

    [SerializeField, Min(0f)]
    private float holdDuration = 0.45f;

    [Header("Billboard")]
    [SerializeField]
    private bool faceCamera = true;

    private Coroutine countdownCoroutine;
    private Camera mainCamera;

    private void Awake()
    {
        if (countdownText == null)
        {
            countdownText =
                GetComponentInChildren<TMP_Text>(true);
        }

        mainCamera = Camera.main;

        HideText();
    }

    private void LateUpdate()
    {
        if (!faceCamera ||
            countdownText == null ||
            !countdownText.enabled ||
            mainCamera == null)
        {
            return;
        }

        transform.rotation =
            Quaternion.LookRotation(
                transform.position -
                mainCamera.transform.position);
    }

    public void PlayCountdown(
        Action onGoShown)
    {
        CancelCountdown();

        if (countdownText == null)
        {
            Debug.LogError(
                "CountdownText3D: Countdown Text belum diisi.");

            return;
        }

        countdownCoroutine =
            StartCoroutine(
                CountdownRoutine(onGoShown));
    }

    public void CancelCountdown()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }

        HideText();
    }

    private IEnumerator CountdownRoutine(
        Action onGoShown)
    {
        string[] labels =
        {
            "3",
            "2",
            "1",
            "GO!!!"
        };

        countdownText.enabled = true;

        foreach (string label in labels)
        {
            countdownText.text = label;
            transform.localScale = smallScale;

            yield return AnimateScale(
                smallScale,
                bigScale,
                growDuration);

            yield return AnimateScale(
                bigScale,
                normalScale,
                settleDuration);

            /*
             * Gameplay, musik, dan note mulai
             * ketika tulisan GO sudah tampil.
             */
            if (label == "GO!!!")
            {
                onGoShown?.Invoke();
            }

            if (holdDuration > 0f)
            {
                yield return new WaitForSecondsRealtime(
                    holdDuration);
            }

            countdownText.enabled = false;

            /*
             * Beri jeda satu frame sebelum
             * angka berikutnya ditampilkan.
             */
            yield return null;

            countdownText.enabled = true;
        }

        countdownCoroutine = null;

        HideText();
    }

    private IEnumerator AnimateScale(
        Vector3 from,
        Vector3 to,
        float duration)
    {
        float elapsed = 0f;

        transform.localScale = from;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsed / duration);

            progress =
                progress *
                progress *
                (3f - 2f * progress);

            transform.localScale =
                Vector3.Lerp(
                    from,
                    to,
                    progress);

            yield return null;
        }

        transform.localScale = to;
    }

    private void HideText()
    {
        transform.localScale = smallScale;

        if (countdownText != null)
        {
            countdownText.enabled = false;
        }
    }
}