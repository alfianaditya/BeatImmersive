using UnityEngine;

public static class GestureDetector
{
    private const int Wrist = 0;

    private const int IndexMcp = 5;
    private const int IndexPip = 6;
    private const int IndexDip = 7;
    private const int IndexTip = 8;

    private const int MiddleMcp = 9;
    private const int MiddlePip = 10;
    private const int MiddleDip = 11;
    private const int MiddleTip = 12;

    private const int RingMcp = 13;
    private const int RingPip = 14;
    private const int RingDip = 15;
    private const int RingTip = 16;

    private const int PinkyMcp = 17;
    private const int PinkyPip = 18;
    private const int PinkyDip = 19;
    private const int PinkyTip = 20;

    private const float PipAngleThreshold = 145f;
    private const float DipAngleThreshold = 145f;
    private const float DistanceMultiplier = 1.05f;

    public static GestureType Detect(Vector3[] landmarks)
    {
        if (landmarks == null || landmarks.Length < 21)
            return GestureType.Unknown;

        bool indexOpen = IsFingerOpen(
            landmarks, IndexMcp, IndexPip, IndexDip, IndexTip);

        bool middleOpen = IsFingerOpen(
            landmarks, MiddleMcp, MiddlePip, MiddleDip, MiddleTip);

        bool ringOpen = IsFingerOpen(
            landmarks, RingMcp, RingPip, RingDip, RingTip);

        bool pinkyOpen = IsFingerOpen(
            landmarks, PinkyMcp, PinkyPip, PinkyDip, PinkyTip);

        if (indexOpen && middleOpen && !ringOpen && !pinkyOpen)
            return GestureType.Scissors;

        if (indexOpen && middleOpen && ringOpen && pinkyOpen)
            return GestureType.Paper;

        if (!indexOpen && !middleOpen && !ringOpen && !pinkyOpen)
            return GestureType.Rock;

        return GestureType.Unknown;
    }

    private static bool IsFingerOpen(
        Vector3[] landmarks,
        int mcpIndex,
        int pipIndex,
        int dipIndex,
        int tipIndex)
    {
        Vector2 wrist = ToVector2(landmarks[Wrist]);
        Vector2 mcp = ToVector2(landmarks[mcpIndex]);
        Vector2 pip = ToVector2(landmarks[pipIndex]);
        Vector2 dip = ToVector2(landmarks[dipIndex]);
        Vector2 tip = ToVector2(landmarks[tipIndex]);

        float pipAngle = CalculateAngle(mcp, pip, tip);
        float dipAngle = CalculateAngle(pip, dip, tip);

        float tipToWrist = Vector2.Distance(tip, wrist);
        float pipToWrist = Vector2.Distance(pip, wrist);

        bool jointsStraight =
            pipAngle >= PipAngleThreshold &&
            dipAngle >= DipAngleThreshold;

        bool tipFarEnough =
            tipToWrist >= pipToWrist * DistanceMultiplier;

        return jointsStraight && tipFarEnough;
    }

    private static float CalculateAngle(
        Vector2 first,
        Vector2 center,
        Vector2 third)
    {
        Vector2 directionA = first - center;
        Vector2 directionB = third - center;

        if (directionA.sqrMagnitude < 0.000001f ||
            directionB.sqrMagnitude < 0.000001f)
        {
            return 0f;
        }

        return Vector2.Angle(directionA, directionB);
    }

    private static Vector2 ToVector2(Vector3 point)
    {
        return new Vector2(point.x, point.y);
    }
}
