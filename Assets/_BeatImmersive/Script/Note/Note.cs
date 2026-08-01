using UnityEngine;

public class Note : MonoBehaviour
{
    [Header("Renderer")]
    public SpriteRenderer background;
    public SpriteRenderer icon;

    [Header("Gesture Sprite")]
    public Sprite rockSprite;
    public Sprite paperSprite;
    public Sprite scissorsSprite;

    [Header("Background Color")]
    public Color leftColor = Color.red;
    public Color rightColor = Color.blue;

    [Header("Movement")]
    public float speed = 6f;

    private Transform target;

    public LaneType Lane { get; private set; }

    public HandType RequiredHand { get; private set; }

    public GestureType RequiredGesture { get; private set; }

    public bool CanHit { get; set; }

    public bool IsHit { get; set; }

    public void Init(Transform hitTarget)
    {
        target = hitTarget;
    }

    private void Update()
    {
        if (target == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime);
    }

    public void SetData(
        LaneType lane,
        HandType hand,
        GestureType gesture)
    {
        Lane = lane;
        RequiredHand = hand;
        RequiredGesture = gesture;

        background.color =
            hand == HandType.Left
            ? leftColor
            : rightColor;

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