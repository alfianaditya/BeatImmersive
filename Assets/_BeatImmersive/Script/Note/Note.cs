using UnityEngine;

public class Note : MonoBehaviour
{
    [Header("Renderer")]
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private SpriteRenderer icon;

    [Header("Gesture Sprite")]
    [SerializeField] private Sprite rockSprite;
    [SerializeField] private Sprite paperSprite;
    [SerializeField] private Sprite scissorsSprite;

    [Header("Background Color")]
    [SerializeField] private Color leftColor = Color.red;
    [SerializeField] private Color rightColor = Color.blue;

    [Header("Movement")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float forwardTargetDistance = 0.05f;

    [Header("Score")]
    [SerializeField] private int scoreValue = 100;

    [Header("Debug")]
    [SerializeField] private bool showHitLog = true;

    private Transform forwardTarget;
    private HandTrackingManager handTrackingManager;
    private ScoreManager scoreManager;

    public LaneType Lane { get; private set; }
    public HandType RequiredHand { get; private set; }
    public GestureType RequiredGesture { get; private set; }

    [Header("Runtime Debug")]
    [SerializeField] private bool isResolved;

    [field: SerializeField]
    public bool CanHit { get; private set; }

    [field: SerializeField]
    public bool IsHit { get; private set; }

    public void Init(
        Transform targetForward,
        HandTrackingManager trackingManager,
        ScoreManager gameScoreManager)
    {
        forwardTarget = targetForward;
        handTrackingManager = trackingManager;
        scoreManager = gameScoreManager;

        CanHit = false;
        IsHit = false;
        isResolved = false;
    }

    private void Update()
    {
        if (isResolved || forwardTarget == null)
            return;

        MoveForward();

        // Gesture tidak diperiksa di sini.
        // Gesture hanya diperiksa oleh HitArea.OnTriggerStay2D().

        CheckForwardTarget();
    }

    private void MoveForward()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            forwardTarget.position,
            speed * Time.deltaTime);
    }

    private void CheckForwardTarget()
    {
        float distanceToTarget = Vector3.Distance(
            transform.position,
            forwardTarget.position);

        if (distanceToTarget <= forwardTargetDistance)
            Miss();
    }

    public void EnterHitArea(LaneType areaLane)
    {
        if (isResolved)
            return;

        if (areaLane != Lane)
            return;

        CanHit = true;
    }

    public void ExitHitArea(LaneType areaLane)
    {
        if (areaLane != Lane)
            return;

        CanHit = false;
    }

    public void TryHitInsideArea(LaneType areaLane)
    {
        if (isResolved)
            return;

        // Pengaman: harus berasal dari lane area yang benar.
        if (areaLane != Lane)
            return;

        // Pengaman: note harus sudah masuk Hit Area.
        if (!CanHit)
            return;

        if (handTrackingManager == null)
            return;

        HandData selectedHand =
            RequiredHand == HandType.Left
                ? handTrackingManager.LeftHand
                : handTrackingManager.RightHand;

        if (selectedHand == null)
            return;

        if (!selectedHand.IsTracked)
            return;

        if (selectedHand.Gesture != RequiredGesture)
            return;

        Hit();
    }

    private void Hit()
    {
        if (isResolved)
            return;

        if (!CanHit)
            return;

        isResolved = true;
        IsHit = true;
        CanHit = false;

        if (scoreManager != null)
            scoreManager.RegisterHit(scoreValue);

        if (showHitLog)
        {
            // Debug.LogError(
            //     $"HIT | Lane: {Lane} | " +
            //     $"Hand: {RequiredHand} | " +
            //     $"Gesture: {RequiredGesture}");
        }

        Destroy(gameObject);
    }

    private void Miss()
    {
        if (isResolved)
            return;

        isResolved = true;
        IsHit = false;
        CanHit = false;

        if (scoreManager != null)
            scoreManager.RegisterMiss();

        if (showHitLog)
        {
            // Debug.LogError(
            //     $"MISS | Lane: {Lane} | " +
            //     $"Hand: {RequiredHand} | " +
            //     $"Gesture: {RequiredGesture}");
        }

        Destroy(gameObject);
    }

    public void SetData(
        LaneType lane,
        HandType hand,
        GestureType gesture)
    {
        Lane = lane;
        RequiredHand = hand;
        RequiredGesture = gesture;

        if (background != null)
        {
            background.color =
                hand == HandType.Left
                    ? leftColor
                    : rightColor;
        }

        if (icon == null)
            return;

        switch (gesture)
        {
            case GestureType.Rock:
                icon.sprite = rockSprite;
                break;

            case GestureType.Paper:
                icon.sprite = paperSprite;
                break;

            case GestureType.Scissors:
                icon.sprite = scissorsSprite;
                break;
        }
    }
}