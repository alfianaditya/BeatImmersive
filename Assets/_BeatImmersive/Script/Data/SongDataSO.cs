using UnityEngine;

[CreateAssetMenu(fileName = "SongData", menuName = "Beat Immersive/Song Data")]
public class SongDataSO : ScriptableObject
{
    [Header("Song")]
    [SerializeField] private string songTitle = "New Song";
    [SerializeField] private AudioClip audioClip;

    [Header("Rhythm")]
    [SerializeField, Min(1f)] private float bpm = 120f;
    [Tooltip("Note muncul setiap berapa beat.")]
    [SerializeField, Min(0.25f)] private float beatsPerSpawn = 2f;

    [Header("Random Note")]
    [SerializeField, Range(0f, 1f)] private float doubleChance = 0.2f;
    [SerializeField, Range(0f, 1f)] private float crossChance = 0.05f;

    public string SongTitle => songTitle;
    public AudioClip AudioClip => audioClip;
    public float BPM => bpm;
    public float BeatsPerSpawn => beatsPerSpawn;
    public float DoubleChance => doubleChance;
    public float CrossChance => crossChance;
    public float Duration => audioClip != null ? audioClip.length : 0f;
    public float SpawnInterval =>
        (60f / Mathf.Max(1f, bpm)) * Mathf.Max(0.25f, beatsPerSpawn);
}
