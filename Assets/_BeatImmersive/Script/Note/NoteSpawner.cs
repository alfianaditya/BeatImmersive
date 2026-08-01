using UnityEngine;
using UnityEngine.Serialization;

public class NoteSpawner : MonoBehaviour
{
    [Header("Systems")]
    [SerializeField] private HandTrackingManager handTrackingManager;
    [SerializeField] private ScoreManager scoreManager;

    [Header("Prefab")]
    [SerializeField] private Note notePrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform leftASpawn;
    [SerializeField] private Transform leftBSpawn;
    [SerializeField] private Transform rightASpawn;
    [SerializeField] private Transform rightBSpawn;

    [Header("Forward Targets")]
    [FormerlySerializedAs("leftAHit")]
    [SerializeField] private Transform leftAForwardTarget;

    [FormerlySerializedAs("leftBHit")]
    [SerializeField] private Transform leftBForwardTarget;

    [FormerlySerializedAs("rightAHit")]
    [SerializeField] private Transform rightAForwardTarget;

    [FormerlySerializedAs("rightBHit")]
    [SerializeField] private Transform rightBForwardTarget;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 0.8f;

    [Range(0f, 1f)]
    [SerializeField] private float doubleChance = 0.3f;

    [Range(0f, 1f)]
    [SerializeField] private float crossChance = 0.15f;

    private float timer;

    private void Start()
    {
        ValidateReferences();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer < spawnInterval)
            return;

        timer = 0f;

        if (Random.value < doubleChance)
            SpawnDouble();
        else
            SpawnSingle();
    }

    private void SpawnSingle()
    {
        LaneType lane = (LaneType)Random.Range(0, 4);

        HandType requiredHand =
            lane == LaneType.LeftA || lane == LaneType.LeftB
                ? HandType.Left
                : HandType.Right;

        if (Random.value < crossChance)
        {
            requiredHand =
                requiredHand == HandType.Left
                    ? HandType.Right
                    : HandType.Left;
        }

        GestureType gesture =
            (GestureType)Random.Range(0, 3);

        Spawn(lane, requiredHand, gesture);
    }

    private void SpawnDouble()
    {
        bool useA = Random.value < 0.5f;

        LaneType leftLane =
            useA ? LaneType.LeftA : LaneType.LeftB;

        LaneType rightLane =
            useA ? LaneType.RightA : LaneType.RightB;

        Spawn(
            leftLane,
            HandType.Left,
            (GestureType)Random.Range(0, 3));

        Spawn(
            rightLane,
            HandType.Right,
            (GestureType)Random.Range(0, 3));
    }

    private void Spawn(
        LaneType lane,
        HandType requiredHand,
        GestureType gesture)
    {
        GetLaneTransforms(
            lane,
            out Transform spawnPoint,
            out Transform forwardTarget);

        if (notePrefab == null ||
            spawnPoint == null ||
            forwardTarget == null)
        {
            Debug.LogError(
                $"Gagal spawn note pada lane {lane}. Reference belum lengkap.");
            return;
        }

        Note note = Instantiate(
            notePrefab,
            spawnPoint.position,
            spawnPoint.rotation);

        note.Init(
            forwardTarget,
            handTrackingManager,
            scoreManager);

        note.SetData(
            lane,
            requiredHand,
            gesture);
    }

    private void GetLaneTransforms(
        LaneType lane,
        out Transform spawnPoint,
        out Transform forwardTarget)
    {
        spawnPoint = null;
        forwardTarget = null;

        switch (lane)
        {
            case LaneType.LeftA:
                spawnPoint = leftASpawn;
                forwardTarget = leftAForwardTarget;
                break;

            case LaneType.LeftB:
                spawnPoint = leftBSpawn;
                forwardTarget = leftBForwardTarget;
                break;

            case LaneType.RightA:
                spawnPoint = rightASpawn;
                forwardTarget = rightAForwardTarget;
                break;

            case LaneType.RightB:
                spawnPoint = rightBSpawn;
                forwardTarget = rightBForwardTarget;
                break;
        }
    }

    private void ValidateReferences()
    {
        if (handTrackingManager == null)
            Debug.LogError("NoteSpawner: HandTrackingManager belum diisi.");

        if (scoreManager == null)
            Debug.LogError("NoteSpawner: ScoreManager belum diisi.");

        if (notePrefab == null)
            Debug.LogError("NoteSpawner: Note Prefab belum diisi.");
    }
}

public enum HandType
{
    Left,
    Right
}

public enum GestureType
{
    Rock = 0,
    Paper = 1,
    Scissors = 2,
    Unknown = 3
}

public enum LaneType
{
    LeftA,
    LeftB,
    RightA,
    RightB
}
