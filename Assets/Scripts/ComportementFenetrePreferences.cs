using Oculus.Interaction;
using UnityEngine;

/*Ce script est à coller sur un objet vide (pour éviter de désactiver ses évènements quand la fenêtre des préférences est désactivée). */
public class ComportementFenetrePreferences : MonoBehaviour 
{
    private GameObject prefab;
    private bool est_visible = false;
    private CanvasGroup canvas;

    private void Start()
    {
        prefab = GameObject.FindWithTag("FenetrePrefs");
        canvas = prefab.GetComponent<CanvasGroup>();
        canvas.alpha = 0f;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
        prefab.GetComponent<RayInteractable>().enabled = false;
    }

    public void ChangerEtat()
    {
        est_visible = !est_visible;
        if (est_visible)
        {
            prefab.GetComponent<RayInteractable>().enabled = true;
            canvas.alpha = 1.0f;
            canvas.interactable = true;
            canvas.blocksRaycasts = true;
        }
        else
        {
            canvas.alpha = 0f;
            canvas.interactable = false;
            canvas.blocksRaycasts = false;
            prefab.GetComponent<RayInteractable>().enabled = false;
        }
            Debug.Log($"État de la fenêtre des préférences : {prefab.activeSelf}.");
    }
}
