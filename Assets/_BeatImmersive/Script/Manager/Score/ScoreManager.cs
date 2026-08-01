using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [Header("Systems")]
    [SerializeField] private RhythmMusicManager musicManager;
    [SerializeField] private BeatmapSpawner beatmapSpawner;

    [Header("Gameplay UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text missText;

    [Header("Game Over")]
    [SerializeField] private GameObject congratPanel;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text finalMissText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button restartButton;

    [Header("Result Messages")]
    [TextArea]
    [SerializeField]
    private string winMessage =
        "Selamat! Permainanmu sangat bagus!";

    [TextArea]
    [SerializeField]
    private string encouragementMessage =
        "Tetap semangat! Coba lagi dan kalahkan skor sebelumnya!";

    public int Score { get; private set; }
    public int HitCount { get; private set; }
    public int MissCount { get; private set; }
    public bool IsGameOver { get; private set; }

    private void Start()
    {
        Time.timeScale = 1f;

        Score = 0;
        HitCount = 0;
        MissCount = 0;
        IsGameOver = false;

        if (congratPanel != null)
            congratPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        RefreshGameplayUI();

        if (musicManager != null)
            RefreshTimer(musicManager.SongLengthSeconds);
    }

    private void Update()
    {
        if (IsGameOver || musicManager == null)
            return;

        RefreshTimer(musicManager.RemainingTimeSeconds);

        if (musicManager.HasSongEnded)
            EndGame();
    }

    public void RegisterHit(int value)
    {
        if (IsGameOver)
            return;

        Score += Mathf.Max(0, value);
        HitCount++;

        RefreshGameplayUI();
    }

    public void RegisterMiss()
    {
        if (IsGameOver)
            return;

        MissCount++;

        RefreshGameplayUI();
    }

    public void EndGame()
    {
        if (IsGameOver)
            return;

        IsGameOver = true;

        // Jangan memakai Time.timeScale = 0.
        // MediaPipe dan pinch restart harus tetap berjalan.
        if (beatmapSpawner != null)
            beatmapSpawner.enabled = false;

        DestroyRemainingNotes();

        if (musicManager != null)
            musicManager.StopSong();

        RefreshTimer(0f);
        ShowResult();
    }

    private void ShowResult()
    {
        if (finalScoreText != null)
            finalScoreText.text = $"Score: {Score}";

        if (finalMissText != null)
            finalMissText.text = $"Miss: {MissCount}";

        if (resultText != null)
        {
            // Perbandingan memakai jumlah HIT dan MISS,
            // bukan poin score yang nilainya 100 per hit.
            resultText.text =
                HitCount > MissCount
                    ? winMessage
                    : encouragementMessage;
        }

        if (congratPanel != null)
            congratPanel.SetActive(true);
    }

    private void DestroyRemainingNotes()
    {
        Note[] remainingNotes =
            FindObjectsByType<Note>(
                FindObjectsSortMode.None);

        foreach (Note note in remainingNotes)
        {
            if (note != null)
                Destroy(note.gameObject);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        Scene currentScene =
            SceneManager.GetActiveScene();

        SceneManager.LoadScene(
            currentScene.buildIndex);
    }

    private void RefreshGameplayUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {Score}";

        if (missText != null)
            missText.text = $"Miss: {MissCount}";
    }

    private void RefreshTimer(float remainingSeconds)
    {
        if (timerText == null)
            return;

        int totalSeconds =
            Mathf.CeilToInt(
                Mathf.Max(0f, remainingSeconds));

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text =
            $"{minutes:00}:{seconds:00}";
    }

    private void OnDestroy()
    {
        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartGame);
    }
}
