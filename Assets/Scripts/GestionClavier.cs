using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/*Je ne sais pas si ce script sert mais il permet d'ouvrir le clavier système du Quest. ATTENTION: CES ENC*LÉS de leurs grands morts de F*P de Meta ne précisent pas que ça ne FONCTIONNERA JAMAIS DANS LINK, IL FAUT ABSOLUMENT LE BUILD SI ON VEUT FAIRE APPARAÎTRE CE CLAVIER DU DÉMON.*/
public class GestionClavier : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public InputField entree;
    public OVRVirtualKeyboard clavier;
    private TouchScreenKeyboard overlayKeyboard;

    private void Start()
    {
        if (entree == null)
            entree = GetComponent<InputField>();
        if (clavier == null)
        {
            clavier = FindAnyObjectByType<OVRVirtualKeyboard>(FindObjectsInactive.Include);
            if (clavier == null)
            {
                Debug.LogError("Pas de clavier virtuel dans la scène !");
            }
        }

        if (clavier != null)
            clavier.gameObject.SetActive(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("Entrée sélectionnée, clavier affiché.");
        overlayKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        Debug.Log($"clavier = {clavier}, entree = {entree}, clavier système = {overlayKeyboard}");
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log("Entrée désélectionnée, clavier masqué.");
        if (clavier != null)
        {
            clavier.gameObject.SetActive(false);
        }
    }
}
