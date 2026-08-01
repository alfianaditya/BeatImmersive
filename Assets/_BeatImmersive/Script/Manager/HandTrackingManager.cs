using UnityEngine;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;

public class HandTrackingManager : MonoBehaviour
{
    [SerializeField]
    private HandLandmarkerRunner runner;

    void Update()
    {
        if (runner == null)
            return;

        var result = runner.CurrentResult;

        if (result.handLandmarks == null)
            return;

        if (result.handLandmarks.Count == 0)
            return;

        Debug.Log($"Detected : {result.handLandmarks.Count}");
    }
}