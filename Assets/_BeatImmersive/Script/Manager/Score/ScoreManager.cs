using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text missText;

    [Header("3D Combo")]
    [SerializeField] private ComboEffect comboEffect;

    [Header("Score Settings")]
    [SerializeField, Min(0)] private int defaultHitScore = 100;

    public int Score { get; private set; }
    public int HitCount { get; private set; }
    public int MissCount { get; private set; }
    public int Combo { get; private set; }
    public int MaxCombo { get; private set; }

    private void Start()
    {
        ResetScore();
    }

    public void RegisterHit(int value)
    {
        int addedScore =
            value > 0
                ? value
                : defaultHitScore;

        Score += addedScore;
        HitCount++;
        Combo++;

        if (Combo > MaxCombo)
            MaxCombo = Combo;

        if (comboEffect != null)
            comboEffect.ShowCombo(Combo);

        RefreshUI();
    }

    public void RegisterMiss()
    {
        MissCount++;
        Combo = 0;

        if (comboEffect != null)
            comboEffect.ResetCombo();

        RefreshUI();
    }

    public void ResetScore()
    {
        Score = 0;
        HitCount = 0;
        MissCount = 0;
        Combo = 0;
        MaxCombo = 0;

        if (comboEffect != null)
            comboEffect.ResetCombo();

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
