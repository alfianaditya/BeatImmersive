using TMPro;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    private enum GameState
    {
        Menu,
        Countdown,
        Playing,
        Clear
    }

    [Header("Systems")]
    [SerializeField] private RhythmMusicManager musicManager;
    [SerializeField] private NoteSpawner noteSpawner;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private CountdownText3D countdownText;

    [Header("Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject clearPanel;

    [Header("Gameplay Panel Text")]
    [SerializeField] private TMP_Text songTitleText;
    [SerializeField] private TMP_Text remainingTimeText;

    [Header("Clear Panel - 3 Text Components")]
    [SerializeField] private TMP_Text resultTitleText;
    [SerializeField] private TMP_Text resultScoreMissText;
    [SerializeField] private TMP_Text resultMaxComboText;

    private SongDataSO selectedSong;
    private GameState state;
    private float songEndRealtime;
    private bool isEndingGame;

    public SongDataSO SelectedSong => selectedSong;

    private void Start()
    {
        ShowSongMenu();
    }

    private void Update()
    {
        if (state != GameState.Playing ||
            selectedSong == null)
        {
            return;
        }

        float remaining =
            Mathf.Max(
                0f,
                songEndRealtime - Time.unscaledTime);

        UpdateRemainingTime(remaining);

        if (remaining <= 0f && !isEndingGame)
            FinishGame();
    }

    public void SelectAndStartSong(SongDataSO song)
    {
        if (song == null || song.AudioClip == null)
        {
            Debug.LogError(
                "GameFlowManager: Song atau AudioClip kosong.");
            return;
        }

        selectedSong = song;
        StartSelectedSong();
    }

    public void RestartCurrentSong()
    {
        if (selectedSong == null)
        {
            ShowSongMenu();
            return;
        }

        StartSelectedSong();
    }

    public void ReturnToSongMenu()
    {
        StopCurrentGame();
        selectedSong = null;
        ShowSongMenu();
    }

    private void StartSelectedSong()
    {
        StopCurrentGame();

        state = GameState.Countdown;
        isEndingGame = false;

        SetPanel(menuPanel, false);
        SetPanel(gameplayPanel, false);
        SetPanel(clearPanel, false);

        if (scoreManager != null)
            scoreManager.ResetScore();

        if (songTitleText != null)
            songTitleText.text = selectedSong.SongTitle;

        UpdateRemainingTime(selectedSong.Duration);

        if (noteSpawner != null)
            noteSpawner.ConfigureFromSong(selectedSong);

        if (countdownText == null)
        {
            Debug.LogError(
                "GameFlowManager: CountdownText3D belum diisi.");

            BeginGameplayOnGo();
            return;
        }

        countdownText.PlayCountdown(BeginGameplayOnGo);
    }

    private void BeginGameplayOnGo()
    {
        if (state != GameState.Countdown ||
            selectedSong == null)
        {
            return;
        }

        SetPanel(gameplayPanel, true);

        bool musicStarted =
            musicManager != null &&
            musicManager.PlaySong(selectedSong);

        if (!musicStarted)
        {
            Debug.LogError(
                "GameFlowManager: Musik gagal dimainkan.");

            FinishGame();
            return;
        }

        songEndRealtime =
            Time.unscaledTime + selectedSong.Duration;

        if (noteSpawner != null)
            noteSpawner.StartSpawning();

        state = GameState.Playing;
    }

    private void FinishGame()
    {
        if (isEndingGame)
            return;

        isEndingGame = true;

        if (musicManager != null)
            musicManager.StopSong();

        if (noteSpawner != null)
        {
            noteSpawner.StopSpawning();
            noteSpawner.ClearAllNotes();
        }

        SetPanel(gameplayPanel, false);
        SetPanel(menuPanel, false);
        SetPanel(clearPanel, true);

        ShowClearResult();

        state = GameState.Clear;
        isEndingGame = false;
    }

    private void ShowClearResult()
    {
        int score =
            scoreManager != null
                ? scoreManager.Score
                : 0;

        int miss =
            scoreManager != null
                ? scoreManager.MissCount
                : 0;

        int maxCombo =
            scoreManager != null
                ? scoreManager.MaxCombo
                : 0;

        bool isWin = score > miss;

        if (resultTitleText != null)
        {
            resultTitleText.text =
                isWin
                    ? "CONGRATULATION!"
                    : "GAME OVER";
        }

        if (resultScoreMissText != null)
        {
            resultScoreMissText.text =
                $"Score : {score}\nMiss : {miss}";
        }

        if (resultMaxComboText != null)
        {
            resultMaxComboText.text =
                $"Max Combo : {maxCombo}";
        }
    }

    private void ShowSongMenu()
    {
        StopCurrentGame();

        state = GameState.Menu;
        isEndingGame = false;

        SetPanel(menuPanel, true);
        SetPanel(gameplayPanel, false);
        SetPanel(clearPanel, false);
    }

    private void StopCurrentGame()
    {
        if (countdownText != null)
            countdownText.CancelCountdown();

        if (musicManager != null)
            musicManager.StopSong();

        if (noteSpawner != null)
        {
            noteSpawner.StopSpawning();
            noteSpawner.ClearAllNotes();
        }
    }

    private void UpdateRemainingTime(float seconds)
    {
        if (remainingTimeText == null)
            return;

        int totalSeconds =
            Mathf.CeilToInt(Mathf.Max(0f, seconds));

        int minutes = totalSeconds / 60;
        int remainder = totalSeconds % 60;

        remainingTimeText.text =
            $"{minutes:0}:{remainder:00}";
    }

    private static void SetPanel(
        GameObject panel,
        bool active)
    {
        if (panel != null)
            panel.SetActive(active);
    }
}
