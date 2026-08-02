using System.Collections;
using TMPro;
using UnityEngine;

public class ComboEffect : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text comboText;

    [SerializeField]
    private string comboFormat = "{0}x COMBO";

    [Header("Scale Animation")]
    [SerializeField]
    private Vector3 startScale =
        new Vector3(0.05f, 0.05f, 0.05f);

    [SerializeField]
    private Vector3 punchScale =
        new Vector3(1.35f, 1.35f, 1.35f);

    [SerializeField]
    private Vector3 normalScale =
        Vector3.one;

    [SerializeField, Min(0.01f)]
    private float growDuration = 0.08f;

    [SerializeField, Min(0.01f)]
    private float shrinkDuration = 0.12f;

    [Header("Billboard")]
    [SerializeField]
    private bool faceCamera = true;

    private Coroutine scaleCoroutine;
    private Camera mainCamera;

    private void Awake()
    {
        if (comboText == null)
        {
            comboText =
                GetComponentInChildren<TMP_Text>(true);
        }

        mainCamera = Camera.main;

        HideComboText();
    }

    private void LateUpdate()
    {
        if (!faceCamera ||
            comboText == null ||
            !comboText.enabled ||
            mainCamera == null)
        {
            return;
        }

        transform.rotation =
            Quaternion.LookRotation(
                transform.position -
                mainCamera.transform.position);
    }

    public void ShowCombo(int combo)
    {
        if (combo <= 0)
        {
            ResetCombo();
            return;
        }

        if (comboText == null)
        {
            Debug.LogError(
                "ComboEffect: Combo Text belum diisi.");

            return;
        }

        comboText.enabled = true;

        comboText.text =
            string.Format(
                comboFormat,
                combo);

        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }

        scaleCoroutine =
            StartCoroutine(
                PlayPulseAnimation());
    }

    public void ResetCombo()
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
            scaleCoroutine = null;
        }

        HideComboText();
    }

    private IEnumerator PlayPulseAnimation()
    {
        transform.localScale = startScale;

        yield return AnimateScale(
            startScale,
            punchScale,
            growDuration);

        yield return AnimateScale(
            punchScale,
            normalScale,
            shrinkDuration);

        transform.localScale = normalScale;
        scaleCoroutine = null;
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

    private void HideComboText()
    {
        transform.localScale = normalScale;

        if (comboText != null)
        {
            comboText.enabled = false;
        }
    }
}