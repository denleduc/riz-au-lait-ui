using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

/*Faites vos tests avec ce fichier. À RETIRER DE LA VERSION FINALE.*/

public class TestInputsOVR : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI debugText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointeur entré sur bouton Fermer.");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointeur sorti du bouton Fermer.");
    }
}
