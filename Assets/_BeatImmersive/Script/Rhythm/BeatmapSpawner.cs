using System;
using System.Collections.Generic;
using UnityEngine;

public class BeatmapSpawner : MonoBehaviour
{
    [Header("Systems")]
    [SerializeField] private RhythmMusicManager musicManager;
    [SerializeField] private HandTrackingManager handTrackingManager;
    [SerializeField] private ScoreManager scoreManager;

    [Header("Prefab")]
    [SerializeField] private Note notePrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform leftASpawn;
    [SerializeField] private Transform leftBSpawn;
    [SerializeField] private Transform rightASpawn;
    [SerializeField] private Transform rightBSpawn;

    [Header("Beat Points")]
    [Tooltip("Titik tengah Hit Area. Note harus sampai di sini tepat pada beat.")]
    [SerializeField] private Transform leftABeatPoint;
    [SerializeField] private Transform leftBBeatPoint;
    [SerializeField] private Transform rightABeatPoint;
    [SerializeField] private Transform rightBBeatPoint;

    [Header("Forward Targets")]
    [Tooltip("Titik akhir setelah Hit Area. Note yang sampai sini dianggap miss.")]
    [SerializeField] private Transform leftAForwardTarget;
    [SerializeField] private Transform leftBForwardTarget;
    [SerializeField] private Transform rightAForwardTarget;
    [SerializeField] private Transform rightBForwardTarget;

    [Header("Timing")]
    [Tooltip("Note muncul berapa beat sebelum beat target.")]
    [SerializeField, Min(0.25f)] private float noteTravelBeats = 4f;

    [Header("Beatmap")]
    [SerializeField] private List<BeatNoteData> beatmap = new();

    [Header("Debug")]
    [SerializeField] private bool showSpawnLog;

    private int nextNoteIndex;

    private void Start()
    {
        beatmap.Sort(
            (a, b) => a.beatNumber.CompareTo(b.beatNumber));

        ValidateReferences();
        ValidatePreRoll();
    }

    private void Update()
    {
        if (musicManager == null ||
            !musicManager.IsScheduled)
        {
            return;
        }

        double currentBeat =
            musicManager.SongPositionBeats;

        while (nextNoteIndex < beatmap.Count)
        {
            BeatNoteData noteData =
                beatmap[nextNoteIndex];

            double targetBeat =
                noteData.beatNumber - 1f;

            double spawnBeat =
                targetBeat - noteTravelBeats;

            if (currentBeat < spawnBeat)
                break;

            SpawnNote(noteData);
            nextNoteIndex++;
        }
    }

    private void SpawnNote(BeatNoteData data)
    {
        GetLaneTransforms(
            data.lane,
            out Transform spawnPoint,
            out Transform beatPoint,
            out Transform forwardTarget);

        if (notePrefab == null ||
            spawnPoint == null ||
            beatPoint == null ||
            forwardTarget == null)
        {
            Debug.LogError(
                $"BeatmapSpawner: reference lane {data.lane} belum lengkap.");
            return;
        }

        float travelSeconds =
            noteTravelBeats * musicManager.SecondsPerBeat;

        float distanceToBeat =
            Vector3.Distance(
                spawnPoint.position,
                beatPoint.position);

        float calculatedSpeed =
            distanceToBeat / Mathf.Max(0.01f, travelSeconds);

        Note note = Instantiate(
            notePrefab,
            spawnPoint.position,
            spawnPoint.rotation);

        note.Init(
            forwardTarget,
            handTrackingManager,
            scoreManager);

        note.SetData(
            data.lane,
            data.requiredHand,
            data.gesture);

        note.SetMoveSpeed(calculatedSpeed);

        if (showSpawnLog)
        {
            Debug.LogError(
                $"SPAWN | Beat: {data.beatNumber} | " +
                $"Lane: {data.lane} | " +
                $"Hand: {data.requiredHand} | " +
                $"Gesture: {data.gesture}");
        }
    }

    private void GetLaneTransforms(
        LaneType lane,
        out Transform spawnPoint,
        out Transform beatPoint,
        out Transform forwardTarget)
    {
        spawnPoint = null;
        beatPoint = null;
        forwardTarget = null;

        switch (lane)
        {
            case LaneType.LeftA:
                spawnPoint = leftASpawn;
                beatPoint = leftABeatPoint;
                forwardTarget = leftAForwardTarget;
                break;

            case LaneType.LeftB:
                spawnPoint = leftBSpawn;
                beatPoint = leftBBeatPoint;
                forwardTarget = leftBForwardTarget;
                break;

            case LaneType.RightA:
                spawnPoint = rightASpawn;
                beatPoint = rightABeatPoint;
                forwardTarget = rightAForwardTarget;
                break;

            case LaneType.RightB:
                spawnPoint = rightBSpawn;
                beatPoint = rightBBeatPoint;
                forwardTarget = rightBForwardTarget;
                break;
        }
    }

    private void ValidatePreRoll()
    {
        if (musicManager == null)
            return;

        float travelSeconds =
            noteTravelBeats * musicManager.SecondsPerBeat;

        float requiredDelay =
            Mathf.Max(
                0f,
                travelSeconds -
                musicManager.FirstBeatOffset);

        if (musicManager.StartDelay < requiredDelay)
        {
            Debug.LogError(
                $"Start Delay terlalu pendek. " +
                $"Gunakan minimal sekitar {requiredDelay:F2} detik " +
                $"agar note beat pertama dapat muncul tepat waktu.");
        }
    }

    private void ValidateReferences()
    {
        if (musicManager == null)
            Debug.LogError("BeatmapSpawner: RhythmMusicManager belum diisi.");

        if (handTrackingManager == null)
            Debug.LogError("BeatmapSpawner: HandTrackingManager belum diisi.");

        if (scoreManager == null)
            Debug.LogError("BeatmapSpawner: ScoreManager belum diisi.");

        if (notePrefab == null)
            Debug.LogError("BeatmapSpawner: Note Prefab belum diisi.");
    }
}

[Serializable]
public class BeatNoteData
{
    [Tooltip("Beat 1 berarti beat pertama lagu.")]
    [Min(1f)]
    public float beatNumber = 1f;

    public LaneType lane;
    public HandType requiredHand;
    public GestureType gesture;
}
