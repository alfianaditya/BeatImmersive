using System;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using UnityEngine;

public class HandTrackingManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField]
    private HandLandmarkerRunner runner;

    [Header("Gesture")]
    [SerializeField, Min(1)]
    private int requiredStableFrames = 4;

    [Header("Handedness")]
    [Tooltip("Aktifkan hanya jika label Left dan Right dari kamera terbaca terbalik.")]
    [SerializeField]
    private bool invertHandedness;

    [Header("Debug")]
    [SerializeField]
    private bool showGestureLog;

    public HandData LeftHand = new HandData();
    public HandData RightHand = new HandData();

    private HandLandmarkerResult snapshot;

    private GestureType previousLeftGesture = GestureType.Unknown;
    private GestureType previousRightGesture = GestureType.Unknown;

    private void Update()
    {
        LeftHand.BeginFrame();
        RightHand.BeginFrame();

        if (runner == null)
        {
            LeftHand.MarkLost();
            RightHand.MarkLost();
            return;
        }

        try
        {
            HandLandmarkerResult result = runner.CurrentResult;

            if (result.handLandmarks == null ||
                result.handedness == null ||
                result.handLandmarks.Count == 0)
            {
                LeftHand.MarkLost();
                RightHand.MarkLost();
                return;
            }

            // Menyalin result agar data aman digunakan pada frame Unity.
            result.CloneTo(ref snapshot);

            int handCount = Mathf.Min(
                snapshot.handLandmarks.Count,
                snapshot.handedness.Count);

            for (int handIndex = 0; handIndex < handCount; handIndex++)
            {
                var landmarkGroup = snapshot.handLandmarks[handIndex];
                var classifications = snapshot.handedness[handIndex];

                if (landmarkGroup.landmarks == null ||
                    landmarkGroup.landmarks.Count < 21 ||
                    classifications.categories == null ||
                    classifications.categories.Count == 0)
                {
                    continue;
                }

                string handednessLabel =
                    classifications.categories[0].categoryName;

                bool isLeft = string.Equals(
                    handednessLabel,
                    "Left",
                    StringComparison.OrdinalIgnoreCase);

                if (invertHandedness)
                    isLeft = !isLeft;

                HandData targetHand = isLeft ? LeftHand : RightHand;
                targetHand.SetTracked(isLeft);

                for (int landmarkIndex = 0;
                     landmarkIndex < 21;
                     landmarkIndex++)
                {
                    var point =
                        landmarkGroup.landmarks[landmarkIndex];

                    targetHand.Landmarks[landmarkIndex] =
                        new Vector3(point.x, point.y, point.z);
                }

                GestureType detectedGesture =
                    GestureDetector.Detect(targetHand.Landmarks);

                targetHand.UpdateGesture(
                    detectedGesture,
                    requiredStableFrames);
            }

            if (!LeftHand.IsTracked)
                LeftHand.MarkLost();

            if (!RightHand.IsTracked)
                RightHand.MarkLost();

            LogGestureChanges();
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"HandTrackingManager error: {exception.Message}");

            LeftHand.MarkLost();
            RightHand.MarkLost();
        }
    }

    private void LogGestureChanges()
    {
        if (!showGestureLog)
            return;

        if (LeftHand.Gesture != previousLeftGesture)
        {
            previousLeftGesture = LeftHand.Gesture;

            Debug.LogError(
                $"LEFT HAND GESTURE: {LeftHand.Gesture}");
        }

        if (RightHand.Gesture != previousRightGesture)
        {
            previousRightGesture = RightHand.Gesture;

            Debug.LogError(
                $"RIGHT HAND GESTURE: {RightHand.Gesture}");
        }
    }
}
