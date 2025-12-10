using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/*Ce script permet aux boutons de la fenêtre de préférences de fonctionner. */
public class EnumFonctionsPreferences : MonoBehaviour
{
    bool utiliser_microphone = false;

    private TextMeshProUGUI[] tab_texte;
    private AgrandissementBoutons[] tab_boutons;
    private GameObject[] fenetres;
    private Color[] tab_couleurs = new Color[3];
    private GameObject entree;
    private Image entree_img;
    private TextMeshProUGUI[] tab_entree_tmp;     

    // Définition des couleurs 
    private readonly Color c_bleu_j = new(65 / 255f, 105 / 255f, 225 / 255f);
    private readonly Color c_rouge_j = new(178 / 255f, 34 / 255f, 34 / 255f);
    private readonly Color c_vert_j = new(108 / 255f, 186 / 255f, 104 / 255f);

    private readonly Color c_bleu_n = new(100 / 255f, 149 / 255f, 237 / 255f);
    private readonly Color c_rouge_n = new(220 / 255f, 20 / 255f, 60 / 255f);
    private readonly Color c_vert_n = new(46 / 255f, 111 / 255f, 64 / 255f);

    private readonly Color c_jour = new(0.95f, 0.95f, 0.95f);
    private readonly Color c_nuit = new(0.35f, 0.35f, 0.35f);
    private bool jour = false;
    private TextMeshProUGUI debugs;
    private GameObject scrollview;
    private Image[] historique_image;
    private Scrollbar[] scrollbars;
    private Material rounded_box_bulles;
    private const string rounded_box_bulle_chemin = "Materials/RoundedBoxUIBulles";
    private Material triangle_bulles;
    private const string triangle_bulle_chemin = "Materials/TriangleMask";
    private readonly float alpha_bulles = 1;
    private List<TextMeshProUGUI> tab_historique = new();

    private GameObject choix_langue;


    private void Awake()
    {
        tab_texte = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        tab_boutons = FindObjectsByType<AgrandissementBoutons>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        fenetres = FonctionsUtilitaires.FindAllWithTag("QDSUIBackplateGradient");
        Debug.Log($"Valeur de fenetre : {fenetres}");

        entree = GameObject.FindWithTag("Entree");
        entree_img = entree.GetComponent<Image>();
        tab_entree_tmp = entree.GetComponentsInChildren<TextMeshProUGUI>();

        debugs = GameObject.FindWithTag("Debug").GetComponent<TextMeshProUGUI>();
        scrollview = GameObject.FindWithTag("Scrollview");

        historique_image = scrollview.GetComponentsInChildren<Image>();
        scrollbars = scrollview.GetComponentsInChildren<Scrollbar>(true);

        rounded_box_bulles = Resources.Load<Material>(rounded_box_bulle_chemin);
        triangle_bulles = Resources.Load<Material>(triangle_bulle_chemin);
        if (rounded_box_bulles == null)
        {
            Debug.LogError($"[Mode Nuit] Le Material à l'emplacement '{rounded_box_bulle_chemin}' est introuvable. Les couleurs des bulles ne changeront pas.");
        }
        choix_langue = GameObject.FindWithTag("ChoisirLangue");
        choix_langue.SetActive(false);
    }

    private void Start()
    {
        ChangerMode();
    }

    public void ChangerMode()
    {
        jour = !jour;
        debugs.text += $"Entrée dans ChangerMode() avec un booléen à {jour}.\n";
        if(jour)
        {
            Material m = Resources.Load<Material>("Materials/LinearGradientUILight");
            foreach (AgrandissementBoutons b in tab_boutons)
            {
                Debug.Log($"Bouton : {b}");
                b.GetComponentInChildren<Image>(true).color = c_jour;
            }
            foreach (TextMeshProUGUI t in tab_texte)
            {
                Debug.Log($"TMP : {t}");
                t.color = new Color(0.5f, 0.5f, 0.5f);
            }
            foreach (GameObject go in fenetres)
            {
                Debug.Log($"Fenetre : {fenetres}");
                go.GetComponentInChildren<Image>(true).material = m;
                Debug.Log($"Valeur de material : {go.GetComponentInChildren<Image>(true).material}");
            }
            tab_couleurs[0] = c_bleu_j;
            tab_couleurs[1] = c_rouge_j;
            tab_couleurs[2] = c_vert_j;
            scrollview.GetComponent<Image>().color = c_jour;
            foreach (Image im in historique_image)
            {
                im.color = new Color(1, 1, 1);
            }
            foreach (Scrollbar sc in scrollbars)
            {
                ColorBlock cb = sc.colors;
                cb.normalColor = c_jour;
                cb.highlightedColor = c_jour * 1.1f;
                cb.pressedColor = c_jour * 0.9f;
                sc.colors = cb;
            }
            Color nouvelle = c_jour;
            nouvelle.a = alpha_bulles;
            rounded_box_bulles.color = nouvelle;
            triangle_bulles.color = nouvelle;

            foreach (TextMeshProUGUI t in tab_historique)
            {
                if (t == null) continue;

                float alpha = t.color.a;
                Color couleur_tmp = c_jour; 

                if (t.color.Equals(c_bleu_n))
                {
                    couleur_tmp = c_bleu_j;
                }
                else if (t.color.Equals(c_rouge_n))
                {
                    couleur_tmp = c_rouge_j;
                }
                else if (t.color.Equals(c_vert_n))
                {
                    couleur_tmp = c_vert_j;
                }

                couleur_tmp.a = alpha;
                t.color = couleur_tmp;
            }

            entree_img.color = c_jour;
            foreach (TextMeshProUGUI tmp in tab_entree_tmp)
            {
                if (tmp == null) continue;

                Color c = new(0.3f, 0.3f, 0.3f);
                float alpha = tmp.color.a;
                c.a = alpha;
                tmp.color = c;
                
            }
        }
        
        else
        {
            Material m = Resources.Load<Material>("Materials/LinearGradientUIDark");
            foreach (AgrandissementBoutons b in tab_boutons)
                b.GetComponentInChildren<Image>(true).color = c_nuit;
            foreach (TextMeshProUGUI t in tab_texte)
                t.color = new Color(0.8f, 0.8f, 0.8f);
            foreach (GameObject go in fenetres)
                go.GetComponentInChildren<Image>(true).material = m;
            tab_couleurs[0] = c_bleu_n;
            tab_couleurs[1] = c_rouge_n;
            tab_couleurs[2] = c_vert_n;
            scrollview.GetComponent<Image>().color = c_nuit;
            foreach (Image im in historique_image)
            {
                im.color = c_nuit;
            }
            foreach (Scrollbar sc in scrollbars)
            {
                ColorBlock cb = sc.colors;
                cb.normalColor = c_nuit;
                cb.highlightedColor = c_nuit * 1.1f;
                cb.pressedColor = c_nuit * 0.9f;
                sc.colors = cb;
            }

            Color nouvelle = c_nuit;
            nouvelle.a = alpha_bulles;
            rounded_box_bulles.color = nouvelle;
            triangle_bulles.color = nouvelle;

            foreach (TextMeshProUGUI t in tab_historique)
            {
                if (t == null) continue;

                float alpha = t.color.a;
                Color couleur_tmp = c_nuit; 

                if (t.color.Equals(c_bleu_j))
                {
                    couleur_tmp = c_bleu_n;
                }
                else if (t.color.Equals(c_rouge_j))
                {
                    couleur_tmp = c_rouge_n;
                }
                else if (t.color.Equals(c_vert_j))
                {
                    couleur_tmp = c_vert_n;
                }
                couleur_tmp.a = alpha;
                t.color = couleur_tmp;
            }

            entree_img.color = c_nuit;
            foreach (TextMeshProUGUI tmp in tab_entree_tmp)
            {
                if (tmp == null) continue;

                Color c = new(0.8f, 0.8f, 0.8f);
                float alpha = tmp.color.a;
                c.a = alpha;
                tmp.color = c;

            }
        }
    }

    /*Permet d'activer le mode Text2Speech*/
    public void ChangerSaisie()
    {
        utiliser_microphone = !utiliser_microphone;
        choix_langue.SetActive(utiliser_microphone);
    }

    public bool GetSaisieAudio() => utiliser_microphone;

    public Color[] GetTabCouleurs() => tab_couleurs;

    public void SetMessageDansListeHistorique(TextMeshProUGUI tmp)
    {
        tab_historique.Add(tmp);
    }
}

