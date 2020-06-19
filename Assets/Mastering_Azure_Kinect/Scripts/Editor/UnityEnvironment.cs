using UnityEditor;
using UnityEngine;

[InitializeOnLoadAttribute]
public class UnityEnvironment : MonoBehaviour
{
    static UnityEnvironment()
    {
        EditorApplication.playModeStateChanged += LogPlayModeState;
    }

    private static void LogPlayModeState(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // Copy files
        }
    }
}
