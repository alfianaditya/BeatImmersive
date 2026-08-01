using System;
using UnityEngine;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(AudioSource))]
public class RhythmMusicManager : MonoBehaviour
{
    [Header("Song")]
    [SerializeField] private AudioClip song;
    [SerializeField, Min(1f)] private float bpm = 156f;

    [Tooltip("Waktu dari awal audio sampai beat pertama, dalam detik.")]
    [SerializeField, Min(0f)] private float firstBeatOffset;

    [Tooltip("Waktu persiapan sebelum audio mulai.")]
    [SerializeField, Min(0.1f)] private float startDelay = 3f;

    [Header("Playback")]
    [SerializeField] private bool playOnStart = true;

    private AudioSource audioSource;

    public bool IsScheduled { get; private set; }
    public double SongStartDspTime { get; private set; }

    public float BPM => bpm;
    public float FirstBeatOffset => firstBeatOffset;
    public float StartDelay => startDelay;
    public float SecondsPerBeat => 60f / bpm;

    public float SongLengthSeconds =>
        song != null ? song.length : 0f;

    public double SongPositionSeconds
    {
        get
        {
            if (!IsScheduled)
                return -startDelay;

            return AudioSettings.dspTime - SongStartDspTime;
        }
    }

    public double SongPositionBeats =>
        (SongPositionSeconds - firstBeatOffset) / SecondsPerBeat;

    public float RemainingTimeSeconds
    {
        get
        {
            double playedSeconds =
                Math.Max(0d, SongPositionSeconds);

            return Mathf.Max(
                0f,
                SongLengthSeconds - (float)playedSeconds);
        }
    }

    public bool HasSongEnded =>
        IsScheduled &&
        SongPositionSeconds >= SongLengthSeconds;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        if (playOnStart)
            StartSong();
    }

    public void StartSong()
    {
        if (song == null)
        {
            Debug.LogError(
                "RhythmMusicManager: Song belum diisi.");
            return;
        }

        audioSource.Stop();
        audioSource.clip = song;

        SongStartDspTime =
            AudioSettings.dspTime + startDelay;

        audioSource.PlayScheduled(SongStartDspTime);
        IsScheduled = true;
    }

    public void StopSong()
    {
        audioSource.Stop();
        IsScheduled = false;
    }
}
