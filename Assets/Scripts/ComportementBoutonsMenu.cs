using TMPro;
using UnityEngine;
using System.Collections;

/*Collez ce script dans sur un canvas pour le faire apparaître/disparaître quand le bouton des trois barres horizontales est cliqué (fonctionne aussi avec la main gauche).
 * Les boutons sont affichés du plus à gauche au plus à droite
 Recherche un composant avec un Tag Debug et un autre avec un Tag MenuBoutons*/

public class ComportementBoutonsMenu : MonoBehaviour
{
    private Canvas canvas;
    private bool est_visible = false, animation_en_cours = false;
    private TextMeshProUGUI debugs;
    private AgrandissementBoutons[] ab;
    private bool bouton_clique = false;

    [Header("Animation : Boutons")]
    public RectTransform[] boutons;
    public float rayon = 0.35f;

    [Header("Animation : Durée de l'apparition & déplacement des boutons (en secondes)")]
    public float duree_deplacement = 0.5f;

    private Vector2[] pos_init;
    private Vector2[] pos_cercle;

    void Start()
    {
        canvas = GameObject.FindWithTag("MenuBoutons")?.GetComponent<Canvas>();
        debugs = GameObject.FindWithTag("Debug")?.GetComponent<TextMeshProUGUI>();
        pos_init = new Vector2[boutons.Length];
        pos_cercle = new Vector2[boutons.Length];
        ab = new AgrandissementBoutons[boutons.Length];
        for(int i = 0; i < boutons.Length; i++)
        {
            pos_init[i] = new(0f, 0f);
            ab[i] = boutons[i].GetComponent<AgrandissementBoutons>();
        }
        pos_cercle = CalculerPositionCercle(boutons.Length, rayon);



        if (canvas != null )
            canvas.gameObject.SetActive(est_visible);
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            est_visible = !est_visible;
            animation_en_cours = true;
            bouton_clique = false;
            debugs.text += $"Update() boutons, valeur de bool : {est_visible} \n";
            if (est_visible)
            {
                canvas.gameObject.SetActive(est_visible);
                StartCoroutine(DeplacerBoutons(pos_cercle));
            }
            else
            {
                StartCoroutine(DeplacerBoutons(pos_init, () => canvas.gameObject.SetActive(est_visible)
                ));
            }
        }
    }

    /*@brief BoutonClique() permet de définir un comportement pour les boutons quand l'un d'entre eux est pressé. Ici, on demande à fermer le menu des boutons.*/
    public void BoutonClique()
    {
        bouton_clique = true;
        for(int i = 0; i < boutons.Length;i++)
        {
            if (ab[i].GetXDuBouton() > ab[i].largeur_init)
            {
                if (ab[i].duree_ouverture > duree_deplacement)
                {
                    float duree_tmp = ab[i].duree_ouverture;
                    ab[i].duree_ouverture = duree_deplacement / 2;
                    ab[i].Retrecir();
                    ab[i].duree_ouverture = duree_tmp;
                }
                else
                    ab[i].Retrecir();
            }
        }
        est_visible = false;
        StartCoroutine(DeplacerBoutons(pos_init,
                    () => canvas.gameObject.SetActive(est_visible)
                ));
    }

    /*@brief, DeplacerBoutons() permet de bouger les boutons d'une position 1 vers une position 2. On les fait glisser vers la position (animation, pas une téléportation).
     @param1 pos_cibles, position finale des boutons à la fin de la fonction.
     @param2 onFinish, fonction de "callback" qui permet d'appeler une fonction incompatible avec les coroutines dans une coroutine (ici gameObject.SetVisible() ).
    @return IEnumerator, c'est une coroutine, on l'appelle avec StartCoroutine().*/
    private IEnumerator DeplacerBoutons(Vector2[] pos_cibles, System.Action onFinish = null)
    {
        float t = 0f;

        Vector2[] pos_depart = new Vector2[boutons.Length];
        for(int i = 0; i < boutons.Length; i++)
        {
            pos_depart[i] = boutons[i].anchoredPosition;
        }

        while (t < duree_deplacement)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / duree_deplacement);

            for(int i = 0; i < boutons.Length; i++)
            {
                boutons[i].anchoredPosition = Vector2.Lerp(pos_depart[i], pos_cibles[i], p);
            }

            yield return null;
        }

        onFinish?.Invoke();
        animation_en_cours = false;
    }

    /*@brief GetAnimEnCours() est un accesseur. Permet de savoir si l'animation d'apparition du menu est en cours ou non.
     @return, un booléen.*/
    public bool GetAnimEnCours()
    {
        return animation_en_cours;
    }

    /*@brief GetBoutonClique() est un accesseur. Permet de savoir si l'utilisateur a cliqué sur un bouton ou non.
     @return, un booléen.*/
    public bool GetBoutonClique()
    {
        return bouton_clique;
    }

    /* @brief, CalculerPositionCercle retourne nb positons selon les paramètres angle_min/angle_max autour d'un cercle de rayon r.
      @param1 nb, un entier qui permet de connaitre le nombre de positions à calculer.
      @param2 r, le rayon du cercle.
     @return un tableau de Vector2 contenant les positions.*/
    private Vector2[] CalculerPositionCercle(int nb, float r)
    {
        Vector2[] resultats = new Vector2[nb];
        float angle_min = 0f;
        float angle_max = 180f;
        float pas_entre_boutons = (angle_min - angle_max) / (nb - 1);
        for(int i = 0; i < nb; i++)
        {
            float angle_degre = angle_max + i*pas_entre_boutons;
            float normaliser_en_rad = angle_degre * Mathf.Deg2Rad;
            resultats[i] = new Vector2(Mathf.Cos(normaliser_en_rad)*r, Mathf.Sin(normaliser_en_rad)*r);
        }
        return resultats;
    }
}
