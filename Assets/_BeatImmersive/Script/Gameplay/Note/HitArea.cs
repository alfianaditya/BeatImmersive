using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HitArea : MonoBehaviour
{
    [Header("Lane")]
    [SerializeField] private LaneType lane;

    [Header("Debug")]
    [SerializeField] private bool showAreaLog = true;

    public LaneType Lane => lane;

    private Collider areaCollider;

    private void Reset()
    {
        areaCollider = GetComponent<Collider>();
        areaCollider.isTrigger = true;
    }

    private void Awake()
    {
        areaCollider = GetComponent<Collider>();
        areaCollider.isTrigger = true;

        if (!CompareTag("HitArea"))
        {
            Debug.LogError(
                $"{name}: GameObject harus menggunakan tag HitArea.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Note note = other.GetComponentInParent<Note>();

        if (note == null)
            return;

        if (note.Lane != lane)
            return;

        note.EnterHitArea(lane);

        if (showAreaLog)
        {
            Debug.LogError(
                $"ENTER HIT AREA | Note: {note.name} | Lane: {lane}");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Note note = other.GetComponentInParent<Note>();

        if (note == null)
            return;

        if (note.Lane != lane)
            return;

        note.TryHitInsideArea(lane);
    }

    private void OnTriggerExit(Collider other)
    {
        Note note = other.GetComponentInParent<Note>();

        if (note == null)
            return;

        if (note.Lane != lane)
            return;

        note.ExitHitArea(lane);

        if (showAreaLog)
        {
            Debug.LogError(
                $"EXIT HIT AREA | Note: {note.name} | Lane: {lane}");
        }
    }
}