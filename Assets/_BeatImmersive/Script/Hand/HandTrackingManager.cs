using UnityEngine;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using System.Reflection;

public class HandTrackingManager : MonoBehaviour
{
    [SerializeField]
    private HandLandmarkerRunner runner;

    public HandData LeftHand = new HandData();
    public HandData RightHand = new HandData();

    private bool printed = false;
    private void Update()
    {
        if (printed) return;

        if (runner == null)
            return;

        var result = runner.CurrentResult;

        if (result.handLandmarks == null || result.handLandmarks.Count == 0)
            return;

        var hand = result.handLandmarks[0];

        Debug.LogWarning("===== FIELDS =====");

        foreach (var field in hand.GetType().GetFields(
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance))
        {
            Debug.LogWarning($"{field.Name} : {field.FieldType}");
        }

        Debug.LogWarning("===== PROPERTIES =====");

        foreach (var property in hand.GetType().GetProperties(
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance))
        {
            Debug.LogWarning($"{property.Name} : {property.PropertyType}");
        }

        printed = true;
    }
}