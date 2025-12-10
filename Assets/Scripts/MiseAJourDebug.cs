using UnityEngine;
using TMPro;

public class MiseAJourDebug : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Debug_text;

    void OnEnable()
    {
        Application.logMessageReceived += GererLog;
    }

    void GererLog(string logString, string stackTrace, LogType type)
    {
        Debug_text.text = $"{type}: {logString} at {stackTrace}";
    }
}
