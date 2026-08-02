using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RhythmMusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public SongDataSO CurrentSong { get; private set; }
    public float CurrentBPM { get; private set; } = 120f;
    public bool IsPlaying => audioSource != null && audioSource.isPlaying;

    // Kompatibilitas dengan BeatmapSpawner lama.
    public bool IsScheduled { get; private set; }
    public float SecondsPerBeat => 60f / Mathf.Max(1f, CurrentBPM);
    public double SongPositionBeats =>
        audioSource != null ? audioSource.time / SecondsPerBeat : 0d;
    public float FirstBeatOffset => 0f;
    public float StartDelay => 0f;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }
    }

    public bool PlaySong(SongDataSO song)
    {
        if (audioSource == null)
        {
            Debug.LogError(
                "RhythmMusicManager: AudioSource belum diisi.");

            return false;
        }

        if (song == null || song.AudioClip == null)
        {
            Debug.LogError(
                "RhythmMusicManager: Song atau AudioClip kosong.");

            return false;
        }

        StopSong();

        CurrentSong = song;
        CurrentBPM = song.BPM;

        audioSource.clip = song.AudioClip;
        audioSource.Play();

        IsScheduled = true;

        return true;
    }

    public void StopSong()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        IsScheduled = false;
    }
}
