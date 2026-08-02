using TMPro;
using UnityEngine;

public class SongButtonAction : MonoBehaviour
{
    [SerializeField] private GameFlowManager gameFlowManager;
    [SerializeField] private SongDataSO songData;

    [Header("Optional Label")]
    [SerializeField] private TMP_Text songLabel;
    [SerializeField] private bool showBpm = true;

    private void OnEnable()
    {
        RefreshLabel();
    }

    public void ChooseSong()
    {
        if (gameFlowManager == null)
        {
            Debug.LogError(
                "SongButtonAction: GameFlowManager belum diisi.");
            return;
        }

        if (songData == null)
        {
            Debug.LogError(
                "SongButtonAction: SongData belum diisi.");
            return;
        }

        gameFlowManager.SelectAndStartSong(songData);
    }

    private void RefreshLabel()
    {
        if (songLabel == null || songData == null)
            return;

        songLabel.text =
            showBpm
                ? $"{songData.SongTitle}\n{songData.BPM:0} BPM"
                : songData.SongTitle;
    }
}
