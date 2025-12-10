using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PNJInteraction : MonoBehaviour
{
    [SerializeField] private AudioSource audio;
    public void Interaction()
    {
        Debug.Log("Bonjour, je suis un \"Nolant Peasant Blue\".");
        audio.Play();
        StartCoroutine(MajSource());
    }

    IEnumerator MajSource()
    {
        string url = "/audio/2160";
        using (UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(BuildConstants.LocalIP + ":8000" + url, AudioType.WAV))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                audio.clip = DownloadHandlerAudioClip.GetContent(req);
            }
            else
            {
                Debug.LogError(req.error);
            }
        }
    }   
}
