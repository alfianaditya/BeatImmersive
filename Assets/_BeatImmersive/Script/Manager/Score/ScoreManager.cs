using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text missText;

    public int Score { get; private set; }
    public int MissCount { get; private set; }

    private void Start()
    {
        RefreshUI();
    }

    public void RegisterHit(int value)
    {
        Score += Mathf.Max(0, value);
        RefreshUI();
    }

    public void RegisterMiss()
    {
        MissCount++;
        RefreshUI();
    }

    public void ResetScore()
    {
        Score = 0;
        MissCount = 0;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {Score}";

        if (missText != null)
            missText.text = $"Miss: {MissCount}";
    }
}
