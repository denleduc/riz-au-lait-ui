using System.Collections;
using UnityEngine;

/*
 Permet d'énumérer les différents cas/fonctions pour les boutons du menu.
 */
public class EnumFonctionsBouton : MonoBehaviour
{
    private EnvoyerRecevoirDonnees e_r_d;
    private ComportementFenetrePreferences c_f_p;

    public void Start()
    {
        e_r_d = FindAnyObjectByType<EnvoyerRecevoirDonnees>();
        c_f_p = FindAnyObjectByType<ComportementFenetrePreferences>();
    }

    /*@brief, LancerRecherche() permet de démarrer la coroutine EnvoyerRecherche() sans être elle-même une coroutine.*/
    public void LancerRecherche()
    {
        StartCoroutine(EnvoyerRecherche());
    }

    /*@brief, EnvoyerRecherche() permet d'interrompre l'action en cours pour lancer une recherche côté serveur.
     @return IEnumerator, c'est une coroutine donc on la démarre avec StartCoroutine().*/
    public IEnumerator EnvoyerRecherche()
    {
        yield return StartCoroutine(e_r_d.EnvoyerRaccourci("2"));
    }

    /*@brief, LancerQuitter() permet de démarrer la coroutine EnvoyerQuitter() sans être elle-même une coroutine.*/
    public void LancerQuitter()
    {
        StartCoroutine(EnvoyerQuitter());
    }

    /*@brief, EnvoyerQuitter() permet d'interrompre l'action en cours pour quitter côté serveur.
     @return IEnumerator, c'est une coroutine donc on la démarre avec StartCoroutine().*/
    public IEnumerator EnvoyerQuitter()
    {
        yield return StartCoroutine(e_r_d.EnvoyerRaccourci("3"));
        Application.Quit();
    }

    /*@brief, LancerPrefs() permet d'ouvrir la fenêtre des préférences.*/
    public void LancerPrefs()
    {
        c_f_p.ChangerEtat();
    }
}
