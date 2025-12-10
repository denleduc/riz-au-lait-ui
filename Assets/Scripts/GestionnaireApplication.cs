using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Meta.WitAi.TTS.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * Le script au coeur de l'interface. Il crée un token, permet les échanges serveur et redirige les messages du serveur au bon endroit.
 */

public class GestionnaireApplication : MonoBehaviour
{
    [SerializeField] private FastAPIClient init_token_client;
    [SerializeField] private EnvoyerRecevoirDonnees envoyer_recevoir;
    [SerializeField] private TextMeshProUGUI DebugsTMP;
    [SerializeField] private TMP_InputField Entree;
    [SerializeField] private float delai_entre_deux_frappes = 0.04f;
    [SerializeField] private ManagerTTS managerTTS;
    private bool afficher_entree = false, envoyer_requete = true, continuer = true;
    private const string base_url = "http://" + BuildConstants.LocalIP + ":8000/";
    private TextMeshProUGUI accueil;
    private TextMeshProUGUI[] pour;
    private TextMeshProUGUI[] contre;
    private string[] messages;
    private Queue<(string message, int index_couleur)> queue_msg;
    private EnumFonctionsPreferences efp;
    private GameObject texte_prefab, historique_prefab;
    private ScrollRect scrollview_historique;
    private const float seuil_descente_auto = 0.05f;
    public Transform content_historique;
    private GameObject bouton_micro;

    private void Awake()
    {
        efp = FindAnyObjectByType<EnumFonctionsPreferences>();
        accueil = GameObject.FindWithTag("Accueil").GetComponentInChildren<TextMeshProUGUI>();
        historique_prefab = GameObject.FindWithTag("TextePrefab");
        GameObject clone = Instantiate(historique_prefab);
        clone.SetActive(false);
        TextMeshProUGUI clone_tmp = clone.GetComponent<TextMeshProUGUI>();
        ConfigurerPrefabModele(clone_tmp);
        texte_prefab = clone;
        texte_prefab.transform.SetParent(null);
        pour = new TextMeshProUGUI[5];
        contre = new TextMeshProUGUI[5];
        pour[0] = GameObject.FindWithTag("Pour").GetComponentInChildren<TextMeshProUGUI>();
        contre[0] = GameObject.FindWithTag("Contre").GetComponentInChildren<TextMeshProUGUI>();
        scrollview_historique = GameObject.FindWithTag("Scrollview").GetComponent<ScrollRect>();
        bouton_micro = GameObject.FindWithTag("Micro");
    }

    private IEnumerator Start()
    {
        // Unity appelle Start() tout seul

        Debug.Log($"URL : {base_url}");
        DebugsTMP.text = "URL" + base_url + "\n";
        yield return StartCoroutine(init_token_client.GetTokenDuServeur(base_url));
        envoyer_recevoir.Init(init_token_client.Token,base_url, OnServeurUpdate);
    }

    private void ConfigurerPrefabModele(TextMeshProUGUI t)
    {
        t.text = "";
        t.fontStyle = FontStyles.Normal;
        t.fontSize = 12;
        t.alignment = TextAlignmentOptions.TopLeft;
        RectTransform rect = t.GetComponent<RectTransform>();
        rect.anchorMin = new(0,1);
        rect.anchorMax = new(1,1);
        rect.pivot = rect.anchorMin;
        rect.sizeDelta = new(0,25f);
    }

    /*@brief OnServeurUpdate() est appelée pour décrypter les messages du serveur.
     @param1 donnees, les données brutes de la réponse du serveur. On les défait en plusieurs morceaux(un par entrée dans le dictionnaire).*/
    private void OnServeurUpdate (EnvoyerRecevoirDonnees.ReponseServeur donnees)
    {
        messages = donnees.messages;
        DebugsTMP.text = string.Join("\n", donnees.messages);
        afficher_entree = donnees.attente_saisie;
        continuer = donnees.continuer;

        DemarrerLectureMessages();
    }

    private void Update()
    {
        if (Entree.IsActive() != afficher_entree)
        {
            Entree.transform.parent.gameObject.SetActive(afficher_entree);
        }
        if (!efp.GetSaisieAudio() && Entree.IsActive())
        {
            bouton_micro.SetActive(false);
        }
        else
        {
            bouton_micro.SetActive(true);
        }

        if (scrollview_historique.verticalNormalizedPosition < seuil_descente_auto)
            Descendre();
        // Si la barre vient d'apparaître, on la force à aller en bas.
        else if (scrollview_historique.verticalNormalizedPosition > 1)
            Descendre();
    }

    public bool GetEnvoyerRequete() => envoyer_requete;

    public bool SetEnvoyerRequete(bool value) => envoyer_requete = value;

    private void DemarrerLectureMessages()
    {
        queue_msg = GestionTexte.CreerQueueDepuisTexte(messages.ToList(), DebugsTMP);
        StartCoroutine(LireMessagesQueue());
    }

    private IEnumerator LireMessagesQueue()
    {
        envoyer_requete = false;
        Color[] couleurs = efp.GetTabCouleurs();
        while (queue_msg.Count > 0)
        {
            var message_en_cours = queue_msg.Dequeue();
            string msg = message_en_cours.message;
            int index_couleur = message_en_cours.index_couleur;
            DebugsTMP.text += ($"J'ai lu le message {msg} avec la couleur {index_couleur}. Sa couleur sera (en RGB) : {couleurs[index_couleur]}. La scrollbar est à {scrollview_historique.verticalNormalizedPosition}\n");
            TTSSpeaker speakerActuel = new();

            switch (index_couleur)
            {
                case 0:
                    {
                        AfficherMessageAccueil(msg);
                        speakerActuel = managerTTS.bobSpeaker;
                        break;
                    }
                case 1:
                    {
                        DebugsTMP.text += "cas contre";
                        AfficherMessageContre(msg);
                        
                        speakerActuel = managerTTS.carlSpeaker;
                        break;
                    }
                case 2:
                    {
                        DebugsTMP.text += "cas pour";
                        AfficherMessagePour(msg);
                        speakerActuel = managerTTS.caelSpeaker;
                        break;
                    }

            }

            yield return StartCoroutine(EcrireEtLireMessage(msg, couleurs[index_couleur], speakerActuel));
        }
    }

    private void AfficherMessageAccueil(string msg)
    {
        accueil.text = "";
        accueil.text = msg;
    }
    private void AfficherMessagePour(string msg)
    {
        pour[0].text = "";
        pour[0].text = msg;
    }
    private void AfficherMessageContre(string msg)
    {
        contre[0].text = "";
        contre[0].text = msg;
    }

    private IEnumerator EcrireEtLireMessage(string msg, Color couleur, TTSSpeaker ttss)
    {
        GameObject nouveau_texte = Instantiate(texte_prefab, content_historique);

        nouveau_texte.transform.SetParent(content_historique);
        nouveau_texte.SetActive(true);

        TextMeshProUGUI tmp = nouveau_texte.GetComponent<TextMeshProUGUI>();
        efp.SetMessageDansListeHistorique(tmp);
        tmp.color = couleur;
        tmp.text = "";
        DebugsTMP.text += ($"EcrireMessage(): Ecriture d'un texte avec la couleur {tmp.color}");

        // Paroles
        if(ttss != null)
        {
            ttss.Speak(msg);

            yield return null;
        }

        for (int i = 0; i < msg.Length; i++)
        {
            tmp.text += msg[i];
            yield return new WaitForSeconds(delai_entre_deux_frappes);
        }
        tmp.text += "\n";

        // Attente de la fin de la parole
        if (ttss != null)
        {
            bool isSpeaking = true;

            // Listener temporaire pour synchroniser le texte et l'audio. coroutine
            UnityEngine.Events.UnityAction<AudioClip> listener = null;
            listener = (t) =>
            {
                isSpeaking = false;
                // IMPORTANT : Se désabonner immédiatement pour ne pas interférer avec d'autres lignes
                ttss.Events.OnAudioClipPlaybackFinished.RemoveListener(listener);
            };

            // S'abonner à l'événement du speaker actuel
            ttss.Events.OnAudioClipPlaybackFinished.AddListener(listener);

            Debug.Log($"[TTS] Attente de la fin de parole de {ttss.name}...");

            // Boucle de blocage de la coroutine : on attend que l'événement soit déclenché
            while (ttss.IsSpeaking)
            {
                yield return null;
            }

            if (!isSpeaking)
            {
                ttss.Events.OnAudioClipPlaybackFinished.RemoveListener(listener);
                Debug.LogWarning($"[TTS] Avertissement: L'événement de fin a été manqué pour {ttss.name}. Nettoyage forcé du listener.");
            }

            Debug.Log($"[TTS] Fin de parole de {ttss.name} atteinte.");
        }
    }

    public void Descendre()
    {
        // On force le rafraîchissement immédiat du Layout Group 
        // pour que le Content Size Fitter recalcule la taille totale avant de scroller.
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollview_historique.content);

        // Ajuste la position verticale au maximum (le bas)
        // La position verticale est 0.0 en haut et 1.0 en bas de la Scroll View.
        scrollview_historique.verticalNormalizedPosition = 0f;
    }

    public bool GetAffcherEntree() => afficher_entree;
}
