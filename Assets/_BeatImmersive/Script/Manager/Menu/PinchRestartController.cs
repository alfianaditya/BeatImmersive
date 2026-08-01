using UnityEngine;

public class PinchRestartController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private HandTrackingManager handTrackingManager;

    [SerializeField]
    private ScoreManager scoreManager;

    [Header("Pinch Settings")]
    [Tooltip("Jarak ibu jari dan telunjuk dibanding ukuran telapak.")]
    [SerializeField, Range(0.1f, 0.8f)]
    private float pinchThreshold = 0.35f;

    [Tooltip("Berapa lama pinch harus ditahan untuk restart.")]
    [SerializeField, Min(0.05f)]
    private float holdDuration = 0.35f;

    [Header("Runtime Debug")]
    [SerializeField] private bool leftPinching;
    [SerializeField] private bool rightPinching;
    [SerializeField] private float pinchHoldTimer;
    [SerializeField] private bool waitingForRelease;

    private bool wasGameOver;

    private void Update()
    {
        if (handTrackingManager == null ||
            scoreManager == null)
        {
            return;
        }

        if (!scoreManager.IsGameOver)
        {
            wasGameOver = false;
            waitingForRelease = false;
            pinchHoldTimer = 0f;
            return;
        }

        if (!wasGameOver)
        {
            wasGameOver = true;

            // Mencegah game langsung restart jika pemain
            // sedang pinch tepat ketika lagu berakhir.
            waitingForRelease = true;
            pinchHoldTimer = 0f;
        }

        leftPinching =
            IsPinching(handTrackingManager.LeftHand);

        rightPinching =
            IsPinching(handTrackingManager.RightHand);

        bool anyPinching =
            leftPinching || rightPinching;

        if (waitingForRelease)
        {
            if (!anyPinching)
                waitingForRelease = false;

            return;
        }

        if (!anyPinching)
        {
            pinchHoldTimer = 0f;
            return;
        }

        pinchHoldTimer += Time.unscaledDeltaTime;

        if (pinchHoldTimer >= holdDuration)
            scoreManager.RestartGame();
    }

    private bool IsPinching(HandData hand)
    {
        if (hand == null ||
            !hand.IsTracked ||
            hand.Landmarks == null ||
            hand.Landmarks.Length < 21)
        {
            return false;
        }

        const int Wrist = 0;
        const int ThumbTip = 4;
        const int IndexTip = 8;
        const int MiddleMcp = 9;

        float palmSize = Vector3.Distance(
            hand.Landmarks[Wrist],
            hand.Landmarks[MiddleMcp]);

        if (palmSize <= 0.0001f)
            return false;

        float pinchDistance = Vector3.Distance(
            hand.Landmarks[ThumbTip],
            hand.Landmarks[IndexTip]);

        float normalizedDistance =
            pinchDistance / palmSize;

        return normalizedDistance <= pinchThreshold;
    }
}
