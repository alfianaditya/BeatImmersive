using UnityEngine;

public class PinchRaycaster3D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HandTrackingManager handTrackingManager;
    [SerializeField] private Camera interactionCamera;

    [Header("Pinch")]
    [SerializeField, Range(0.005f, 0.2f)]
    private float pinchThreshold = 0.045f;

    [SerializeField, Range(0.005f, 0.3f)]
    private float releaseThreshold = 0.07f;

    [Header("Coordinate")]
    [SerializeField] private bool mirrorX = true;
    [SerializeField] private bool invertY = true;

    [Header("Raycast")]
    [SerializeField] private LayerMask buttonLayerMask = ~0;
    [SerializeField, Min(0.1f)] private float maxDistance = 100f;

    private bool leftPinchLatched;
    private bool rightPinchLatched;

    private void Awake()
    {
        if (interactionCamera == null)
            interactionCamera = Camera.main;
    }

    private void Update()
    {
        if (handTrackingManager == null ||
            interactionCamera == null)
        {
            return;
        }

        ProcessHand(
            handTrackingManager.LeftHand,
            ref leftPinchLatched);

        ProcessHand(
            handTrackingManager.RightHand,
            ref rightPinchLatched);
    }

    private void ProcessHand(
        HandData hand,
        ref bool pinchLatched)
    {
        if (hand == null ||
            !hand.IsTracked ||
            hand.Landmarks == null ||
            hand.Landmarks.Length <= 8)
        {
            pinchLatched = false;
            return;
        }

        Vector3 thumbTip = hand.Landmarks[4];
        Vector3 indexTip = hand.Landmarks[8];

        float distance = Vector2.Distance(
            new Vector2(thumbTip.x, thumbTip.y),
            new Vector2(indexTip.x, indexTip.y));

        if (!pinchLatched &&
            distance <= pinchThreshold)
        {
            pinchLatched = true;

            Vector2 midpoint =
                new Vector2(
                    (thumbTip.x + indexTip.x) * 0.5f,
                    (thumbTip.y + indexTip.y) * 0.5f);

            TryPressButton(midpoint);
        }
        else if (pinchLatched &&
                 distance >= releaseThreshold)
        {
            pinchLatched = false;
        }
    }

    private void TryPressButton(Vector2 normalizedPoint)
    {
        float x =
            mirrorX
                ? 1f - normalizedPoint.x
                : normalizedPoint.x;

        float y =
            invertY
                ? 1f - normalizedPoint.y
                : normalizedPoint.y;

        x = Mathf.Clamp01(x);
        y = Mathf.Clamp01(y);

        Rect cameraRect =
            interactionCamera.pixelRect;

        Vector3 screenPoint =
            new Vector3(
                cameraRect.x + x * cameraRect.width,
                cameraRect.y + y * cameraRect.height,
                0f);

        Ray ray =
            interactionCamera.ScreenPointToRay(
                screenPoint);

        Debug.DrawRay(
            ray.origin,
            ray.direction * maxDistance,
            Color.red,
            0.5f);

        RaycastHit[] hits =
            Physics.RaycastAll(
                ray,
                maxDistance,
                buttonLayerMask,
                QueryTriggerInteraction.Collide);

        foreach (RaycastHit hit in hits)
        {
            PinchButton3D button =
                hit.collider.GetComponentInParent<
                    PinchButton3D>();

            if (button == null)
                continue;

            button.Press();
            return;
        }
    }

    private void OnValidate()
    {
        if (releaseThreshold <= pinchThreshold)
            releaseThreshold = pinchThreshold + 0.01f;
    }
}
