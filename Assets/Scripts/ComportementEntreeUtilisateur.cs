using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/*
 Ce script permet de gérer l'envoi au serveur au niveau du TMP_InputField.
 */

public class ComportementEntreeUtilisateur : MonoBehaviour
{
    private TMP_InputField Entree;
    private TouchScreenKeyboard clavier;
    private EnvoyerRecevoirDonnees envoyer_recevoir;
    private RunWhisper ia;
    private EnumFonctionsPreferences efp;

    private void Start()
    {
        Entree = GetComponentInChildren<TMP_InputField>();

        if (clavier == null)
            Debug.LogError("Aucun clavier trouvé ! ALERTE AU GOGOLE LES ENFANTS !!!");

        envoyer_recevoir = FindAnyObjectByType<EnvoyerRecevoirDonnees>();

        ia = FindAnyObjectByType<RunWhisper>();
        ia.OnTranscriptionComplete += HandleTranscriptionResult;
        efp = FindAnyObjectByType<EnumFonctionsPreferences>();
    }

    public void TraitementEntree()
    {
        Debug.Log("Entrée dans TraitementEntrée()");
        if (!string.IsNullOrWhiteSpace(Entree.text))
            StartCoroutine(envoyer_recevoir.EnvoyerAction(Entree.text));
        Entree.text = string.Empty;
        Entree.DeactivateInputField();
    }

    public void TraitementEntreeAudio(string s)
    {
        Debug.Log("On est censé pouvoir parler là.");

        if(ia != null)
        {
            ia.StartMicrophoneInference();
        }
    }

    private void HandleTranscriptionResult(string texteTranscrit)
    {
        Debug.Log("HandleTranscriptionResult() Transcription terminée reçue : " + texteTranscrit);

        if (!string.IsNullOrWhiteSpace(texteTranscrit))
        {
            Entree.text = texteTranscrit;
            Entree.caretPosition = Entree.text.Length;
            Entree.ForceLabelUpdate();
            Entree.MoveTextEnd(false);
        }
    }

    public TMP_InputField GetEntree() => Entree;

    void OnDestroy()
    {
        if (ia != null)
        {
            ia.OnTranscriptionComplete -= HandleTranscriptionResult;
        }
    }
}
