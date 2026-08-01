using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public Note notePrefab;

    [Header("Spawn Points")]
    public Transform leftASpawn;
    public Transform leftBSpawn;
    public Transform rightASpawn;
    public Transform rightBSpawn;

    [Header("Hit Points")]
    public Transform leftAHit;
    public Transform leftBHit;
    public Transform rightAHit;
    public Transform rightBHit;

    [Header("Spawn Settings")]
    public float spawnInterval = 0.8f;

    [Range(0, 1)]
    public float doubleChance = 0.3f;

    [Range(0, 1)]
    public float crossChance = 0.15f;

    float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0;

            if (Random.value < doubleChance)
                SpawnDouble();
            else
                SpawnSingle();
        }
    }

    void SpawnSingle()
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

    void SpawnDouble()
    {
        bool useA = Random.value < 0.5f;

        LaneType leftLane = useA ? LaneType.LeftA : LaneType.LeftB;
        LaneType rightLane = useA ? LaneType.RightA : LaneType.RightB;

        Spawn(
            leftLane,
            HandType.Left,
            (GestureType)Random.Range(0, 3));

        Spawn(
            rightLane,
            HandType.Right,
            (GestureType)Random.Range(0, 3));
    }

    void Spawn(
        LaneType lane,
        HandType requiredHand,
        GestureType gesture)
    {
        Transform spawn = null;
        Transform hit = null;

        switch (lane)
        {
            case LaneType.LeftA:
                spawn = leftASpawn;
                hit = leftAHit;
                break;

            case LaneType.LeftB:
                spawn = leftBSpawn;
                hit = leftBHit;
                break;

            case LaneType.RightA:
                spawn = rightASpawn;
                hit = rightAHit;
                break;

            case LaneType.RightB:
                spawn = rightBSpawn;
                hit = rightBHit;
                break;
        }

        Note note = Instantiate(
            notePrefab,
            spawn.position,
            spawn.rotation,
            spawn);

        note.Init(hit);

        note.SetData(
            lane,
            requiredHand,
            gesture);
    }
}

public enum HandType
{
    Left,
    Right
}

public enum GestureType
{
    Rock,
    Paper,
    Scissors
}

public enum LaneType
{
    LeftA,
    LeftB,
    RightA,
    RightB
}