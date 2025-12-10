using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*Script à coller sur un bouton de la fenêtre des préférences pour activer la focntion (à coller dans l'inspecteur).*/
public class ComportementBoutonsPreferences : MonoBehaviour, IPointerClickHandler
{
    private GameObject bouton;
    [Header("Fonction à exécuter quand le bouton est pressé.")]
    public UnityEvent fonction;

    private void Start()
    {
        bouton = GameObject.FindWithTag("QDSUIToggleSwitch");
        if (fonction == null)
            Debug.LogError($"La fonction n'a pas été définie dans l'inspecteur ! Le bouton {bouton.name} est inutilisable.");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Bouton {bouton.name} cliqué");
        fonction?.Invoke();
    }
}
