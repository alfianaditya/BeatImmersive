using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class PinchButton3D : MonoBehaviour
{
    [Header("Action")]
    [SerializeField]
    private UnityEvent onPinch;

    [Header("Press Animation")]
    [Tooltip("Skala tombol ketika ditekan.")]
    [SerializeField]
    private Vector3 pressedScale =
        new Vector3(0.9f, 0.9f, 0.9f);

    [Tooltip("Durasi animasi sebelum tombol menjalankan action.")]
    [SerializeField, Min(0.01f)]
    private float pressDuration = 0.08f;

    [Header("Protection")]
    [Tooltip("Mencegah tombol terpencet berulang kali terlalu cepat.")]
    [SerializeField, Min(0f)]
    private float cooldown = 0.35f;

    private Vector3 normalScale;
    private Coroutine pressCoroutine;
    private float nextAllowedTime;
    private bool isPressed;

    private void Awake()
    {
        normalScale = transform.localScale;
    }

    public void Press()
    {
        if (!isActiveAndEnabled ||
            !gameObject.activeInHierarchy ||
            isPressed ||
            Time.unscaledTime < nextAllowedTime)
        {
            return;
        }

        nextAllowedTime =
            Time.unscaledTime + cooldown;

        pressCoroutine =
            StartCoroutine(PressRoutine());
    }

    private IEnumerator PressRoutine()
    {
        isPressed = true;

        transform.localScale =
            Vector3.Scale(
                normalScale,
                pressedScale);

        yield return new WaitForSecondsRealtime(
            pressDuration);

        transform.localScale = normalScale;

        isPressed = false;
        pressCoroutine = null;

        onPinch?.Invoke();
    }

    private void OnDisable()
    {
        if (pressCoroutine != null)
        {
            StopCoroutine(pressCoroutine);
            pressCoroutine = null;
        }

        isPressed = false;
        transform.localScale = normalScale;
    }
}