using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/*Collez ce script sur un bouton dans une fenêtre type UI pour qu'il réagisse aux bons évènements. Si votre bouton se trouve dans une fenêtre, ce script est votre meilleure option.*/
public class ComportementBoutonsFenetreUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private AgrandissementBoutons ab;
    public UnityEvent onPointerClick;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ab = GetComponent<AgrandissementBoutons>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        ab.Agrandir();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ab.Retrecir();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(onPointerClick== null)
            Debug.LogError("Aucune fonction n'a été définie pour le bouton.");
        onPointerClick?.Invoke();
    }
}
