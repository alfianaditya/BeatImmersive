using System.Collections;
using TMPro;
using UnityEngine;

public class ComboEffect : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private string comboFormat = "{0}x COMBO";

    [Header("Scale Animation")]
    [SerializeField] private Vector3 startScale = Vector3.zero;
    [SerializeField] private Vector3 punchScale = new Vector3(1.35f, 1.35f, 1.35f);
    [SerializeField] private Vector3 normalScale = Vector3.one;

    [SerializeField, Min(0.01f)] private float growDuration = 0.08f;
    [SerializeField, Min(0.01f)] private float shrinkDuration = 0.12f;

    [Header("Billboard")]
    [SerializeField] private bool faceCamera = true;

    private Coroutine scaleCoroutine;
    private Camera mainCamera;

    private void Awake()
    {
        if (comboText == null)
            comboText = GetComponentInChildren<TMP_Text>();

        mainCamera = Camera.main;
        HideImmediately();
    }

    private void LateUpdate()
    {
        if (!faceCamera ||
            !gameObject.activeSelf ||
            mainCamera == null)
        {
            return;
        }

        transform.rotation = Quaternion.LookRotation(
            transform.position -
            mainCamera.transform.position);
    }

    public void ShowCombo(int combo)
    {
        if (combo <= 0)
        {
            HideImmediately();
            return;
        }

        if (comboText == null)
        {
            Debug.LogError("Combo3DText: TMP_Text belum diisi.");
            return;
        }

        comboText.text = string.Format(comboFormat, combo);
        gameObject.SetActive(true);

        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);

        scaleCoroutine = StartCoroutine(PlayPulse());
    }

    public void ResetCombo()
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
            scaleCoroutine = null;
        }

        HideImmediately();
    }

    private IEnumerator PlayPulse()
    {
        transform.localScale = startScale;

        yield return ScaleTo(punchScale, growDuration);
        yield return ScaleTo(normalScale, shrinkDuration);

        transform.localScale = normalScale;
        scaleCoroutine = null;
    }

    private IEnumerator ScaleTo(Vector3 targetScale, float duration)
    {
        Vector3 initialScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t);

            transform.localScale = Vector3.Lerp(
                initialScale,
                targetScale,
                t);

            yield return null;
        }

        transform.localScale = targetScale;
    }

    private void HideImmediately()
    {
        transform.localScale = startScale;
        gameObject.SetActive(false);
    }
}
