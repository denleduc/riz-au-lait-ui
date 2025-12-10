using System.Collections;
using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

// Utilisé une seule fois par EnvoyerRecevoirDonnees pour créer un token

public class FastAPIClient : MonoBehaviour
{
    [System.Serializable]
    public class TokenInit
    {
        public string token;
    }
    public string Token = "";


    /*@brief, GetTokenDuServeur() récupère un token et le stocke dans une chaine de caractères Token.
     @param1 API_URL, une chaine de caractère qui contient l'adresse du serveur web.
    @return IEnumerator, c'est une coroutine, on la lance avec StartCoroutine().*/
    public IEnumerator  GetTokenDuServeur(string API_URL)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(API_URL))
        {
            // Envoie la requête et attend la réponse
            yield return request.SendWebRequest();

            // Vérifie les erreurs réseau ou HTTP
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur : " + request.error);
            }
            else
            {
                // Récupère la réponse JSON brute
                string json = request.downloadHandler.text;
                Debug.Log("Réponse brute : " + json);

                // Désérialise le JSON
                TokenInit token = JsonUtility.FromJson<TokenInit>(json);
                Token = token.token;
            }
        }
    }
}

