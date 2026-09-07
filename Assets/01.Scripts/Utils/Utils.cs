using System.Diagnostics;
using UnityEngine;

public static class Utils
{
    [Conditional("UNITY_EDITOR")]
    public static void Log(string message)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Log(message);
#endif
    }

    public static void LogError(string message)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.LogError(message);
#endif
    }

    public static void LogWarning(string message)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.LogWarning(message);
#endif
    }
}
