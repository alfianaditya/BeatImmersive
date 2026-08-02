using System;
using UnityEngine;

[Serializable]
public class HandData
{
    public bool IsTracked;
    public bool IsLeft;

    public Vector3[] Landmarks = new Vector3[21];

    public GestureType RawGesture = GestureType.Unknown;
    public GestureType Gesture = GestureType.Unknown;

    [NonSerialized]
    private GestureType candidateGesture = GestureType.Unknown;

    [NonSerialized]
    private int candidateFrames;

    public void BeginFrame()
    {
        IsTracked = false;
    }

    public void SetTracked(bool isLeft)
    {
        IsTracked = true;
        IsLeft = isLeft;
    }

    public void UpdateGesture(
        GestureType detectedGesture,
        int requiredStableFrames)
    {
        RawGesture = detectedGesture;

        if (detectedGesture == GestureType.Unknown)
        {
            candidateGesture = GestureType.Unknown;
            candidateFrames = 0;
            Gesture = GestureType.Unknown;
            return;
        }

        if (candidateGesture != detectedGesture)
        {
            candidateGesture = detectedGesture;
            candidateFrames = 1;
            return;
        }

        candidateFrames++;

        if (candidateFrames >= Mathf.Max(1, requiredStableFrames))
            Gesture = candidateGesture;
    }

    public void MarkLost()
    {
        IsTracked = false;
        RawGesture = GestureType.Unknown;
        Gesture = GestureType.Unknown;
        candidateGesture = GestureType.Unknown;
        candidateFrames = 0;
    }
}
