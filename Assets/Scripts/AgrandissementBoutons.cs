using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 Collez ce script à un bouton pour qu'il s'agrandisse progressivement.
NOTE: Ceci fonctionne mieux avec les backgrounds de Meta (Materials). C'est plus joli UwU.
 */

public class AgrandissementBoutons : MonoBehaviour
{
    public enum AnimationDir
    {
        DeuxCôtés = 0,
        Gauche = 1,
        Droite = 2
    };

    [Header("Animation : Ouverture des boutons")]
    private RectTransform bouton;
    public float largeur_init = 100f;
    public float largeur_max = 500f;
    public float duree_ouverture = 0.3f;
    public AnimationDir AnimerA = AnimationDir.DeuxCôtés;
    private Coroutine coroutine;
    private ComportementBoutonsMenu cbm;

    // Afficher le texte
    private TextMeshProUGUI texte;
    private bool est_visible = false;

    private bool micro_actif = false;

    private void Start()
    {
        cbm = FindAnyObjectByType<ComportementBoutonsMenu>();
        bouton = GetComponent<RectTransform>();
        texte = GetComponentInChildren<TextMeshProUGUI>();
        texte.gameObject.SetActive(est_visible);
    }

    /* @brief, Agrandir() permet de grossir la largeur du bouton (il s'étend à l'horizontale).*/
    public void AgrandirMenu()
    {
        if(!cbm.GetAnimEnCours() && !cbm.GetBoutonClique())
        {
            Debug.Log("AgrandirMenu() exécutée.");
            if (Mathf.Abs(bouton.sizeDelta.x - largeur_max) < 0.01f)
                return;
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(ChangerLargeur(largeur_max));
        }
    }

    /* @brief, Agrandir() permet de grossir la largeur du bouton (il s'étend à l'horizontale).*/
    public void Agrandir()
    {
            Debug.Log("Agrandir() exécutée.");
            if (Mathf.Abs(bouton.sizeDelta.x - largeur_max) < 0.01f)
                return;
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(ChangerLargeur(largeur_max));
    }

    /* @brief, Retrecir() permet de rétrécir un bouton vers sa valeur d'origine après un certain délai. Le délai permet d'éviter les clignotements des pointeurs, qui déclenchent des évènements.
     */
    public void Retrecir()
    {
        Debug.Log("Retrécir() appelée.");
        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(ChangerLargeur(largeur_init));
    }

    public void RetrecirMicro()
    {
        Debug.Log($"Entrée dans RetrecirMicro() avec un booléen qui vaut : {micro_actif}");
        if(!micro_actif)
        {
            Debug.Log("RetrécirMicro() appelée.");
            EventSystem.current.SetSelectedGameObject(null);
            GetComponent<Selectable>().OnDeselect(null);
            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(ChangerLargeur(largeur_init));
        }
    }

    public void OnToggleMicroButton()
    {
        Debug.Log($"OnToggleMicroButton() appelée avec un booléen à {micro_actif}.");
        micro_actif = !micro_actif;
    }

    /*@brief, ChangerLargeur() permet de passer la largeur d'un bouton de sa valeur de base à une valeur cible.
     @param1 cible, un flottant qui indique la taille vers laquelle le bouton doit évoluer.
     @return IEnumerator, c'est aussi une coroutine, on la lance avec StartCoroutine()*/
    private IEnumerator ChangerLargeur(float cible)
    {
        float depart = bouton.sizeDelta.x;
        Vector2 pos_depart = bouton.anchoredPosition;
        float t = 0;
        while (t < duree_ouverture)
        {
            t += Time.deltaTime;
            float w = Mathf.Lerp(depart, cible, t / duree_ouverture);
            bouton.sizeDelta = new Vector2(w, bouton.sizeDelta.y);
            if(bouton.sizeDelta.x/largeur_max > 0.8) est_visible = true;
            else est_visible = false;
            switch (AnimerA)
            {
                case AnimationDir.Gauche:
                    {
                        bouton.anchoredPosition = pos_depart - new Vector2((w - depart) / 2, 0);;
                        break;
                    }
                case AnimationDir.Droite:
                    {
                        Vector2 pos = bouton.anchoredPosition + new Vector2(w / 2, 0);
                        bouton.anchoredPosition = pos;
                        break;
                    }
                default:
                    break;
            }
            yield return null;
        }
    }

    private void Update()
    {
        texte.gameObject.SetActive(est_visible);
    }

    public float GetXDuBouton() => bouton.sizeDelta.x;

    public bool GetEtatMicro() => micro_actif;

    public void SetMicroActif(bool b)
    {
        micro_actif = b;
    }
}


