using UnityEngine;

public class BeatManager : MonoBehaviour
{
    public AudioSource music;

    public float bpm = 120;

    public float BeatInterval => 60f / bpm;

    public float SongTime => music.time;

    public void PlaySong()
    {
        music.Play();
    }
}