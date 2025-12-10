using Meta.WitAi.TTS.Utilities;
using UnityEngine;

public class LigneTexteTTS
{
    public TTSSpeaker speaker;
    public string text;

    public LigneTexteTTS(TTSSpeaker speaker, string text)
    {
        this.speaker = speaker;
        this.text = text;
    }
}
