using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/* Appelé par GestionnaireApplication. 
 * Permet de faire des requêtes vers le serveur aussi bien pour envoyer que pour recevoir des chose.
 On décrypte la requête connue du serveur et on l'adapte.
*/


public class EnvoyerRecevoirDonnees: MonoBehaviour
{
    private string base_url;
    public string Token { get; private set; } = "";

    private bool est_pret = false;

    private Action<ReponseServeur> OnServeurUpdate;
    private GestionnaireApplication gestapp;

    [Serializable]
    public class ReponseServeur
    {
        public string[] messages;
        public string[] debugs;
        public bool attente_saisie;
        public bool continuer;
    }

    /*@brief, Init() sert de constructeur ++ pour la classe EnvoyerRecevoirDonnees. Elle instancie les paramètres et lance la boucle principale.
     @param1 token, une chaine de caractère qui contient le token du client.
     @param2 URL, l'URL du serveur.
     @param3 callback, une fonction. Ici, il s'agit de OnServeurUpdate du GestionnaireApplication, qui lui permet de décrypter les données brutes reçues par le serveur. */
    public void Init(string token, string URL, Action<ReponseServeur> callback)
    {
        Token = token;
        est_pret = true;
        OnServeurUpdate = callback;
        base_url = URL;
        gestapp = FindAnyObjectByType<GestionnaireApplication>();

        Debug.Log("Client initialisé avec le token : " + Token);

        // On lance la boucle principale
        StartCoroutine(MainLoop());
    }

    /*@brief, MainLoop() est la boucle centrale de ce script. Elle permet de vérifier les nouvelles données du serveur.
     @return, c'est une coroutine, on l'appelle avec StartCoroutine().
    NOTE: on pourra modifier ces fonctions pour qu'elles ne fassent des requêtes que lorsque c'est nécessaire (ici on en fait toutes les deux secondes, même si rien n'a changé).*/
    IEnumerator MainLoop()
    {
        while (est_pret)
        {
            if(gestapp.GetEnvoyerRequete())
            {
                yield return GetServerUpdate();
                yield return new WaitForSeconds(2);
            }
            yield return null;
        }
    }

    /*@brief, GetServerUpdate() fait la requête au serveur et les transmet au GestionnaireApplication grâce à la fonction callback.
     @return IEnumerator, c'est une coroutine on l'appelle avec StartCoroutine()*/
    IEnumerator GetServerUpdate()
    {
        using (UnityWebRequest requete = UnityWebRequest.Get(base_url + "envoyer/" + Token))
        {
            yield return requete.SendWebRequest();

            if (requete.result != UnityWebRequest.Result.Success)
                Debug.LogWarning("Erreur update : " + requete.error);
            else
            {
                string json = requete.downloadHandler.text;
                ReponseServeur donnees_brutes = JsonUtility.FromJson<ReponseServeur>(json);
                Debug.Log("Données du serveur : " + json);
                // On va essayer de transmettre les données vers le gestionnaire du jeu
                OnServeurUpdate?.Invoke(donnees_brutes);
            }
        }
    }

    /*@brief, EnvoyerAction() permet d'envoyer des éléments au serveur de du type : "{"entree": "<ma_valeur>"}".
     * @param action, la chaine de caractères à envoyer au serveur.
     @return IEnumerator, c'est une coroutine, on l'appelle avec StartCoroutine().*/
    public IEnumerator EnvoyerAction(string action)
    {
        if (!est_pret) yield break;

        string jsonBody = "{\"entree\":\"" + action + "\"}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(base_url + "recevoir/" + Token, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                Debug.LogError("Erreur envoi : " + request.error);
            else
                Debug.Log("Action envoyée : " + action);
        }
        gestapp.SetEnvoyerRequete(true);
    }

    /*@brief, EnvoyerRaccourci() permet de transmettre une interruption au serveur de type : "{"raccourci": "<ma_valeur>"}". 
     @param interruption, une chaine de caractère contenant l'interruption. Les interruptions contiennent des valeur entières qui seront transformées par le serveur au besoin (par exemple "1", "2", ...).
     @return IEnumerator, c'est une coroutine donc on la démarre avec StartCoroutine().*/
    public IEnumerator EnvoyerRaccourci(string interruption)
    {
        if (!est_pret) yield break;
        gestapp.SetEnvoyerRequete(true);

        string jsonBody = "{\"raccourci\":\"" + interruption + "\"}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(base_url + "recevoir/" + Token, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                Debug.LogError("Erreur envoi : " + request.error);
            else
                Debug.Log("Action envoyée : " + interruption);
        }
    }
}
